using Huskar.Data;
using Huskar.Models;

namespace Huskar.Services
{
    public interface IPostService
    {
        public Task<PostMessage[]> GetPosts(int MovieId);
        public Task<Post> Post(int UserId, string Message, long MovieId);
        public Task<bool> Remove(int PostId);
    }
}
