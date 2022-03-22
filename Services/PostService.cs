using Huskar.Data;
using Huskar.Models;
using Microsoft.EntityFrameworkCore;

namespace Huskar.Services
{
    public class PostService : IPostService
    {
        readonly MovieContext db;

        public PostService(MovieContext db)
        { this.db = db; }
        public async Task<PostMessage[]> GetPosts(int MovieId)
        {
            var all = (from p in await db.Posts.ToListAsync()
                       join u in db.Users.ToList()
                       on p.UserId equals u.Id
                       where p.MovieId == MovieId
                       select new PostMessage
                       {
                           Id = p.Id,
                           Name = u.Name,
                           Message = p.Message,
                           Date = p.Date,
                           Profile = u.Profile,
                           MovieId = p.MovieId,
                           UserId = u.Id
                       }).ToArray();

            return all;
        }

        public async Task<Post> Post(int UserId, string Message, long MovieId)
        {
            var post = await db.Posts.FirstOrDefaultAsync(p => p.UserId == UserId && p.Message.CompareTo(Message) == 0);

            if (post == null)
            {
                post = new Post
                {
                    Message = Message,
                    MovieId = MovieId,
                    Date = DateTime.Now,
                    UserId = UserId
                };

                db.Posts.Add(post);
                await db.SaveChangesAsync();
                return post;
            }

            return null;
        }

        public async Task<bool> Remove(int PostId)
        {
            var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == PostId);

            if (post != null)
            {
                db.Posts.Remove(post);
                await db.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
