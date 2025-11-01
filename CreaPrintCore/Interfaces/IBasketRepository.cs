using System.Collections.Generic;
using System.Threading.Tasks;
using CreaPrintCore.Models;

namespace CreaPrintCore.Interfaces
{
 public interface IBasketRepository
 {
 Task<Basket?> GetByUserIdAsync(int userId);
 Task<Basket> CreateAsync(Basket basket);
 Task UpdateAsync(Basket basket);
 Task AddItemAsync(BasketItem item);
 Task RemoveItemAsync(int itemId);
 Task<BasketItem?> GetItemByIdAsync(int itemId);
 Task<BasketItem?> GetItemByBasketAndArticleAsync(int basketId, int articleId);
 Task UpdateItemAsync(BasketItem item);
 }
}
