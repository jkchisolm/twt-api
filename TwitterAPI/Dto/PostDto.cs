using TwitterAPI.Models;

namespace TwitterAPI.Dto;

public class PostDto
{
    public int Id { get; set; }
    public int TextContent { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime EditedDate { get; set; }
}