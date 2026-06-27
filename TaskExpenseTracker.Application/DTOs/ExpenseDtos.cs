namespace TaskExpenseTracker.Application.DTOs;

public record CreateExpenseDto(
    string Description,
    decimal Amount,
    string Category,
    DateTime Date
);

public record UpdateExpenseDto(
    string Description,
    decimal Amount,
    string Category,
    DateTime Date
);

public record ExpenseResponseDto(
    int Id,
    string Description,
    decimal Amount,
    string Category,
    DateTime Date,
    DateTime CreatedAt
);
