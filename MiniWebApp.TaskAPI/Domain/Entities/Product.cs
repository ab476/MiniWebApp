using System.ComponentModel.DataAnnotations;

namespace MiniWebApp.TaskAPI.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required, MaxLength(100)]
    public string Category { get; set; } = null!;

    public int Stock { get; set; }
}