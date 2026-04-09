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
        // V entity frameworku funguje Change Tracking takže to nesíme dělat takto přes .Entity, ale 
        // jakmile udělám SaveChangesAsync tak se mi aktualizuje hodnota v TaskEntity task
        var retD = dbContext.Tasks.Add(task).Entity;

        await dbContext.Tasks.AddAsync(task, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        // zde už má task.Id hodnotu z DB

        // Toto smaže všechny úkoly z cache
        await cache.RemoveByTagAsync(TaskCacheTag, cancellationToken);

        // Toto přidá úkol do cache
        await cache.SetAsync($"task:{task.Id}", task, tags: [TaskCacheTag], cancellationToken: cancellationToken);

        logger.LogInformation("Byl VYTVOŘEN nový úkol {TaskId}", retD.Id);
        return CreatedAtAction(nameof(GetById), new { id = retD.Id }, retD);
        //return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<TaskEntity>> Update([FromBody] TaskEntity task, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == task.Id, cancellationToken);
        if (existing is null) return NotFound();

        dbContext.Entry(existing).CurrentValues.SetValues(task);
        await dbContext.SaveChangesAsync(cancellationToken);
        await cache.RemoveByTagAsync(TaskCacheTag, cancellationToken);
        logger.LogInformation("Byl UPRAVEN úkol číslo: {TaskId}", existing.Id);
        return existing;
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<TaskEntity>> GetById(int id, CancellationToken cancellationToken)
    {
        var entity = await cache.GetOrCreateAsync(
            $"task:{id}",
            async cancel =>
            {
                // AsNoTracking je důležité, tímto vypneme change tracking a query bude rychlejší
                return await dbContext.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancel);
            },
            tags: [TaskCacheTag],
            cancellationToken: cancellationToken);
        if (entity is null) return NotFound();
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
        var entity = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return NotFound();
        dbContext.Tasks.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Opět dojde k smazání všech úkolů z cache
        await cache.RemoveByTagAsync(TaskCacheTag, cancellationToken);
        // správně chceme smazat pouze jeden úkol
        await cache.RemoveAsync($"task:{id}", cancellationToken);

        logger.LogInformation("Byl SMAZÁN úkol číslo: {TaskId}", id);
        
        // Tady vracíme 204 což by ale mohlo znamenat, že API nezná takový úkol, lepší si myslím že by bylo Ok();
        return NoContent();
    }
}


