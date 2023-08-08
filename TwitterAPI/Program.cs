using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TwitterAPI.Data;
using TwitterAPI.Interfaces;
using TwitterAPI.Models;
using TwitterAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var conn = builder.Configuration.GetConnectionString("WebApiDatabase");

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddIdentityCore<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<AppDbContext>();


var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Migrate the latest changes to the database during startup
using (var scope = app.Services.CreateScope())
{
    Console.WriteLine(conn);
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();