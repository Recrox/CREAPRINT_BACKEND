using System.Text.Json.Serialization;

namespace CreaPrintCore.Models;

public class Basket : AuditableEntity
{
public int UserId { get; set; }
[JsonIgnore]
public User? User { get; set; }
public ICollection<BasketItem>? Items { get; set; }
}