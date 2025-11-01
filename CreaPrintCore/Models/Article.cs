using Newtonsoft.Json;

namespace CreaPrintCore.Models;

public class Article : AuditableEntity
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required decimal Price { get; set; }

    // CategoryId can be null for uncategorized articles
    public int? CategoryId { get; set; }

    [JsonIgnore]
    public Category? Category { get; set; }

    // Available stock quantity for the article
    public int Stock { get; set; } = 0;

    // Images for the article
    public ICollection<ArticleImage>? Images { get; set; }
}
