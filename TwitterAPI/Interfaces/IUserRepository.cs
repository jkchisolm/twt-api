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
    bool CreateUser(User user);
    bool UpdateUser(int userId, User updatedUser);
    bool DeleteUser(int userId);
    bool Save();
}