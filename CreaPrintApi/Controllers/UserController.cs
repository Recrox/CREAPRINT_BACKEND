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

        var tokenStr = GenerateToken(user);
        Log.Logger.Information("User {Username} authenticated", request.Username);
        return Ok(new { token = tokenStr });
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

        var tokenStr = GenerateToken(user);
        var expiresIn = 60 * 60 * 2; //2 hours
        return Ok(new { access_token = tokenStr, token_type = "bearer", expires_in = expiresIn });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<User>> Create([FromBody] CreateUserRequest request)
    {
        var user = new User { Username = request.Username, Email = request.Email, Rights = (UserRights)request.Rights };
        var created = await _service.CreateAsync(user, request.Password);

        // Send welcome email
        await _emailService.SendEmailAsync(created.Email, "Welcome to CreaPrint", $"Hello {created.Username}, your account has been created.");

        return CreatedAtAction(null, new { id = created.Id }, new { created.Id, created.Username });
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
        if (parts.Length != 2 || !parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
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
