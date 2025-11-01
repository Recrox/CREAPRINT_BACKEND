using CreaPrintCore.Models.Base;
using CreaPrintCore.Models.Users;
using System.Text.Json.Serialization;

namespace CreaPrintCore.Models.Baskets;

public class Basket : AuditableEntity
{
public int UserId { get; set; }
[JsonIgnore]
public User? User { get; set; }
public ICollection<BasketItem>? Items { get; set; }
}