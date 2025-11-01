using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models.Orders;
using CreaPrintCore.Models.Baskets;
using CreaPrintCore.Models.Articles;
using CreaPrintCore.Interfaces;

namespace CreaPrintCore.Services
{
 public class OrderService : IOrderService
 {
 private readonly IBasketRepository _basketRepo;
 private readonly IArticleRepository _articleRepo;
 private readonly IOrderRepository _orderRepo;

 public OrderService(IBasketRepository basketRepo, IArticleRepository articleRepo, IOrderRepository orderRepo)
 {
 _basketRepo = basketRepo;
 _articleRepo = articleRepo;
 _orderRepo = orderRepo;
 }

 public async Task<Order?> CreateFromBasketAsync(int userId)
 {
 var basket = await _basketRepo.GetByUserIdAsync(userId);
 if (basket == null || basket.Items == null || !basket.Items.Any()) return null;

 // Build order snapshot
 var order = new Order { UserId = userId, Status = OrderStatus.Pending };
 order.Items = new List<OrderItem>();
 decimal total =0m;
 foreach (var item in basket.Items)
 {
 var art = item.Article;
 var price = art?.Price ??0m;
 var oi = new OrderItem { ArticleId = item.ArticleId, TitleSnapshot = art?.Title ?? "", PriceSnapshot = price, Quantity = item.Quantity };
 order.Items.Add(oi);
 total += price * item.Quantity;
 // decrement stock if available
 if (art != null)
 {
 art.Stock = Math.Max(0, art.Stock - item.Quantity);
 await _articleRepo.UpdateAsync(art);
 }
 }
 order.Total = total;

 var created = await _orderRepo.CreateAsync(order);
 // clear basket
 foreach (var item in basket.Items.ToList())
 {
 await _basketRepo.RemoveItemAsync(item.Id);
 }
 return created;
 }

 public async Task<IEnumerable<Order>> GetByUserAsync(int userId)
 {
 return await _orderRepo.GetByUserAsync(userId);
 }

 public async Task<Order?> GetByIdAsync(int orderId)
 {
 return await _orderRepo.GetByIdAsync(orderId);
 }
 }
}
