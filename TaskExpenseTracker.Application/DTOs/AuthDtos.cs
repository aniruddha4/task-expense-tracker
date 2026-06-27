namespace TaskExpenseTracker.Application.DTOs;

public record RegisterDto(
    string Email,
    string Password
);

public record LoginDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    string Token,
    string Email,
    DateTime Expires
);
