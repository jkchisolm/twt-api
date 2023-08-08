using System.ComponentModel.DataAnnotations;

namespace TwitterAPI.Dto;

public class LoginDto
{
    public string? Email { get; set; } = null!;

    public string? UserName { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
}