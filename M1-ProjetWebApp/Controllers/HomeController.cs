using Microsoft.AspNetCore.Mvc;
using M1_ProjetWebApp.Data;
using M1_ProjetWebApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace M1_ProjetWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /
        public async Task<IActionResult> Index()
        {
            var NUMBER_OF_ITEMS = 3;
            var viewModel = new HomeViewModel
            {
                LatestProjects = await _context.Projects
                    .OrderByDescending(p => p.DateCreated)
                    .Take(NUMBER_OF_ITEMS)
                    .ToListAsync(),

                LatestArticles = await _context.Articles
                    .Include(a => a.Author)
                    .OrderByDescending(a => a.PublishedDate)
                    .Take(NUMBER_OF_ITEMS)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}
