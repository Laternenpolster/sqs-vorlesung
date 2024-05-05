using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Pokemon
{
    [Key]
    [MaxLength(20)]
    public required string Name { get; set; }
}