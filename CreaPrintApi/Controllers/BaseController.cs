using Microsoft.AspNetCore.Mvc;
using CreaPrintCore.Services;
using CreaPrintCore.Models;

namespace CreaPrintApi.Controllers
{
 [ApiController]
 public abstract class BaseController : ControllerBase
 {
 private readonly CurrentUser _currentUserService;

 protected BaseController(CurrentUser currentUserService)
 {
 _currentUserService = currentUserService;
 }

 /// <summary>
 /// The currently authenticated user, or null if not authenticated.
 /// </summary>
 protected User? CurrentUser => _currentUserService?.User;
 }
}
