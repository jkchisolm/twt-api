using Microsoft.AspNetCore.Mvc;
using TwitterAPI.Dto;
using TwitterAPI.Interfaces;
using TwitterAPI.Models;

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

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public IActionResult CreatePost([FromBody] PostDto postToCreate)
    {
        if (postToCreate == null) return BadRequest(ModelState);
        
        var post = FromPostDto(postToCreate);
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        if (!_postRepository.CreatePost(post))
        {
            ModelState.AddModelError("", $"Something went wrong saving the post");
            return StatusCode(500, ModelState);
        }

        return Ok("Post successfully created.");
    }

    [HttpPut]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public IActionResult EditPost([FromQuery] int postId, [FromBody] PostDto updatedPost)
    {
        if (postId == null) return BadRequest(ModelState);
        if (updatedPost == null) return BadRequest(ModelState);
        
        // check if the post already exists, if not, return 404
        if (!_postRepository.PostExists(postId)) return NotFound();

        var post = FromPostDto(updatedPost);
        
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        if (!_postRepository.UpdatePost(postId, post))
        {
            ModelState.AddModelError("", $"Something went wrong updating the post");
            return StatusCode(500, ModelState);
        }
        
        return Ok("Post successfully updated.");
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
    
    private static Post FromPostDto(PostDto postDto)
    {
        return new Post
        {
            Id = postDto.Id,
            TextContent = postDto.TextContent,
            CreatedDate = DateTime.Now,
            EditedDate = DateTime.Now 
        };
    }
}