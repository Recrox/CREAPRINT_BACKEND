using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreaPrintCore.Interfaces
{
 public interface IArticleService
 {
 Task<IEnumerable<Models.Article>> GetAllAsync();
 Task<Models.Article?> GetByIdAsync(int id);
 Task<Models.Article> CreateAsync(Models.Article article);
 Task<IEnumerable<Models.Article>> GetPagedAsync(int page, int pageSize);
 Task<int> GetCountAsync(Func<Models.Article, bool>? filter = null);
 Task<bool> UpdateAsync(int id, Models.Article article);
 Task DeleteAsync(int id);
 }
}
