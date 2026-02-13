using Microsoft.EntityFrameworkCore;
using MiniWebApp.TaskAPI.Application.Tasks.Dtos;
using MiniWebApp.TaskAPI.Domain;

namespace MiniWebApp.TaskAPI.Application.Tasks;

public class TaskItemService(TaskAPIDbContext db)
{
    public async Task<TaskResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var taskItem = await db.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        return taskItem?.ToResponse();
    }

    public async Task<IReadOnlyList<TaskResponse>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        return await db.Tasks
            .AsNoTracking()
            .OrderByDescending(t => t.LastModified)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToTaskResponse()
            .ToListAsync(ct);
    }

    public async Task<TaskResponse> CreateAsync(
        CreateTaskRequest taskRequest,
        CancellationToken ct = default)
    {
        var task = taskRequest.ToEntity();

        task.Id = Guid.NewGuid();
        task.LastModified = DateTime.UtcNow;

        db.Tasks.Add(task);
        await db.SaveChangesAsync(ct);

        return task.ToResponse();
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateTaskRequest taskRequest,
        DateTime lastModified,
        CancellationToken ct = default)
    {
        var rowsAffected = await db.Tasks
            .Where(t => t.Id == id && t.LastModified == lastModified)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Title, taskRequest.Title)
                .SetProperty(t => t.Description, taskRequest.Description)
                .SetProperty(t => t.Status, taskRequest.Status)
                .SetProperty(t => t.Priority, taskRequest.Priority)
                .SetProperty(t => t.DueDate, taskRequest.DueDate)
                .SetProperty(t => t.LastModified, DateTime.UtcNow),
                ct);

        return rowsAffected == 1;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var rowsAffected = await db.Tasks
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync(ct);

        return rowsAffected == 1;
    }
}
