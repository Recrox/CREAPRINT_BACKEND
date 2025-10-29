namespace CreaPrintCore.Models
{
 public class Category : AuditableEntity
 {
 public required string Name { get; set; }
 public ICollection<Article>? Articles { get; set; }
 }
}
