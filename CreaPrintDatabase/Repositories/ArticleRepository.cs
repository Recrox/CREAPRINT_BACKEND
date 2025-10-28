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
    // Ajoute ici des m�thodes sp�cifiques � Article si besoin
}
