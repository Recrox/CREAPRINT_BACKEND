using System.Threading.Tasks;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;

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
 await _repository.AddItemAsync(item);
 }

 public async Task RemoveItemAsync(int itemId)
 {
 await _repository.RemoveItemAsync(itemId);
 }
 }
}
