using System.ComponentModel.DataAnnotations;

namespace MiniWebApp.TaskAPI.Domain.Entities;

public class Note
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Soft delete
    public bool IsDeleted { get; set; } = false;

    public ICollection<NoteTag> Tags { get; set; } = new List<NoteTag>();
}