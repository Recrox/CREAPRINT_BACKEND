using CreaPrintCore.Interfaces;
using CreaPrintCore.Models.Articles;
using CreaPrintCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintDatabase.Repositories;

public class ArticleRepository : BaseRepository<Article>, IArticleRepository
{
    private readonly CreaPrintDbContext _dbContext;
    public ArticleRepository(CreaPrintDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public override async Task<IEnumerable<Article>> GetAllAsync()
    {
        return await _dbContext.Articles.Include(a => a.Category).Include(a => a.Images).Include(a => a.Translations).ToListAsync();
    }

    public override async Task<IEnumerable<Article>> GetPagedAsync(int page, int pageSize)
    {
        return await _dbContext.Articles.Include(a => a.Category).Include(a => a.Images).Include(a => a.Translations)
            .Skip(page * pageSize).Take(pageSize).ToListAsync();
    }

    public override async Task<Article?> GetByIdAsync(int id)
    {
        return await _dbContext.Articles.Include(a => a.Category).Include(a => a.Images).Include(a => a.Translations)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    // helper: search by title using base FindAsync
    public async Task<IEnumerable<Article>> GetByTitleAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return Enumerable.Empty<Article>();
        var normalized = title.Trim();
        return await FindAsync(a => EF.Functions.Like(a.Title, $"%{normalized}%"));
    }
}
