using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using M1_ProjetWebApp.Data;
using M1_ProjetWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using M1_ProjetWebApp.Models.ViewModels;

namespace M1_ProjetWebApp.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ArticlesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var articles = await _context.Articles
                .Include(a => a.Author)
                .OrderByDescending(a => a.PublishedDate)
                .ToListAsync();
            return View(articles);
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        [Authorize(Roles = "Admin,Auteur")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Auteur")]
        public async Task<IActionResult> Create(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var article = new Article
                {
                    Title = model.Title,
                    Content = model.Content,
                    AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                    PublishedDate = DateTime.Now
                };

                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Articles/Edit/5
        [Authorize(Roles = "Admin,Auteur")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            if (!IsAuthorOrAdmin(article))
            {
                return Forbid();
            }

            var model = new ArticleViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                PublishedDate = article.PublishedDate,
                AuthorId = article.AuthorId
            };

            return View(model);
        }

        // POST: Articles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Auteur")]
        public async Task<IActionResult> Edit(int id, ArticleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var originalArticle = await _context.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

            if (originalArticle == null)
            {
                return NotFound();
            }

            if (!IsAuthorOrAdmin(originalArticle))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var article = await _context.Articles.FindAsync(id);
                    if (article == null)
                    {
                        return NotFound();
                    }

                    article.Title = model.Title;
                    article.Content = model.Content;
                    // Keep original author and published date
                    article.AuthorId = originalArticle.AuthorId;
                    article.PublishedDate = originalArticle.PublishedDate;

                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Articles/Delete/5
        [Authorize(Roles = "Admin,Auteur")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            if (!IsAuthorOrAdmin(article))
            {
                return Forbid();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Auteur")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article != null)
            {
                if (!IsAuthorOrAdmin(article))
                {
                    return Forbid();
                }

                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }

        /**
         * Verify if the current user is the author of the article or an admin
         */
        private bool IsAuthorOrAdmin(Article article)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return article.AuthorId == currentUserId || User.IsInRole("Admin");
        }
    }
}
