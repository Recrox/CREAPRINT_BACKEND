using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models.Articles;

namespace CreaPrintCore.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _repository;

    public ArticleService(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Article>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Article?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Article> CreateAsync(Article article)
    {
        article.CreatedOn = DateTime.UtcNow;
        return await _repository.CreateAsync(article);
    }

    public async Task<Article?> UpdateAsync(int id, Article article)
    {
        if (id != article.Id) return null;

        var updated = await _repository.UpdateAndGetAsync(article);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id) => await _repository.DeleteAsync(id);

    public async Task<IEnumerable<Article>> GetPagedAsync(int page, int pageSize)
    {
        return await _repository.GetPagedAsync(page, pageSize);
    }

    public async Task<int> GetCountAsync(Func<Article, bool>? filter = null)
    {
        return await _repository.GetCountAsync(filter);
    }

    public async Task<IEnumerable<Article>> GetByTitleAsync(string title)
    {
        return await _repository.GetByTitleAsync(title);
    }

    // Convenience: get article localized by language code (falls back to default translation or base fields)
    public async Task<(Article? Article, string Title, string Content)> GetLocalizedByIdAsync(int id, string language)
    {
        var art = await _repository.GetByIdAsync(id);
        if (art == null) return (null, string.Empty, string.Empty);

        var tr = art.Translations?.FirstOrDefault(t => t.Language.Equals(language, StringComparison.OrdinalIgnoreCase))
         ?? art.Translations?.FirstOrDefault(t => t.IsDefault)
         ?? art.Translations?.FirstOrDefault();

        var title = tr?.Title ?? art.Title;
        var content = tr?.Content ?? art.Content;
        return (art, title, content);
    }
}
