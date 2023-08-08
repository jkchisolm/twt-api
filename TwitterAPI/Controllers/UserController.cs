using Microsoft.AspNetCore.Mvc;
using TwitterAPI.Dto;
using TwitterAPI.Interfaces;
using TwitterAPI.Models;

namespace TwitterAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
    public IActionResult GetUsers()
    {
        var users = _userRepository.GetUsers().Select(u => FromUser(u)).ToList();
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(users);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(200, Type = typeof(UserDto))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetUser(int userId)
    {
        if (!_userRepository.UserExists(userId)) return NotFound();
        var user = _userRepository.GetUser(userId);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(FromUser(user));
    }

    [HttpPost("register")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public IActionResult CreateUser([FromBody] UserDto userToCreate, [FromQuery] string userPassword)
    {
        if (_userRepository.UserExists(userToCreate.Email))
        {
            ModelState.AddModelError("", "User with that email address already exists.");
            return StatusCode(422, ModelState);
        }

        if (!_userRepository.RegisterUser(userToCreate, userPassword))
        {
            ModelState.AddModelError("", $"Something went wrong saving the user");
            return StatusCode(500, ModelState);
        }
        return Created("", "User successfully created.");
    }
    
    [HttpPost("login")]
    [ProducesResponseType(200, Type = typeof(UserDto))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult LoginUser([FromQuery] string userEmail, [FromQuery] string userPassword)
    {
        if (!_userRepository.UserExists(userEmail)) return NotFound();
        var user = _userRepository.LoginUser(userEmail, userPassword);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(user);
    }

    private static UserDto FromUser(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            handle = user.Handle,
            Email = user.Email,
            CreatedDate = user.CreatedDate,
            BirthDate = user.BirthDate,
            Posts = user.Posts
        };
    }
    
    private static User FromUserDto(UserDto userDto, string userPassword)
    {
        return new User
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Handle = userDto.handle,
            Email = userDto.Email,
            // Password = userPassword,
            CreatedDate = userDto.CreatedDate,
            BirthDate = userDto.BirthDate,
            Posts = userDto.Posts
        };
    }
}