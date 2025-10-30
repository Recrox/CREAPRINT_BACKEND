using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using CreaPrintCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintDatabase.Repositories;

public class ArticleRepository : GenericRepository<Article>, IArticleRepository
{
    private readonly CreaPrintDbContext _dbContext;
    public ArticleRepository(CreaPrintDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public override async Task<IEnumerable<Article>> GetAllAsync()
    {
        return await _dbContext.Articles.Include(a => a.Category).ToListAsync();
    }

    public override async Task<IEnumerable<Article>> GetPagedAsync(int page, int pageSize)
    {
        return await _dbContext.Articles.Include(a => a.Category)
            .Skip(page * pageSize).Take(pageSize).ToListAsync();
    }

    public override async Task<Article?> GetByIdAsync(int id)
    {
        return await _dbContext.Articles.Include(a => a.Category)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    //public async Task<int> GetCountAsync(Func<Article, bool>? filter = null)
    //{
    //    if (filter == null)
    //        return await _dbContext.Articles.CountAsync();
    //    else
    //        return await Task.FromResult(_dbContext.Articles.Count(filter));
    //}
}
