using TwitterAPI.Models;

namespace TwitterAPI.Dto;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string handle { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime BirthDate { get; set; }
    public ICollection<Post>? Posts { get; set; }
}