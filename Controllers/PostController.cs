using Huskar.Data;
using Huskar.Models;
using Huskar.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Huskar.Controllers
{
    [IgnoreAntiforgeryToken]
    public class PostController : ControllerBase
    {
        private IPostService ps;
        public PostController(IPostService ps)
        { this.ps = ps; }

        public async Task<JsonResult> Posts(int MovieId)
        {
            var posts = await ps.GetPosts(MovieId);
            return new JsonResult(posts);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Post(int UserId, string Message, long MovieId)
        {
            var res = await ps.Post(UserId, Message, MovieId);
            if(res != null) return Created("Post/Post", res);
            return Ok();
        }

        [HttpDelete, Authorize]
        public async Task<IActionResult> Remove(int PostId)
        {
            bool res = await ps.Remove(PostId);
            if (res) return Ok();
            return BadRequest();
        }
    }
}
