using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterAPI.Data;
using TwitterAPI.Dto;
using TwitterAPI.Helpers;
using TwitterAPI.Interfaces;
using TwitterAPI.Models;
using TwitterAPI.Services;

namespace TwitterAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public ICollection<User> GetUsers()
    {
        return _userManager.Users.ToList();
    }

    public async Task<ActionResult<User>> GetUserByName(string userName)
    {
        var result = await _userManager.FindByNameAsync(userName);
        if (result == null)
        {
            return new NotFoundResult();
        }
        
        return result;
    }

    public RegisterUserReturnType RegisterUser(UserDto userInfo, string password)
    {
        var userToCreate = new User
        {
            UserName = userInfo.UserName,
            Email = userInfo.Email,
            Handle = userInfo.Handle,
            CreatedDate = DateTime.Now,
            BirthDate = userInfo.BirthDate,
            Bio = userInfo.Bio,
        };

        var result = _userManager.CreateAsync(userToCreate, password).Result;
        if (!result.Succeeded)
        {
            return new RegisterUserReturnType() { success = false, errors = result.Errors };
        }

        return new RegisterUserReturnType() { success = true };

        // return result.Succeeded;
    }
}