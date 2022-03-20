using Huskar.Data;
using Huskar.Models;
using Huskar.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Huskar.Controllers
{
    public class HomeController : Controller
    {
        private MovieContext db;
        private MovieService ms;

        public HomeController(MovieContext db, MovieService ms)
        {
            this.db = db;
            this.ms = ms;
        }

        public async Task<IActionResult> Index(int? page)
        {
            ViewData["pages"] = await ms.UpcomingPages();
            int result = 1;

            if (page != null) result = page.Value;
            ViewData["pageid"] = result;

            var array = await ms.GetUpcoming(result);
            return View(array);
        }

        public async Task<IActionResult> Results(int? page, string[] filter, string? name)
        {
            var genres = string.Join(", ", filter);
            if (filter.Length == 0) genres = null;

            if (page != null)
            {
                var array = await ms.GetResults(page.Value, name, genres);
                return Ok(array);
            }
            else return Ok();
        }

        public async Task<IActionResult> TopRated(int? page)
        {
            ViewData["pages"] = await ms.GetPageCount();
            int result = 1;

            if (page != null) result = page.Value;
            ViewData["pageid"] = result;

            var array = await ms.GetRated(result);
            return View(array);
        }

        public async Task<IActionResult> Details(int id)
        {
            var obj = await ms.GetDetails(id);
            return View(obj);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}