using System.ComponentModel.DataAnnotations;

namespace IczTask.Models;

public class Task
{
    public int Id { get; init; }

    [Required]
    [MinLength(5)]
    [MaxLength(255)]
    public string Name { get; init; } = string.Empty;


    [MaxLength(255)] public string? Description { get; set; }

    public bool Done { get; set; }
}