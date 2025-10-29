using System.Collections.Generic;
using System.Threading.Tasks;
using CreaPrintCore.Models;

namespace CreaPrintCore.Interfaces
{
 public interface IArticleRepository
 {
 Task<IEnumerable<Article>> GetAllAsync();
 Task<Article?> GetByIdAsync(int id);
 Task<Article> CreateAsync(Article article);
 Task<IEnumerable<Article>> GetPagedAsync(int page, int pageSize);
 Task<int> GetCountAsync(Func<Article, bool>? filter = null);
 }
}
