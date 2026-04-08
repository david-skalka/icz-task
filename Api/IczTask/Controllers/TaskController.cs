using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using TaskEntity = IczTask.Models.Task;

namespace IczTask.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController(
    ApplicationDbContext dbContext,
    HybridCache cache,
    ILogger<TaskController> logger) : ControllerBase
{
    private const string TaskCacheTag = "tasks";

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TaskEntity>> Create([FromBody] TaskEntity task, CancellationToken cancellationToken)
    {
        var retD = dbContext.Tasks.Add(task).Entity;
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveByTagAsync(TaskCacheTag, cancellationToken);
        logger.LogInformation("Byl VYTVOŘEN nový úkol {TaskId}", retD.Id);
        return retD;
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<TaskEntity>> Update([FromBody] TaskEntity task, CancellationToken cancellationToken)
    {
        var retD = dbContext.Tasks.Update(task).Entity;
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveByTagAsync(TaskCacheTag, cancellationToken);
        logger.LogInformation("Byl UPRAVEN úkol číslo: {TaskId}", retD.Id);
        return retD;
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<TaskEntity>> GetById(int id, CancellationToken cancellationToken)
    {
        var entity = await cache.GetOrCreateAsync(
            $"task:{id}",
            async cancel => await dbContext.Tasks.SingleAsync(x => x.Id == id, cancel),
            tags: [TaskCacheTag],
            cancellationToken: cancellationToken);
        return entity;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<TaskEntity>>> GetAll(
        [FromQuery] string? name,
        CancellationToken cancellationToken)
    {
        var filterKey = name ?? string.Empty;
        var list = await cache.GetOrCreateAsync(
            $"tasks:list:{filterKey}",
            async cancel => await dbContext.Tasks
                .Where(t => string.IsNullOrEmpty(name) || t.Name.Contains(name))
                .ToListAsync(cancel),
            tags: [TaskCacheTag],
            cancellationToken: cancellationToken);
        return list;
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Tasks.SingleAsync(x => x.Id == id, cancellationToken);
        dbContext.Tasks.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveByTagAsync(TaskCacheTag, cancellationToken);
        logger.LogInformation("Byl SMAZÁN úkol číslo: {TaskId}", id);
        return NoContent();
    }
}


