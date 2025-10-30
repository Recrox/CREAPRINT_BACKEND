using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Models;
using CreaPrintCore.Services;
using Microsoft.AspNetCore.Authorization;

namespace CreaPrintApi.Controllers;

[Route("api/[controller]")]
public class ArticlesController : BaseController
{
    private readonly IArticleService _service;

    public ArticlesController(IArticleService service, CurrentUser currentUser) : base(currentUser)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Article>>> GetAll()
    {
        // example access to CurrentUser in controller

        var me = CurrentUser; // may be null if anonymous

        var articles = await _service.GetAllAsync();
        return Ok(articles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Article>> GetById(int id)
    {
        var article = await _service.GetByIdAsync(id);
        if (article == null) return NotFound();
        return Ok(article);
    }

    // Search by title
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Article>>> Search([FromQuery] string title)
    {
        var results = await _service.GetByTitleAsync(title);
        return Ok(results);
    }

    [HttpPost]
    public async Task<ActionResult<Article>> Create(Article article)
    {
        var created = await _service.CreateAsync(article);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Article article)
    {
        if (id != article.Id) return BadRequest();
        var updated = await _service.UpdateAsync(id, article);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Article> patch)
    {
        if (patch == null) return BadRequest();
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();

        // apply patch to the tracked entity
        patch.ApplyTo(existing, ModelState);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var updated = await _service.UpdateAsync(id, existing);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("paged")]
    //[Authorize("AdminOnly")]
    public async Task<ActionResult<IEnumerable<Article>>> GetPaged([FromQuery] int page = 0, [FromQuery] int pageSize = 10)
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
