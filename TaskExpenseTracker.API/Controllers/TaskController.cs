using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskExpenseTracker.Application.DTOs;
using TaskExpenseTracker.Domain.Entities;
using TaskExpenseTracker.Infrastructure.Data;

namespace TaskExpenseTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksController(AppDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // GET api/tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAll()
    {
        var tasks = await _db.Tasks
            .Where(t => t.UserId == UserId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskResponseDto(
                t.Id, t.Title, t.Description,
                t.IsCompleted, t.DueDate, t.CreatedAt))
            .ToListAsync();

        return Ok(tasks);
    }

    // GET api/tasks/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(int id)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (task is null) return NotFound();

        return Ok(new TaskResponseDto(
            task.Id, task.Title, task.Description,
            task.IsCompleted, task.DueDate, task.CreatedAt));
    }

    // POST api/tasks
    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> Create(CreateTaskDto dto)
    {
        var task = new AppTask
        {
            Title       = dto.Title,
            Description = dto.Description,
            DueDate     = dto.DueDate,
            UserId      = UserId
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        var response = new TaskResponseDto(
            task.Id, task.Title, task.Description,
            task.IsCompleted, task.DueDate, task.CreatedAt);

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, response);
    }

    // PUT api/tasks/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTaskDto dto)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (task is null) return NotFound();

        task.Title       = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;
        task.DueDate     = dto.DueDate;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/tasks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (task is null) return NotFound();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}