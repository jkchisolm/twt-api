using Microsoft.AspNetCore.Mvc;
using TwitterAPI.Dto;
using TwitterAPI.Helpers;
using TwitterAPI.Models;

namespace TwitterAPI.Interfaces;

public interface IUserRepository
{
    ICollection<ApplicationUser> GetUsers();
    Task<ActionResult<ApplicationUser>> GetUserByName(string userName);
    RegisterUserReturnType RegisterUser(UserDto userInfo, string password);
}