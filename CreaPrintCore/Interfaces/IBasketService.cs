using System.Threading.Tasks;
using CreaPrintCore.Models;
using CreaPrintCore.Models.Baskets;
using CreaPrintCore.Dtos;

namespace CreaPrintCore.Interfaces
{
 public interface IBasketService
 {
 Task<Basket?> GetByUserIdAsync(int userId);
 Task<BasketDto?> GetDtoByUserIdAsync(int userId);
 Task<Basket> CreateAsync(Basket basket);
 Task AddItemAsync(BasketItem item);
 Task RemoveItemAsync(int itemId);
 Task<decimal> GetTotalByUserIdAsync(int userId);
 }
}
