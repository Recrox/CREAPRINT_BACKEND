using System.Security.Claims;
using CreaPrintCore.Services;
using CreaPrintCore.Interfaces;

namespace CreaPrintApi.Middleware;

public class CurrentUserMiddleware
{
 private readonly RequestDelegate _next;

 public CurrentUserMiddleware(RequestDelegate next)
 {
 _next = next;
 }

 public async Task InvokeAsync(HttpContext context, CurrentUser currentUser, IUserRepository userRepo)
 {
 try
 {
 // Try to read an id from common claim names
 var sub = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
 ?? context.User?.FindFirst("sub")?.Value;

 if (!string.IsNullOrEmpty(sub) && int.TryParse(sub, out var id))
 {
 var user = await userRepo.GetByIdAsync(id);
 currentUser.User = user;
 }
 else
 {
 // Fallback: try to resolve by username from different claim names
 var username = context.User?.FindFirst(ClaimTypes.Name)?.Value
 ?? context.User?.FindFirst(ClaimTypes.Upn)?.Value
 ?? context.User?.FindFirst("unique_name")?.Value
 ?? context.User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName)?.Value;

 if (!string.IsNullOrEmpty(username))
 {
 var user = await userRepo.GetByUsernameAsync(username);
 currentUser.User = user;
 }
 }
 }
 catch
 {
 // ignore errors - do not block request pipeline
 }

 await _next(context);
 }
}
