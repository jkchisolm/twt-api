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
            Password = userPassword,
            CreatedDate = userDto.CreatedDate,
            BirthDate = userDto.BirthDate,
            Posts = userDto.Posts
        };
    }
}