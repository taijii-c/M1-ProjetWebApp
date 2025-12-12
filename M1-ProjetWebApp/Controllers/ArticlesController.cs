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
        private const int PAGE_SIZE = 5;

        public ArticlesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Articles
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var articles = _context.Articles
                .Include(a => a.Author)
                .OrderByDescending(a => a.PublishedDate);

            var pageIndex = pageNumber ?? 1;
            var paginatedArticles = await PaginatedList<Article>.CreateAsync(articles, pageIndex, PAGE_SIZE);

            return View(paginatedArticles);
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
                .Include(a => a.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null)
            {
                return NotFound();
            }

            var viewModel = new ArticleDetailsViewModel
            {
                Article = article,
                Comments = article.Comments.OrderByDescending(c => c.PublishedDate).ToList(),
                NewComment = new CommentViewModel { ArticleId = article.Id }
            };

            return View(viewModel);
        }

        // POST: Articles/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddComment(CommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var comment = new Comment
                {
                    Content = model.Content,
                    ArticleId = model.ArticleId,
                    AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                    PublishedDate = DateTime.Now
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = model.ArticleId });
            }

            var article = await _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == model.ArticleId);

            if (article == null)
            {
                return NotFound();
            }

            var viewModel = new ArticleDetailsViewModel
            {
                Article = article,
                Comments = article.Comments.OrderByDescending(c => c.PublishedDate).ToList(),
                NewComment = model
            };

            return View("Details", viewModel);
        }

        // POST: Articles/DeleteComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            if (!IsAuthorOrAdmin(comment.AuthorId))
            {
                return Forbid();
            }

            var articleId = comment.ArticleId;
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = articleId });
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

            if (!IsAuthorOrAdmin(article.AuthorId))
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

            if (!IsAuthorOrAdmin(originalArticle.AuthorId))
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

            if (!IsAuthorOrAdmin(article.AuthorId))
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
                if (!IsAuthorOrAdmin(article.AuthorId))
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
        private bool IsAuthorOrAdmin(string authorId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return authorId == currentUserId || User.IsInRole("Admin");
        }
    }
}
