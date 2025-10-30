using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using CreaPrintCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintDatabase.Repositories;

public class BasketRepository : BaseRepository<Basket>, IBasketRepository
{
 private readonly CreaPrintDbContext _dbContext;
 public BasketRepository(CreaPrintDbContext context) : base(context)
 {
 _dbContext = context;
 }

 public async Task<Basket?> GetByUserIdAsync(int userId)
 {
 return await _dbContext.Set<Basket>().Include(b => b.Items).ThenInclude(i => i.Article).FirstOrDefaultAsync(b => b.UserId == userId);
 }

 public async Task<Basket> CreateAsync(Basket basket)
 {
 return await base.CreateAsync(basket);
 }

 public async Task UpdateAsync(Basket basket)
 {
 await base.UpdateAsync(basket);
 }

 public async Task AddItemAsync(BasketItem item)
 {
 _dbContext.Set<BasketItem>().Add(item);
 await _dbContext.SaveChangesAsync();
 }

 public async Task RemoveItemAsync(int itemId)
 {
 var item = await _dbContext.Set<BasketItem>().FindAsync(itemId);
 if (item != null)
 {
 _dbContext.Set<BasketItem>().Remove(item);
 await _dbContext.SaveChangesAsync();
 }
 }
}
