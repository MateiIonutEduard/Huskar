using Huskar.Data;
using Huskar.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using AspNet.Security.OAuth.Instagram;
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

        [Route("/signin-instagram")]
        public async Task InstagramLogin()
        {
            if(User.Identity.IsAuthenticated)
                await HttpContext.SignOutAsync();

            await HttpContext.ChallengeAsync("Instagram", new AuthenticationProperties
            {
                RedirectUri = Url.Action("Redirect")
            });
        }

        public async Task<string> FacebookProfile(string id, string token)
        {
            string str = $"https://graph.facebook.com/{id}?fields=picture&access_token={token}";
            var client = new HttpClient();
            var res = await client.GetAsync(str);
            var data = await res.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(data);
            return obj.picture.data.url.ToString();
        }

        public async Task<string> InstagramProfile(string name, string token)
        {
            string str = $"https://www.instagram.com/{name}/?__a=1";
            var client = new HttpClient();
            var res = await client.GetAsync(str);
            var data = await res.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(data);
            return obj.graphql.user.profile_pic_url.ToString();
        }

        public async Task<IActionResult> Redirect()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var name = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            string picture = await InstagramProfile(name, token);

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var user = db.Users.FirstOrDefault(u => u.Name.CompareTo(name) == 0 && u.AuthModel == 3);
            var identifier = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (user == null)
            {
                user = new User
                {
                    Name = name,
                    Profile = picture,
                    AuthModel = 3
                };

                db.Users.Add(user);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }

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
            if (User.Identity.IsAuthenticated)
                await HttpContext.SignOutAsync();

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
        public async Task<IActionResult> FacebookLogin()
        {
            if (User.Identity.IsAuthenticated)
                await HttpContext.SignOutAsync();
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
            
            var token = await HttpContext.GetTokenAsync("access_token");
            var picture = await FacebookProfile(identifier, token);

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

            return RedirectToAction("Index", "Home");
        }

        [Route("/signout")]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
                await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
