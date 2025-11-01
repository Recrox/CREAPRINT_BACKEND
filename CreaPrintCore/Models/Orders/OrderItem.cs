using CreaPrintCore.Models.Base;

namespace CreaPrintCore.Models.Orders
{
 public class OrderItem : AuditableEntity
 {
 public int OrderId { get; set; }
 public Order? Order { get; set; }

 // snapshot of article at time of order
 public int ArticleId { get; set; }
 public string TitleSnapshot { get; set; } = string.Empty;
 public decimal PriceSnapshot { get; set; }
 public int Quantity { get; set; }
 }
}
