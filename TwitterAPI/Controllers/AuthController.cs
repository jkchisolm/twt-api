using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TwitterAPI.Auth;
using TwitterAPI.Dto;
using TwitterAPI.Interfaces;
using TwitterAPI.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TwitterAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ISecretHelper _secretHelper;

    public AuthController(UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration,
        ISecretHelper secretHelper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _secretHelper = secretHelper;
    }

    [HttpPost("login")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginInfo)
    {
        if (loginInfo.Email == null && loginInfo.UserName == null)
        {
            return BadRequest("Handle or email is required");
        }
        else if (loginInfo.Password == null)
        {
            return BadRequest("Password is required");
        }
        else
        {
            var user = await _userManager.FindByEmailAsync(loginInfo.Email);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(loginInfo.UserName);
                if (user == null)
                {
                    return NotFound("User not found");
                }
            }
            
            if (await _userManager.CheckPasswordAsync(user, loginInfo.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.PrimarySid, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = await CreateToken(authClaims);
                var refreshToken = GenerateRefreshToken();
                
                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
                
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
                
                await _userManager.UpdateAsync(user);
                
                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                });
            }
            
            return BadRequest("Invalid credentials");
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerInfo)
    {
        // check if user with same email or handle exists
        var userExists = await _userManager.FindByEmailAsync(registerInfo.UserEmail);
        if (userExists != null)
        {
            return BadRequest("User with this email already exists");
        }
        userExists = await _userManager.FindByNameAsync(registerInfo.UserName);
        if (userExists != null)
        {
            return BadRequest("User with this handle already exists");
        }

        ApplicationUser user = new()
        {
            Email = registerInfo.UserEmail,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerInfo.UserName,
            DisplayName = registerInfo.DisplayName,
        };

        var result = await _userManager.CreateAsync(user, registerInfo.Password);
        if (!result.Succeeded)
        {
            return StatusCode(500, result.Errors);
        }

        return Ok("User registered successfully.");
    }

    [HttpPost("registerAdmin")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto registerInfo)
    {
        // check if user with same email or handle exists
        var userExists = await _userManager.FindByEmailAsync(registerInfo.UserEmail);
        if (userExists != null)
        {
            return BadRequest("User with this email already exists");
        }
        userExists = await _userManager.FindByNameAsync(registerInfo.UserName);
        if (userExists != null)
        {
            return BadRequest("User with this handle already exists");
        }

        ApplicationUser user = new()
        {
            Email = registerInfo.UserEmail,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerInfo.UserName,
            DisplayName = registerInfo.DisplayName,
        };
        var result = await _userManager.CreateAsync(user, registerInfo.Password);
        
        if (!result.Succeeded)
        {
            return StatusCode(500, result.Errors);
        }
        
        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        }
        
        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        }
        
        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        }

        if (await _roleManager.RoleExistsAsync(UserRoles.User))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.User);
        }
        
        return Ok("User registered successfully.");
    }

    [HttpPost("refreshToken")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenInfo)
    {
        if (tokenInfo is null)
        {
            return BadRequest("Invalid client request");
        }

        string? accessToken = tokenInfo.AccessToken;
        string? refreshToken = tokenInfo.RefreshToken;

        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return BadRequest("Invalid access or refresh token.");
        }
        
        var userName = principal.Identity?.Name;
        
        var user = await _userManager.FindByNameAsync(userName);
        
        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Invalid access or refresh token.");
        }

        var newAccessToken = await CreateToken(principal.Claims.ToList());
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken,
        });
    }
    
    [Authorize]
    [HttpPost("revoke/{UserName}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Revoke(string UserName)
    {
        var user = await _userManager.FindByNameAsync(UserName);
        if (user == null)
        {
            return BadRequest("Invalid client request");
        }
        
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        
        return Ok("User logged out successfully.");
    }
    
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(200, Type = typeof(UserDto))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Me()
    {
        var userName = User.Identity?.Name;
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return BadRequest("Invalid client request");
        }
        
        return Ok(new
        {
            user.DisplayName,
            user.Email,
            user.UserName,
        });
    }
    


    private async Task<JwtSecurityToken> CreateToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            await _secretHelper.GetSecretAsync("JwtSecret")));
        _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);
        
        var token = new JwtSecurityToken(
            issuer:_configuration["JWT:ValidIssuer"], 
            audience: _configuration["JWT:ValidAudience"], 
            expires: DateTime.Now.AddMinutes(tokenValidityInMinutes), 
            claims: authClaims, 
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return token;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (!(securityToken is JwtSecurityToken jwtSecurityToken) 
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}