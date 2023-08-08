using System.ComponentModel.DataAnnotations;

namespace TwitterAPI.Dto;

public class RegisterDto
{
    [Required]
    public string UserName { get; set; } = null!;

    [Required] 
    [EmailAddress]
    public string UserEmail { get; set; } = null!;

    [Required] 
    public string DisplayName { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
}