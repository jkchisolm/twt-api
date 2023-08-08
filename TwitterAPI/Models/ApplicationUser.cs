using Microsoft.AspNetCore.Identity;

namespace TwitterAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime BirthDate { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<Post>? Posts { get; set; }
}