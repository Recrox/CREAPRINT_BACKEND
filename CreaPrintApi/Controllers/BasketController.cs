using Microsoft.AspNetCore.Mvc;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using CreaPrintCore.Services;

namespace CreaPrintApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : BaseController
{
 private readonly IBasketService _basketService;

 public BasketController(CurrentUser currentUser, IBasketService basketService)
 : base(currentUser)
 {
 _basketService = basketService;
 }

 // Get current user's basket
 [HttpGet("me")]
 //[Authorize]
 public async Task<IActionResult> GetMyBasket()
 {
 var user = CurrentUser;
 if (user == null) return Unauthorized();
 var userId = user.Id;

 var basket = await _basketService.GetByUserIdAsync(userId);
 if (basket == null) return NotFound();
 return Ok(new { basket.Id, Items = basket.Items?.Select(i => new { i.Id, i.ArticleId, i.Quantity, Article = i.Article == null ? null : new { i.Article.Id, i.Article.Title, i.Article.Price } }) });
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

 var basket = await _basketService.GetByUserIdAsync(userId);
 if (basket == null)
 {
 basket = new Basket { UserId = userId };
 basket = await _basketService.CreateAsync(basket);
 }

 var item = new BasketItem { BasketId = basket.Id, ArticleId = request.ArticleId, Quantity = request.Quantity };
 await _basketService.AddItemAsync(item);
 return CreatedAtAction(nameof(GetMyBasket), null);
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