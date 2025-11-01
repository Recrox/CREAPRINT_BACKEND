using System.Collections.Generic;
using System.Threading.Tasks;
using CreaPrintCore.Models.Orders;

namespace CreaPrintCore.Interfaces
{
 public interface IOrderRepository
 {
 Task<Order> CreateAsync(Order order);
 Task<Order?> GetByIdAsync(int id);
 Task<IEnumerable<Order>> GetByUserAsync(int userId);
 }
}
