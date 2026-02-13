using System.ComponentModel.DataAnnotations;

namespace MiniWebApp.TaskAPI.Domain.Entities;

public class NoteTag
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Tag { get; set; } = null!;

    public int NoteId { get; set; }
    public Note Note { get; set; } = null!;
}