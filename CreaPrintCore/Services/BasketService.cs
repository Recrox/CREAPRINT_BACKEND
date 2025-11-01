using System.Threading.Tasks;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using System.Linq;

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

 public async Task<Basket> CreateAsync(Basket basket)
 {
 return await _repository.CreateAsync(basket);
 }

 public async Task AddItemAsync(BasketItem item)
 {
 // Check article exists and has enough stock
 var article = await _articleRepository.GetByIdAsync(item.ArticleId);
 if (article == null) throw new KeyNotFoundException("Article not found");
 if (article.Stock < item.Quantity) throw new InvalidOperationException("Insufficient stock");

 // Decrease stock
 article.Stock -= item.Quantity;
 await _articleRepository.UpdateAsync(article);

 await _repository.AddItemAsync(item);
 }

 public async Task RemoveItemAsync(int itemId)
 {
 // Retrieve item with article to restore stock
 var item = await _repository.GetItemByIdAsync(itemId);
 if (item == null) return;

 // restore stock
 if (item.Article != null)
 {
 item.Article.Stock += item.Quantity;
 await _articleRepository.UpdateAsync(item.Article);
 }

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
