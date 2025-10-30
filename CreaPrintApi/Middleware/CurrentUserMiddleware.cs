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
 var sub = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User?.FindFirst("sub")?.Value;
 if (!string.IsNullOrEmpty(sub) && int.TryParse(sub, out var id))
 {
 var user = await userRepo.GetByIdAsync(id);
 currentUser.User = user;
 }
 }
 catch { /* ignore */ }

 await _next(context);
 }
}
