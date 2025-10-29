namespace CreaPrintApi.Dtos
{
 public class Article
 {
 public int Id { get; set; }
 public string Title { get; set; } = string.Empty;
 public string Content { get; set; } = string.Empty;
 public decimal Price { get; set; }
 public int? CategoryId { get; set; }
 public Category? Category { get; set; }
 }
}
