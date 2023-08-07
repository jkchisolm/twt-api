using Microsoft.AspNetCore.Mvc;
using TwitterAPI.Dto;
using TwitterAPI.Interfaces;
using TwitterAPI.Models;
using TwitterAPI.Repositories;

namespace TwitterAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : Controller
{
    private readonly IPostRepository _postRepository;

    public PostController(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<PostDto>))]
    public IActionResult GetPosts()
    {
        var posts = _postRepository.GetPosts().Select(p => FromPost(p)).ToList();
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(posts);
    }
    
    
    private static PostDto FromPost(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            TextContent = post.TextContent,
            CreatedDate = post.CreatedDate,
            EditedDate = post.EditedDate
        };
    }
}