using Riok.Mapperly.Abstractions;
using System.Runtime.ConstrainedExecution;

namespace MiniWebApp.TaskAPI.Application.Tasks.Dtos;

[Mapper]
public static partial class TaskItemMapper
{
    public static partial TaskResponse ToResponse(this TaskItem entity);
    public static partial IQueryable<TaskResponse> ProjectToTaskResponse(this IQueryable<TaskItem> query);

    [MapperIgnoreTarget(nameof(TaskItem.Id))]
    [MapperIgnoreTarget(nameof(TaskItem.LastModified))]
    public static partial TaskItem ToEntity(this CreateTaskRequest request);

    [MapperIgnoreTarget(nameof(TaskItem.LastModified))]
    public static partial TaskItem ToEntity(this UpdateTaskRequest request);
}
