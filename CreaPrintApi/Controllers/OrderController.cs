using Microsoft.AspNetCore.Mvc;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Services;

namespace CreaPrintApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : BaseController
{
 private readonly IOrderService _orderService;

 public OrdersController(CurrentUser currentUser, IOrderService orderService)
 : base(currentUser)
 {
 _orderService = orderService;
 }

 // POST /api/orders/checkout - create an order from the current user's basket
 [HttpPost("checkout")]
 //[Authorize]
 public async Task<IActionResult> Checkout()
 {
 var user = CurrentUser;
 if (user == null) return Unauthorized();
 var userId = user.Id;

 var order = await _orderService.CreateFromBasketAsync(userId);
 if (order == null) return BadRequest(new { error = "Cannot create order (empty basket or error)" });

 return Created($"/api/orders/{order.Id}", order);
 }

 // GET /api/orders/me - list orders for current user
 [HttpGet("me")]
 //[Authorize]
 public async Task<IActionResult> GetMyOrders()
 {
 var user = CurrentUser;
 if (user == null) return Unauthorized();
 var userId = user.Id;

 var orders = await _orderService.GetByUserAsync(userId);
 return Ok(orders);
 }

 // GET /api/orders/{id} - get a single order, ensure owner
 [HttpGet("{id}")]
 //[Authorize]
 public async Task<IActionResult> GetById(int id)
 {
 var user = CurrentUser;
 if (user == null) return Unauthorized();
 var userId = user.Id;

 var order = await _orderService.GetByIdAsync(id);
 if (order == null) return NotFound();
 if (order.UserId != userId) return Forbid();
 return Ok(order);
 }
}
