using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskEntity = IczTask.Models.Task;

namespace IczTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TaskEntity>> Create([FromBody] TaskEntity task, CancellationToken cancellationToken)
    {
        var retD = dbContext.Tasks.Add(task).Entity;
        await dbContext.SaveChangesAsync(cancellationToken);
        return retD;
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TaskEntity>> Update([FromBody] TaskEntity task, CancellationToken cancellationToken)
    {
        var retD = dbContext.Tasks.Update(task).Entity;
        await dbContext.SaveChangesAsync(cancellationToken);
        return retD;
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TaskEntity>> GetById(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Tasks.SingleAsync(x => x.Id == id, cancellationToken);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<TaskEntity>>> GetAll(CancellationToken cancellationToken)
    {
        return await dbContext.Tasks.ToListAsync(cancellationToken);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Tasks.SingleAsync(x => x.Id == id, cancellationToken);
        dbContext.Tasks.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
