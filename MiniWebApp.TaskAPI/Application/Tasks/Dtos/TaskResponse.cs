using TaskStatus = MiniWebApp.TaskAPI.Domain.Entities.Enums.TaskStatus;

namespace MiniWebApp.TaskAPI.Application.Tasks.Dtos;

public sealed record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    DateTime LastModified
);
