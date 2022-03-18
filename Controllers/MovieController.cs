using Huskar.Data;
using Microsoft.AspNetCore.Mvc;
using Huskar.Services;

namespace Huskar.Controllers
{
    [IgnoreAntiforgeryToken]
    public class MovieController : Controller
    {
        private MovieContext db;
        private MovieService ms;
        public MovieController(MovieContext db, MovieService ms)
        {
            this.db = db;
            this.ms = ms;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Latest(int? page)
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
    }
}
