using TwitterAPI.Dto;
using TwitterAPI.Models;

namespace TwitterAPI.Interfaces;

public interface IUserRepository
{
    public ICollection<User> GetUsers();
    User GetUser(int userId);
    User GetUserByEmail(string email);
    User GetUserByHandle(string handle);
    bool UserExists(int userId);
    bool UserExists(string email);
    bool RegisterUser(UserDto user, string password);
    UserDto LoginUser(string email, string password);
    bool UpdateUser(int userId, User updatedUser);
    bool DeleteUser(int userId);
    bool Save();
}