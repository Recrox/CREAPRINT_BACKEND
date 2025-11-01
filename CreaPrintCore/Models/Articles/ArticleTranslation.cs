using CreaPrintCore.Models.Base;

namespace CreaPrintCore.Models.Articles
{
 public class ArticleTranslation : AuditableEntity
 {
 public int ArticleId { get; set; }
 public Article? Article { get; set; }

 // IETF language tag or simple code like "en", "fr"
 public required string Language { get; set; }

 public required string Title { get; set; }
 public required string Content { get; set; }

 // optional flag to mark default translation
 public bool IsDefault { get; set; } = false;
 }
}
