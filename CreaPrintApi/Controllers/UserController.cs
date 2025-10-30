using Microsoft.AspNetCore.Mvc;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CreaPrintApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
 private readonly IUserService _service;
 private readonly IConfiguration _configuration;

 public UserController(IUserService service, IConfiguration configuration)
 {
 _service = service;
 _configuration = configuration;
 }

 [HttpPost("authenticate")]
 public async Task<ActionResult> Authenticate([FromBody] LoginRequest request)
 {
 var user = await _service.AuthenticateAsync(request.Username, request.Password);
 if (user == null) return Unauthorized();

 var tokenStr = GenerateToken(user);
 return Ok(new { token = tokenStr });
 }

 // OAuth2 token endpoint (password flow) - accepts form data
 [HttpPost("token")]
 [Consumes("application/x-www-form-urlencoded")]
 public async Task<IActionResult> Token([FromForm] string username, [FromForm] string password)
 {
 var user = await _service.AuthenticateAsync(username, password);
 if (user == null) return Unauthorized();

 var tokenStr = GenerateToken(user);
 var expiresIn =60 *60 *2; //2 hours
 return Ok(new { access_token = tokenStr, token_type = "bearer", expires_in = expiresIn });
 }

 [HttpPost]
 [AllowAnonymous]
 public async Task<ActionResult<User>> Create([FromBody] CreateUserRequest request)
 {
 var user = new User { Username = request.Username };
 var created = await _service.CreateAsync(user, request.Password);
 return CreatedAtAction(null, new { id = created.Id }, new { created.Id, created.Username });
 }

 [HttpGet]
 [Authorize(Policy = "AdminOnly")]
 public async Task<ActionResult<IEnumerable<object>>> GetAll()
 {
 var users = await _service.GetAllAsync();
 return Ok(users.Select(u => new { u.Id, u.Username }));
 }

 private string GenerateToken(User user)
 {
 var jwtKey = _configuration["Jwt:Key"] ?? "dev_secret_change_me_long_enough";
 var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
 var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

 var claims = new[] {
 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
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
}

public record LoginRequest(string Username, string Password);
public record CreateUserRequest(string Username, string Password);
