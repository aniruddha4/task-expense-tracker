namespace TaskExpenseTracker.Application.DTOs;

public record CreateTaskDto(
    string Title,
    string? Description,
    DateTime DueDate
);

public record UpdateTaskDto(
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime DueDate
);

public record TaskResponseDto(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime DueDate,
    DateTime CreatedAt
);
