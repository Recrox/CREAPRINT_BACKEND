using CreaPrintCore.Models.Base;
using System.Text.Json.Serialization;

namespace CreaPrintCore.Models.Articles
{
    public class Category : AuditableEntity
    {
        public required string Name { get; set; }
        [JsonIgnore]
        public ICollection<Article>? Articles { get; set; }
    }
}
