using System.Threading.Tasks;
using CreaPrintCore.Models;

namespace CreaPrintCore.Interfaces
{
 public interface IBasketService
 {
 Task<Basket?> GetByUserIdAsync(int userId);
 Task<Basket> CreateAsync(Basket basket);
 Task AddItemAsync(BasketItem item);
 Task RemoveItemAsync(int itemId);
 }
}
