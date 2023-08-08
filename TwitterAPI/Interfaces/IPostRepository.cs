using TwitterAPI.Models;

namespace TwitterAPI.Interfaces;

public interface IPostRepository
{
    ICollection<Post> GetPosts();
    Post GetPost(int postId);
    bool PostExists(int postId);
    bool CreatePost(Post post);
    bool Save();
}