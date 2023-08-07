namespace TwitterAPI.Models;

public class Post
{
    public int Id { get; set; }
    public int TextContent { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime EditedDate { get; set; }
}