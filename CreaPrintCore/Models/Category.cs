using System.Text.Json.Serialization;

namespace CreaPrintCore.Models
{
 public class Category : AuditableEntity
 {
 public required string Name { get; set; }
        //[JsonIgnore]
        public ICollection<Article>? Articles { get; set; }
 }
}
