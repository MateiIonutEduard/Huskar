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
        private IAccountService account;
        public AccountController(IAccountService account)
        { this.account = account; }

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

        public async Task<IActionResult> Redirect()
        {
            var name = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await account.InstagramSignIn(name);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet, Route("/avatar")]
        public async Task<IActionResult> Avatar(int id)
        {
            var image = await account.GetProfile(id);
            if (image != null) return File(image, "image/png");
            return NotFound();
        }

        [HttpGet, Route("/profile")]
        public async Task<IActionResult> GetProfile(string name, int auth)
        {
            if(!string.IsNullOrEmpty(name))
            {
                var image = await account.GetProfile(name, auth);
                if(image != null) return File(image, "image/png");
            }

            return NotFound();
        }

        [HttpGet, Route("/me")]
        public async Task<IActionResult> GetUser(string name, int auth)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var user = await account.GetUser(name, auth);
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

            await account.GoogleSignIn(name, profile);
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
            var name = HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var identifier = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var token = await HttpContext.GetTokenAsync("access_token");
            await account.FacebookSignIn(identifier, name, token);
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
