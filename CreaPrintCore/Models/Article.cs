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
}
