using System.Security.Claims;
using CreaPrintCore.Services;
using CreaPrintCore.Interfaces;
using Serilog;
using System.Linq;

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
            currentUser.User = null; // reset default

            // If request is not authenticated, continue the pipeline
            if (context.User?.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            // Debug: log all incoming claims so we can inspect what's present
            try
            {
                var claimsInfo = context.User.Claims.Select(c => new { c.Type, c.Value }).ToArray();
            }
            catch { }

            // Candidate claim names that may contain the user id
            var idClaimCandidates = new[]
            {
                ClaimTypes.NameIdentifier,
                System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
                "sub",
                "nameid",
                "id",
                "userid",
                "user_id",
                // sometimes mapped
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            };

            foreach (var claimName in idClaimCandidates)
            {
                var claim = context.User.FindFirst(claimName);
                if (claim == null || string.IsNullOrWhiteSpace(claim.Value)) continue;

                if (int.TryParse(claim.Value, out var id))
                {
                    var userById = await userRepo.GetByIdAsync(id);
                    if (userById != null)
                    {
                        currentUser.User = userById;
                        break;
                    }
                }
            }

            // If not resolved by id, try username claims
            if (currentUser.User == null)
            {
                var usernameCandidates = new[]
                {
                    ClaimTypes.Name,
                    ClaimTypes.Upn,
                    "unique_name",
                    System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName,
                    "preferred_username",
                    "email",
                    "username"
                };

                foreach (var claimName in usernameCandidates)
                {
                    var claim = context.User.FindFirst(claimName);
                    if (claim == null || string.IsNullOrWhiteSpace(claim.Value)) continue;

                    var userByName = await userRepo.GetByUsernameAsync(claim.Value);
                    if (userByName != null)
                    {
                        currentUser.User = userByName;
                        break;
                    }
                }
            }

            // As a last resort, try any numeric claim
            if (currentUser.User == null)
            {
                var numeric = context.User.Claims.FirstOrDefault(c => int.TryParse(c.Value, out _));
                if (numeric != null && int.TryParse(numeric.Value, out var numericId))
                {
                    var userByNumeric = await userRepo.GetByIdAsync(numericId);
                    if (userByNumeric != null)
                    {
                        currentUser.User = userByNumeric;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log but do not block the request pipeline
            try { Log.Logger.Warning(ex, "CurrentUserMiddleware failed to resolve current user"); } catch { }
        }

        await _next(context);
    }
}
