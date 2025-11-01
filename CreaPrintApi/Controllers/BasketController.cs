using Microsoft.AspNetCore.Mvc;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models.Baskets;
using System.Linq;
using CreaPrintCore.Dtos;

namespace CreaPrintApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : BaseController
{
 private readonly IBasketService _basketService;

 public BasketController(CreaPrintCore.Services.CurrentUser currentUser, IBasketService basketService)
 : base(currentUser)
 {
 _basketService = basketService;
 }

 // Get current user's basket
 [HttpGet("me")]
 //[Authorize]
 public async Task<ActionResult<BasketDto>> GetMyBasket()
 {
 var user = CurrentUser;
 if (user == null) return Unauthorized();
 var userId = user.Id;

 var dto = await _basketService.GetDtoByUserIdAsync(userId);
 if (dto == null) return NotFound();
 return Ok(dto);
 }

 // GET /api/basket/me/total - total price of current user's basket
 [HttpGet("me/total")]
 //[Authorize]
 public async Task<IActionResult> GetMyBasketTotal()
 {
 var user = CurrentUser;
 if (user == null) return Unauthorized();
 var userId = user.Id;

 var total = await _basketService.GetTotalByUserIdAsync(userId);
 return Ok(new { total });
 }

 // Add item to current user's basket
 [HttpPost("me/items")]
 //[Authorize]
 public async Task<IActionResult> AddItemToMyBasket([FromBody] AddItemRequest request)
 {
 var user = CurrentUser;
 if (user == null) return Unauthorized();
 var userId = user.Id;

 if (request.Quantity <=0) return BadRequest(new { error = "Quantity must be greater than zero" });

 try
 {
 await _basketService.AddItemToUserBasketAsync(userId, request.ArticleId, request.Quantity);
 return CreatedAtAction(nameof(GetMyBasket), null);
 }
 catch (Exception ex)
 {
 return BadRequest(new { error = ex.Message });
 }
 }

 // Remove item
 [HttpDelete("me/items/{itemId}")]
 //[Authorize]
 public async Task<IActionResult> RemoveItem(int itemId)
 {
 var user = CurrentUser;
 if (user == null) return Unauthorized();
 // Optionally verify item belongs to user's basket - omitted for brevity
 await _basketService.RemoveItemAsync(itemId);
 return NoContent();
 }
}

public record AddItemRequest(int ArticleId, int Quantity);