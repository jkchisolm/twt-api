using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TwitterAPI.Models;

namespace TwitterAPI.Data;

public class AppDbContext : IdentityUserContext<User>
{
    private readonly IConfiguration _configuration;

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>().Property(e => e.Handle).HasMaxLength(50);
        
        builder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .HasPrincipalKey(u => u.Id)
            .HasConstraintName("FK_Posts_AspNetUsers_UserId");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to postgres
        options.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));
    }
    
    public DbSet<Post> Posts { get; set; }
    // public DbSet<User> Users { get; set; }
}