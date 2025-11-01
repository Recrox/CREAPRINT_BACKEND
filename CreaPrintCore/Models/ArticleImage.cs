namespace CreaPrintCore.Models;

public class ArticleImage : AuditableEntity
{
 public int ArticleId { get; set; }
 public Article? Article { get; set; }

 // URL to the image (can be remote or a path served by the app)
 public string? Url { get; set; }

 // Optional raw image bytes (if you want to store the image in DB)
 public byte[]? Data { get; set; }
 public string? MimeType { get; set; }

 // mark primary image
 public bool IsPrimary { get; set; } = false;
}
