using CreaPrintCore.Models.Base;

namespace CreaPrintCore.Models.Orders;
public class Order : AuditableEntity
{
public int UserId { get; set; }
public OrderStatus Status { get; set; } = OrderStatus.Pending;
public decimal Total { get; set; }
public ICollection<OrderItem>? Items { get; set; }
}
