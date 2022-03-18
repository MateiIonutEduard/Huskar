using Facebook;
using Huskar.Data;
using Huskar.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
#pragma warning disable

namespace Huskar.Controllers
{
    [IgnoreAntiforgeryToken]
    public class AccountController : Controller
    {
        private MovieContext db;
        public AccountController(MovieContext db)
        { this.db = db; }

        [HttpGet, Route("/me")]
        public async Task<IActionResult> GetUser(string name, int auth)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Name.CompareTo(name) == 0 && u.AuthModel == auth);
                if (user != null) return Ok(user);
            }
            return BadRequest();
        }

        [Route("/GoogleLogin")]
        public async Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities
                .FirstOrDefault().Claims.Select(claims => new
                {
                    claims.Issuer,
                    claims.OriginalIssuer,
                    claims.Type,
                    claims.Value
                });

            string name = User.Identity?.Name;
            string profile = User.Claims.FirstOrDefault(c => c.Type.Contains("urn:google:picture"))
                        ?.Value;

            var user = db.Users.FirstOrDefault(u => u.Name.CompareTo(name) == 0 && u.AuthModel == 2);

            if(user == null)
            {
                user = new User
                {
                    Name = name,
                    Profile = profile,
                    AuthModel = 2
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        [Route("/facebook-login")]
        public IActionResult FacebookLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse") };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [Route("/facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var str = HttpContext.User.FindFirstValue(ClaimTypes.Name);

            var user = db.Users.FirstOrDefault(u => u.Name.CompareTo(str) == 0 && u.AuthModel == 1);
            var identifier = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var picture = $"https://graph.facebook.com/{identifier}/picture?type=large";
            
            if (user == null)
            {
                user = new User
                {
                    Name = str,
                    Profile = picture,
                    AuthModel = 1
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("TopRated", "Home");
        }

        [Route("/signout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
