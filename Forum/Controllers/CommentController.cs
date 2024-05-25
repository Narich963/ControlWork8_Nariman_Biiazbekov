using Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers;

public class CommentController : Controller
{
    private readonly ForumContext _context;
    private readonly UserManager<User> _userManager;

    public CommentController(ForumContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> GetComments()
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Post)
            .OrderBy(c => c.Created)
            .ToListAsync();
        return PartialView("_CommentsPartial", comments);
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
