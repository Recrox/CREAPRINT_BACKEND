namespace CreaPrintCore.Models
{
 public class Article : AuditableEntity
 {
 public required string Title { get; set; }
 public required string Content { get; set; }
 public required decimal Price { get; set; }
 public int? CategoryId { get; set; }
 public Category? Category { get; set; }
 }
}
