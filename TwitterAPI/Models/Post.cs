namespace TwitterAPI.Models;

public class Post
{
    public int Id { get; set; }
    public string TextContent { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime EditedDate { get; set; }
}