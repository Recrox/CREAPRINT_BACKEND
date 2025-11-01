using CreaPrintCore.Models.Articles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreaPrintCore.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetAllAsync();
        Task<Article?> GetByIdAsync(int id);
        Task<Article> CreateAsync(Article article);
        Task<IEnumerable<Article>> GetPagedAsync(int page, int pageSize);
        Task<int> GetCountAsync(Func<Article, bool>? filter = null);
        Task<Article?> UpdateAsync(int id, Article article);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Article>> GetByTitleAsync(string title);
    }
}
