using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintDatabase.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly CreaPrintDbContext _context;

        public ArticleRepository(CreaPrintDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public async Task<Article?> GetByIdAsync(int id)
        {
            return await _context.Articles.FindAsync(id);
        }

        public async Task<Article> CreateAsync(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }
    }
}
