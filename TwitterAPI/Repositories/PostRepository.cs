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

    public bool PostExists(int postId)
    {
        return _context.Posts.Any(p => p.Id == postId);
    }

    public bool CreatePost(Post post)
    {
        _context.Add(post);
        return Save();
    }

    public bool UpdatePost(int postId, Post updatedPost)
    {
        var post = GetPost(postId);
        post.TextContent = updatedPost.TextContent;
        post.EditedDate = DateTime.Now;
        _context.Posts.Update(post);
        return Save();
    }

    public bool DeletePost(Post post)
    {
        throw new NotImplementedException();
    }

    public bool Save()
    {
        var saved = _context.SaveChanges();
        return saved > 0;
    }
}