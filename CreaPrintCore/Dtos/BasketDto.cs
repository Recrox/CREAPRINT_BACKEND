using System.Collections.Generic;

namespace CreaPrintCore.Dtos
{
 public class BasketDto
 {
 public int Id { get; set; }
 public IEnumerable<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();
 }

 public class BasketItemDto
 {
 public int Id { get; set; }
 public int ArticleId { get; set; }
 public int Quantity { get; set; }
 public ArticleDto? Article { get; set; }
 }

 public class ArticleDto
 {
 public int Id { get; set; }
 public string Title { get; set; } = string.Empty;
 public decimal Price { get; set; }
 public IEnumerable<ArticleImageDto> Images { get; set; } = new List<ArticleImageDto>();
 }

 public class ArticleImageDto
 {
 public int Id { get; set; }
 public string? Url { get; set; }
 public bool IsPrimary { get; set; }
 }
}
