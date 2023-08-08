using TwitterAPI.Models;

namespace TwitterAPI.Dto;

public class PostDto
{
    public int Id { get; set; }
    public string TextContent { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime EditedDate { get; set; }
}