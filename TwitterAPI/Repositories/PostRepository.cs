using TwitterAPI.Data;
using TwitterAPI.Interfaces;
using TwitterAPI.Models;

namespace TwitterAPI.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context)
    {
        _context = context;
    }
    public ICollection<Post> GetPosts()
    {
        return _context.Posts.OrderBy(p => p.Id).ToList();
    }

    public Post GetPost(int postId)
    {
        return _context.Posts.FirstOrDefault(p => p.Id == postId);
    }
}