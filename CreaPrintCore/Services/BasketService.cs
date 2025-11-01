using System.Threading.Tasks;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using System.Linq;

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

 public async Task<Basket> CreateAsync(Basket basket)
 {
 return await _repository.CreateAsync(basket);
 }

 public async Task AddItemAsync(BasketItem item)
 {
 // Do not modify Article.Stock here. Stock will be reserved/consumed when creating the order.
 await _repository.AddItemAsync(item);
 }

 public async Task RemoveItemAsync(int itemId)
 {
 // Simply remove the item from the basket. Stock restoration happens when order is cancelled/modified if needed.
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
