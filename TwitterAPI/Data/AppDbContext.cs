using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TwitterAPI.Models;

namespace TwitterAPI.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder options)
    // {
    //     // connect to postgres
    //     options.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));
    // }
    
    public DbSet<Post> Posts { get; set; }
}