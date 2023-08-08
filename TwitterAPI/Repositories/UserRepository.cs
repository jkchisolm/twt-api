using TwitterAPI.Data;
using TwitterAPI.Interfaces;
using TwitterAPI.Models;

namespace TwitterAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public ICollection<User> GetUsers()
    {
        return _context.Users.OrderBy(u => u.Id).ToList();
    }

    public User GetUser(int userId)
    {
        return _context.Users.FirstOrDefault(u => u.Id == userId);
    }

    public User GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    public User GetUserByHandle(string handle)
    {
        return _context.Users.FirstOrDefault(u => u.Handle == handle);
    }

    public bool UserExists(int userId)
    {
        return _context.Users.Any(u => u.Id == userId);
    }

    public bool UserExists(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }

    public bool CreateUser(User user)
    {
        _context.Add(user);
        return Save();
    }

    public bool UpdateUser(int userId, User updatedUser)
    {
        throw new NotImplementedException();
    }

    public bool DeleteUser(int userId)
    {
        throw new NotImplementedException();
    }

    public bool Save()
    {
        throw new NotImplementedException();
    }
}