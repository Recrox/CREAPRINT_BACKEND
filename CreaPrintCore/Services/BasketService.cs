using System.Threading.Tasks;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using System.Linq;
using CreaPrintCore.Models.Baskets;
using CreaPrintCore.Dtos;

namespace CreaPrintCore.Services
{
 public class BasketService : IBasketService
 {
 private readonly IBasketRepository _repository;

 public BasketService(IBasketRepository repository)
 {
 _repository = repository;
 }

 public async Task<Basket?> GetByUserIdAsync(int userId)
 {
 return await _repository.GetByUserIdAsync(userId);
 }

 public async Task<BasketDto?> GetDtoByUserIdAsync(int userId)
 {
 var basket = await _repository.GetByUserIdAsync(userId);
 if (basket == null) return null;

 var dto = new BasketDto
 {
 Id = basket.Id,
 Items = basket.Items?.Select(i => new BasketItemDto
 {
 Id = i.Id,
 ArticleId = i.ArticleId,
 Quantity = i.Quantity,
 Article = i.Article == null ? null : new ArticleDto
 {
 Id = i.Article.Id,
 Title = i.Article.Title,
 Price = i.Article.Price,
 Images = i.Article.Images?.Select(img => new ArticleImageDto { Id = img.Id, Url = img.Url, IsPrimary = img.IsPrimary }) ?? Enumerable.Empty<ArticleImageDto>()
 }
 }) ?? Enumerable.Empty<BasketItemDto>()
 };

 return dto;
 }

 public async Task<Basket> CreateAsync(Basket basket)
 {
 return await _repository.CreateAsync(basket);
 }

 public async Task AddItemAsync(BasketItem item)
 {
 // If item for same basket+article exists, increment quantity
 var existing = await _repository.GetItemByBasketAndArticleAsync(item.BasketId, item.ArticleId);
 if (existing != null)
 {
 existing.Quantity += item.Quantity;
 await _repository.UpdateItemAsync(existing);
 return;
 }

 await _repository.AddItemAsync(item);
 }

 public async Task RemoveItemAsync(int itemId)
 {
 // Simply remove the item from the basket.
 await _repository.RemoveItemAsync(itemId);
 }

 public async Task<decimal> GetTotalByUserIdAsync(int userId)
 {
 var basket = await _repository.GetByUserIdAsync(userId);
 if (basket == null) return 0m;
 // Sum quantity * price, handle missing Article or Price
 return basket.Items?.Sum(i => (i.Article?.Price ?? 0m) * i.Quantity) ?? 0m;
 }
 }
}
