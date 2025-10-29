using Microsoft.AspNetCore.Mvc;
using CreaPrintCore.Interfaces;
using AutoMapper;
using CoreArticle = CreaPrintCore.Models.Article;
using ApiArticle = CreaPrintApi.Dtos.Article;

namespace CreaPrintApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _service;
    private readonly IMapper _mapper;

    public ArticlesController(IArticleService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApiArticle>>> GetAll()
    {
        var articles = await _service.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<ApiArticle>>(articles));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiArticle>> GetById(int id)
    {
        var article = await _service.GetByIdAsync(id);
        if (article == null) return NotFound();
        return Ok(_mapper.Map<ApiArticle>(article));
    }

    [HttpPost]
    public async Task<ActionResult<ApiArticle>> Create(CoreArticle article)
    {
        var created = await _service.CreateAsync(article);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<ApiArticle>(created));
    }

    [HttpGet("paged")]
    public async Task<ActionResult<IEnumerable<ApiArticle>>> GetPaged([FromQuery] int page = 0, [FromQuery] int pageSize = 10)
    {
        var articles = await _service.GetPagedAsync(page, pageSize);
        return Ok(_mapper.Map<IEnumerable<ApiArticle>>(articles));
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount()
    {
        return Ok(await _service.GetCountAsync());
    }
}
