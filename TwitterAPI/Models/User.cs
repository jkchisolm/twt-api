namespace TwitterAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Handle { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime BirthDate { get; set; }
    public ICollection<Post>? Posts { get; set; }
}