namespace TwitterAPI.Models;

public class Post
{
    public int Id { get; set; }
    public string TextContent { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime EditedDate { get; set; }
    
    // Properties for the author
    public string UserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;
}