using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using CreaPrintCore.Models.Baskets;
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
 // Include items, the related Article and the Article images so higher-level services can access them without additional queries
 return await _dbContext.Set<Basket>()
 .Include(b => b.Items)
 .ThenInclude(i => i.Article)
 .ThenInclude(a => a.Images)
 .FirstOrDefaultAsync(b => b.UserId == userId);
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

 public async Task<BasketItem?> GetItemByIdAsync(int itemId)
 {
 return await _dbContext.Set<BasketItem>().Include(i => i.Article).ThenInclude(a => a.Images).FirstOrDefaultAsync(i => i.Id == itemId);
 }

 public async Task<BasketItem?> GetItemByBasketAndArticleAsync(int basketId, int articleId)
 {
 return await _dbContext.Set<BasketItem>().Include(i => i.Article).ThenInclude(a => a.Images).FirstOrDefaultAsync(i => i.BasketId == basketId && i.ArticleId == articleId);
 }

 public async Task UpdateItemAsync(BasketItem item)
 {
 _dbContext.Set<BasketItem>().Update(item);
 await _dbContext.SaveChangesAsync();
 }
}
