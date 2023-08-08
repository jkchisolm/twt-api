using TwitterAPI.Data;
using TwitterAPI.Dto;
using TwitterAPI.Interfaces;
using TwitterAPI.Models;
using TwitterAPI.Services;

namespace TwitterAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly string _pepper;

    public UserRepository(AppDbContext context)
    {
        _context = context;
        _pepper = Environment.GetEnvironmentVariable("PasswordHashPepper") 
                  ?? throw new ArgumentNullException("PasswordHashPepper");
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

    public bool RegisterUser(UserDto userInfo, string password)
    {
        var user = new User
        {
            Name = userInfo.Name,
            Handle = userInfo.handle,
            Email = userInfo.Email,
            CreatedDate = DateTime.Now,
            BirthDate = userInfo.BirthDate,
            PasswordSalt = PasswordHasher.GenerateSalt()
        };
        user.PasswordHash = PasswordHasher.ComputeHash(password, user.PasswordSalt, _pepper, 3);
        _context.Users.Add(user);
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
        var saved = _context.SaveChanges();
        return saved > 0;
    }
}