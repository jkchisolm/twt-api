using Microsoft.AspNetCore.Identity;

namespace TwitterAPI.Models;

public class User : IdentityUser
{
    public string Handle { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime BirthDate { get; set; }
    public ICollection<Post>? Posts { get; set; }
}