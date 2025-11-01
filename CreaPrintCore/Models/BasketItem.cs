using System.Text.Json.Serialization;

namespace CreaPrintCore.Models;

public class BasketItem : AuditableEntity
{
public int BasketId { get; set; }
[JsonIgnore]
public Basket? Basket { get; set; }

public int ArticleId { get; set; }
public Article? Article { get; set; }

public int Quantity { get; set; }
}
