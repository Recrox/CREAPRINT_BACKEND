using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreaPrintCore.Interfaces
{
 public interface IArticleService
 {
 Task<IEnumerable<Models.Article>> GetAllAsync();
 Task<Models.Article?> GetByIdAsync(int id);
 Task<Models.Article> CreateAsync(Models.Article article);
 }
}
