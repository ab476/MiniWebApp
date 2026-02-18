namespace MiniWebApp.UserApi.Domain.Models;

public class TTenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Domain { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
