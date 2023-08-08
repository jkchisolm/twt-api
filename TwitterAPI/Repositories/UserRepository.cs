using Microsoft.EntityFrameworkCore;
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
        return _context.Users.Where(u => u.Id == userId)
            .Include(u => u.Posts).FirstOrDefault();
    }

    public User GetUserByEmail(string email)
    {
        return _context.Users.Where(u => u.Email == email)
            .Include(u => u.Posts).FirstOrDefault();
    }

    public User GetUserByHandle(string handle)
    {
        return _context.Users.Where(u => u.Handle == handle)
            .Include(u => u.Posts).FirstOrDefault();
    }

    public bool UserExists(int userId)
    {
        return _context.Users.Any(u => u.Id == userId);
    }

    public bool UserExists(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }

    public bool UserExistsByHandle(string handle)
    {
        return _context.Users.Any(u => u.Handle == handle);
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

    public UserDto LoginUser(string email, string password)
    {
        var user = GetUserByEmail(email);
        if (user == null) return null;
        var computedHash = PasswordHasher.ComputeHash(password, user.PasswordSalt, _pepper, 3);
        if (computedHash != user.PasswordHash) throw new Exception("Username or password did not match.");
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            handle = user.Handle,
            Email = user.Email,
            CreatedDate = user.CreatedDate,
            BirthDate = user.BirthDate
        };
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