using Microsoft.AspNetCore.Mvc;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CreaPrintApi.Services;
using Serilog;
using System.Linq;
using CreaPrintCore.Services;

namespace CreaPrintApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    private readonly IUserService _service;
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _configuration;
    private readonly ITokenBlacklist _blacklist;
    private readonly IEmailService _emailService;

    public UserController(CurrentUser currentUser, IUserService service, IUserRepository userRepo, IConfiguration configuration, ITokenBlacklist blacklist, IEmailService emailService)
        : base(currentUser)
    {
        _service = service;
        _userRepo = userRepo;
        _configuration = configuration;
        _blacklist = blacklist;
        _emailService = emailService;
    }

    [HttpPost("authenticate")]
    public async Task<ActionResult> Authenticate([FromBody] LoginRequest request)
    {
        var user = await _service.AuthenticateAsync(request.Username, request.Password);
        if (user == null)
        {
            Log.Logger.Information("Failed login for {Username}", request.Username);
            return Unauthorized();
        }

        //// deny login if user not active
        //if (!user.IsActive)
        //{
        //    Log.Logger.Information("Attempt to login with inactive account {Username}", request.Username);
        //    return Unauthorized(new { error = "Account not activated" });
        //}

        var accessToken = GenerateToken(user);
        var refreshToken = GenerateRefreshToken(user);
        Log.Logger.Information("User {Username} authenticated", request.Username);
        var expiresIn =60 *60 *2; //2 hours
        return Ok(new { access_token = accessToken, refresh_token = refreshToken, token_type = "bearer", expires_in = expiresIn });
    }

    // Alias endpoint: POST /api/user/login
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        // re-use same logic as Authenticate
        return await Authenticate(request);
    }

    // OAuth2 token endpoint (password flow) - accepts form data
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Token([FromForm] string username, [FromForm] string password)
    {
        var user = await _service.AuthenticateAsync(username, password);
        if (user == null) return Unauthorized();

        //if (!user.IsActive) return Unauthorized(new { error = "Account not activated" });

        var accessToken = GenerateToken(user);
        var refreshToken = GenerateRefreshToken(user);
        var expiresIn =60 *60 *2; //2 hours
        return Ok(new { access_token = accessToken, refresh_token = refreshToken, token_type = "bearer", expires_in = expiresIn });
    }

    // Refresh endpoint: POST /api/user/token/refresh
    [HttpPost("token/refresh")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Refresh([FromForm(Name = "refresh_token")] string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) return BadRequest(new { error = "refresh_token missing" });

        // Check revocation
        if (_blacklist.IsRevoked(refreshToken)) return Unauthorized(new { error = "Refresh token revoked or invalid" });

        var jwtKey = _configuration["Jwt:Key"] ?? "dev_secret_change_me_long_enough";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            }, out var validatedToken);

            // ensure token is a JWT and has the expected 'type' claim
            var jwt = validatedToken as JwtSecurityToken;
            if (jwt == null) return Unauthorized(new { error = "Invalid token" });

            var typeClaim = principal.FindFirst("type")?.Value;
            if (typeClaim != "refresh") return Unauthorized(new { error = "Invalid token type" });

            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(sub, out var userId)) return Unauthorized();

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return Unauthorized();

            // rotate refresh token: revoke old, issue new
            _blacklist.RevokeToken(refreshToken);

            var newAccess = GenerateToken(user);
            var newRefresh = GenerateRefreshToken(user);
            var expiresIn =60 *60 *2; //2 hours for access token

            return Ok(new { access_token = newAccess, refresh_token = newRefresh, token_type = "bearer", expires_in = expiresIn });
        }
        catch (SecurityTokenException)
        {
            return Unauthorized(new { error = "Invalid or expired refresh token" });
        }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<User>> Create([FromBody] CreateUserRequest request)
    {
        var user = new User { Username = request.Username, Email = request.Email, Rights = (UserRights)request.Rights };
        var created = await _service.CreateAsync(user, request.Password);

        // Build activation link
        var frontendBase = _configuration["FrontendBaseUrl"] ?? _configuration["App:BaseUrl"] ?? "";
        var activationPath = $"/api/user/activate?token={created.ActivationToken}";
        var activationLink = string.IsNullOrEmpty(frontendBase) ? activationPath : frontendBase.TrimEnd('/') + activationPath;

        // Send welcome email with activation link
        var emailBody = $"Hello {created.Username}, your account has been created. Please activate it by visiting: {activationLink}";
        await _emailService.SendEmailAsync(created.Email, "Welcome to CreaPrint - Activate your account", emailBody);

        return CreatedAtAction(null, new { id = created.Id }, new { created.Id, created.Username });
    }

    [HttpGet("activate")]
    [AllowAnonymous]
    public async Task<IActionResult> Activate([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token)) return BadRequest(new { error = "Token missing" });

        var user = await _userRepo.GetByActivationTokenAsync(token);
        if (user == null) return NotFound(new { error = "Invalid or expired token" });

        user.IsActive = true;
        user.ActivationToken = null;
        user.ActivationTokenExpires = null;
        await _userRepo.UpdateAsync(user);

        return Ok(new { message = "Account activated" });
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var users = await _service.GetAllAsync();
        return Ok(users.Select(u => new { u.Id, u.Username }));
    }

    // Simple GET by id: GET /api/user/{id}
    [HttpGet("{id}")]
    //[Authorize]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(new { user.Id, user.Username });
    }

    // Logout endpoint: revokes the current bearer token
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // Expecting Authorization: Bearer <token>
        var auth = Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(auth)) return BadRequest(new { error = "Authorization header missing" });

        var parts = auth.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length !=2 || !parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { error = "Invalid Authorization header" });

        var token = parts[1];
        _blacklist.RevokeToken(token);
        Log.Logger.Information("Token revoked for request by {User}", User?.Identity?.Name ?? "unknown");
        return NoContent();
    }

    private string GenerateToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "dev_secret_change_me_long_enough";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
 // include both JWT standard claims and System.Security.Claims types to ensure availability
 new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
 new Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
 new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
 new Claim("rights", ((int)user.Rights).ToString())
 };

        var token = new JwtSecurityToken(
        issuer: null,
        audience: null,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "dev_secret_change_me_long_enough";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
 new Claim("type", "refresh")
 };

        var token = new JwtSecurityToken(
        issuer: null,
        audience: null,
        claims: claims,
        expires: DateTime.UtcNow.AddDays(30), // long lived refresh token
        signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // GET /api/user/me - returns the current authenticated user
    [HttpGet("me")]
    //[Authorize]
    public ActionResult<User?> GetCurrentUser()
    {
        var user = CurrentUser; // BaseController provides CurrentUser (may be null)
        if (user == null) return Unauthorized();

        return Ok(user);
    }

    // POST /api/user/change-password - change current user's password
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var user = CurrentUser;
        if (user == null) return Unauthorized();

        var success = await _service.ChangePasswordAsync(user.Id, request.CurrentPassword, request.NewPassword);
        if (!success) return BadRequest(new { error = "Current password invalid" });

        // Notify user by email
        await _emailService.SendEmailAsync(user.Email, "Password changed", "Your password has been changed.");

        return NoContent();
    }
}

public record LoginRequest(string Username, string Password);
public record CreateUserRequest(string Username, string Password, string Email, int Rights =0);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
