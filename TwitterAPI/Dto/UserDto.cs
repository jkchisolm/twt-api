using TwitterAPI.Models;

namespace TwitterAPI.Dto;

public class UserDto
{
    public string? Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime BirthDate { get; set; }
    public ICollection<Post>? Posts { get; set; }
}