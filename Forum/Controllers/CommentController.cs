using Forum.Models;
using Forum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers;
[Authorize]
public class CommentController : Controller
{
    private readonly ForumContext _context;
    private readonly UserManager<User> _userManager;

    public CommentController(ForumContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> GetComments(string? postId, int pageNumber = 1, int pageSize = 2)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Post)
            .Where(c => c.PostId.ToString() == postId)
            .OrderBy(c => c.Created)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return PartialView("_CommentsPartial", comments);
    }
    public async Task<IActionResult> GetPagination(string? postId, int pageNumber = 1, int pageSize = 2)
    {
        var totalComments = await _context.Comments
            .Include (c => c.Post)
            .Where(c => c.PostId.ToString() == postId)
            .CountAsync();
        var totalPages = (int)Math.Ceiling(totalComments / (double)pageSize);

        var model = new PaginationViewModel
        {
            CurrentPage = pageNumber,
            TotalPages = totalPages
        };

        return PartialView("_PaginationPartial", model);
    }
    [HttpPost]
    public async Task<IActionResult> CreateAjax(string? username, string? postId, string? body)
    {
        if (username != null && postId != null && body != null)
        {
            User user = await _userManager.FindByNameAsync(username);
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id.ToString() == postId);
            if (user != null && post != null)
            {
                Comment cm = new()
                {
                    UserId = user.Id,
                    PostId = post.Id,
                    Body = body
                };
                await _context.AddAsync(cm);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
        }
        return Json(new { success = false });
    }
}
