using CreaPrintCore.Interfaces;
using CreaPrintCore.Models.Orders;
using CreaPrintCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintDatabase.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
 private readonly CreaPrintDbContext _dbContext;
 public OrderRepository(CreaPrintDbContext context) : base(context)
 {
 _dbContext = context;
 }

 public async Task<Order> CreateAsync(Order order)
 {
 _dbContext.Set<Order>().Add(order);
 await _dbContext.SaveChangesAsync();
 return order;
 }

 public async Task<Order?> GetByIdAsync(int id)
 {
 return await _dbContext.Set<Order>().Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
 }

 public async Task<IEnumerable<Order>> GetByUserAsync(int userId)
 {
 return await _dbContext.Set<Order>().Include(o => o.Items).Where(o => o.UserId == userId).ToListAsync();
 }
}
