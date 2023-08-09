using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterAPI.Dto;
using TwitterAPI.Interfaces;
using TwitterAPI.Models;

namespace TwitterAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public UserController(IUserRepository userRepository, UserManager<ApplicationUser> userManager, 
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _configuration = configuration;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
    public IActionResult GetUsers()
    {
        var users = _userRepository.GetUsers();
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(users);
    }

    [HttpGet("{userName}")]
    [ProducesResponseType(200, Type = typeof(UserDto))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserDto>> GetUser(string userName)
    {
        var user = await _userManager.Users.Include(u => u.Posts)
            .FirstOrDefaultAsync(u => u.UserName.Trim().ToUpper() == userName.Trim().ToUpper());
        if (user == null)
        {
            return NotFound();
        }

        return Ok(FromUser(user));
    }

    [HttpPost("register")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public IActionResult RegisterUser([FromBody] UserDto userToCreate, [FromQuery] string password)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var registerStatus = _userRepository.RegisterUser(userToCreate, password);
        
        if (!registerStatus.success)
        {
            return BadRequest(registerStatus.errors);
        }
        
        return Created("", "User successfully created.");
    }
    
    private static UserDto FromUser(ApplicationUser applicationUser)
    {
        return new UserDto
        {
            Id = applicationUser.Id,
            UserName = applicationUser.UserName,
            Email = applicationUser.Email,
            DisplayName = applicationUser.DisplayName,
            BirthDate = applicationUser.BirthDate,
            CreatedDate = applicationUser.CreatedDate,
            Bio = applicationUser.Bio,
            Posts = applicationUser.Posts,
        };
    }
    
    private static ApplicationUser FromUserDto(UserDto userDto)
    {
        return new ApplicationUser
        {
            Id = userDto.Id,
            UserName = userDto.UserName,
            Email = userDto.Email,
            DisplayName = userDto.DisplayName,
            BirthDate = userDto.BirthDate,
            CreatedDate = DateTime.Now,
            Bio = userDto.Bio,
        };
    }
}