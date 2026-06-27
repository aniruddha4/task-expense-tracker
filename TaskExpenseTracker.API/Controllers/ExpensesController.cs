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
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExpensesController(AppDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // GET api/expenses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseResponseDto>>> GetAll()
    {
        var expenses = await _db.Expenses
            .Where(e => e.UserId == UserId)
            .OrderByDescending(e => e.Date)
            .Select(e => new ExpenseResponseDto(
                e.Id, e.Description, e.Amount,
                e.Category, e.Date, e.CreatedAt))
            .ToListAsync();

        return Ok(expenses);
    }

    // GET api/expenses/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseResponseDto>> GetById(int id)
    {
        var expense = await _db.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == UserId);
        if (expense is null) return NotFound();

        return Ok(new ExpenseResponseDto(
            expense.Id, expense.Description, expense.Amount,
            expense.Category, expense.Date, expense.CreatedAt));
    }

    // POST api/expenses
    [HttpPost]
    public async Task<ActionResult<ExpenseResponseDto>> Create(CreateExpenseDto dto)
    {
        var expense = new Expense
        {
            Description = dto.Description,
            Amount      = dto.Amount,
            Category    = dto.Category,
            Date        = dto.Date,
            UserId      = UserId
        };

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        var response = new ExpenseResponseDto(
            expense.Id, expense.Description, expense.Amount,
            expense.Category, expense.Date, expense.CreatedAt);

        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, response);
    }

    // PUT api/expenses/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateExpenseDto dto)
    {
        var expense = await _db.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == UserId);
        if (expense is null) return NotFound();

        expense.Description = dto.Description;
        expense.Amount      = dto.Amount;
        expense.Category    = dto.Category;
        expense.Date        = dto.Date;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/expenses/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await _db.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == UserId);
        if (expense is null) return NotFound();

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}