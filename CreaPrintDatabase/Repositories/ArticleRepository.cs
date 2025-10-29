using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using CreaPrintCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintDatabase.Repositories;

public class ArticleRepository : GenericRepository<Article>, IArticleRepository
{
    public ArticleRepository(CreaPrintDbContext context) : base(context)
    {
    }
    // Ajoute ici des méthodes spécifiques à Article si besoin
    public async Task<IEnumerable<Article>> GetPagedAsync(int page, int pageSize)
    {
        return await base.GetPagedAsync(page, pageSize);
    }
    public async Task<int> GetCountAsync(Func<Article, bool>? filter = null)
{
    return await base.GetCountAsync(filter);
}
}
