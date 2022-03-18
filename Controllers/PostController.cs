using Huskar.Data;
using Huskar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Huskar.Controllers
{
    [IgnoreAntiforgeryToken]
    public class PostController : ControllerBase
    {
        MovieContext db;
        public PostController(MovieContext db)
        { this.db = db; }

        public async Task<JsonResult> Posts(int MovieId)
        {
            var all = (from p in await db.Posts.ToListAsync() join u in db.Users.ToList()
                       on p.UserId equals u.Id where p.MovieId == MovieId select new PostMessage
                       {
                           Id = p.Id,
                           Name = u.Name,
                           Message = p.Message,
                           Date = p.Date,
                           Profile = u.Profile,
                           MovieId = p.MovieId
                       }).ToList();
            return new JsonResult(all);
        }

        [HttpPost]
        public async Task<IActionResult> Post(int UserId, string Message, int MovieId)
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
            }

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
                           MovieId = p.MovieId
                       }).ToList();
            return Ok(all);
        }

        [HttpDelete]
        public async Task<IActionResult> Remove(int PostId)
        {
            var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == PostId);

            if(post != null)
            {
                db.Posts.Remove(post);
                await db.SaveChangesAsync();
                return Ok();
            }

            return BadRequest();
        }
    }
}
