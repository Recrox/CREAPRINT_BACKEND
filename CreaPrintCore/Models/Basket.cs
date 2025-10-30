using System.Text.Json.Serialization;

namespace CreaPrintCore.Models
{
 public class Basket : AuditableEntity
 {
 public int UserId { get; set; }
 [JsonIgnore]
 public User? User { get; set; }
 public ICollection<BasketItem>? Items { get; set; }
 }

 public class BasketItem : AuditableEntity
 {
 public int BasketId { get; set; }
 [JsonIgnore]
 public Basket? Basket { get; set; }

 public int ArticleId { get; set; }
 public Article? Article { get; set; }

 public int Quantity { get; set; }
 }
}
