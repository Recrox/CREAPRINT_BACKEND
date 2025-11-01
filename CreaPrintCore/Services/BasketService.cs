using System;
using System.Threading.Tasks;
using System.Collections.Generic;
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
 private readonly IArticleRepository _articleRepository;

 public BasketService(IBasketRepository repository, IArticleRepository articleRepository)
 {
 _repository = repository;
 _articleRepository = articleRepository;
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

 // new: add item for a user's basket with full flow (create basket if needed)
 public async Task AddItemToUserBasketAsync(int userId, int articleId, int quantity)
 {
 if (quantity <=0) throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

 var basket = await _repository.GetByUserIdAsync(userId);
 if (basket == null)
 {
 basket = new Basket { UserId = userId };
 basket = await _repository.CreateAsync(basket);
 }

 // optional: validate article exists and stock
 var article = await _articleRepository.GetByIdAsync(articleId);
 if (article == null) throw new InvalidOperationException("Article not found");

 var existing = await _repository.GetItemByBasketAndArticleAsync(basket.Id, articleId);
 if (existing != null)
 {
 existing.Quantity += quantity;
 await _repository.UpdateItemAsync(existing);
 }
 else
 {
 var item = new BasketItem { BasketId = basket.Id, ArticleId = articleId, Quantity = quantity };
 await _repository.AddItemAsync(item);
 }
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
 return basket.Items?.Sum(i => (i.Article?.Price ??0m) * i.Quantity) ??0m;
 }
 }
}
