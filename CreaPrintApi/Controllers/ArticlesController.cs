using Microsoft.AspNetCore.Mvc;
using CreaPrintCore.Interfaces;
using CoreArticle = CreaPrintCore.Models.Article;

namespace CreaPrintApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _service;

    public ArticlesController(IArticleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoreArticle>>> GetAll()
    {
        var articles = await _service.GetAllAsync();
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoreArticle>> GetById(int id)
    {
        var article = await _service.GetByIdAsync(id);
        if (article == null) return NotFound();
        return Ok(article);
    }

    [HttpPost]
    public async Task<ActionResult<CoreArticle>> Create(CoreArticle article)
    {
        var created = await _service.CreateAsync(article);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<IEnumerable<CoreArticle>>> GetPaged([FromQuery] int page = 0, [FromQuery] int pageSize = 10)
    {
        var articles = await _service.GetPagedAsync(page, pageSize);
        return Ok(articles);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount()
    {
        return Ok(await _service.GetCountAsync());
    }
}
