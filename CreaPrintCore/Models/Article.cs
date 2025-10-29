namespace CreaPrintCore.Models
{
 public class Article : Item
 {
 public required string Title { get; set; }
 public required string Content { get; set; }
 public required string Category { get; set; }
 public required decimal Price { get; set; } // Ajout du prix
 }
}
