using System.Threading.Tasks;
using System.Collections.Generic;
using CreaPrintCore.Models.Orders;

namespace CreaPrintCore.Interfaces
{
 public interface IOrderService
 {
 Task<Order?> CreateFromBasketAsync(int userId);
 Task<IEnumerable<Order>> GetByUserAsync(int userId);
 Task<Order?> GetByIdAsync(int orderId);
 }
}
