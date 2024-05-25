using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Forum.Controllers;

[Authorize]
public class PostController : Controller
{
    private readonly ForumContext _context;
    private readonly UserManager<User> _userManager;
    private int PageSize = 2;

    public PostController(ForumContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Post
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        return View();
    }
    public async Task<IActionResult> GetPosts(int pageNumber = 1, int pageSize = 2)
    {
        var posts = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.Created)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        int totalPosts = await _context.Posts.CountAsync();

        var model = new PaginationViewModel()
        {
            Posts = posts,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize)
        };

        return PartialView("_PostsPartial", model);
    }
    [AllowAnonymous]
    // GET: Post/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var post = await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    // GET: Post/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Post/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Post post)
    {
        User user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            post.UserId = user.Id;
            if (ModelState.IsValid)
            {
                await _context.AddAsync(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
        return View(post);
    }

    // GET: Post/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
        return View(post);
    }

    // POST: Post/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,Created,UserId")] Post post)
    {
        if (id != post.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(post);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(post.Id))
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
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
        return View(post);
    }

    // GET: Post/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var post = await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    // POST: Post/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PostExists(int id)
    {
        return _context.Posts.Any(e => e.Id == id);
    }
}
