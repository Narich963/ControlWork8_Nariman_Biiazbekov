using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Controllers;
[Authorize]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ForumContext _context;
    public UserController(UserManager<User> userManager, ForumContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public async Task<IActionResult> Details(string? name)
    {
        if (name != null)
        {
            User user = await _context.Users
                .Include(u => u.Comments)
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.UserName == name);
            if (user != null)
            {
                return View(user);
            }
        }
        return NotFound();
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }
        if (user.UserName == User.Identity.Name)
        {
            return View(user);
        }
        return NotFound();
    }

    // POST: User/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, User user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                User oldUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                oldUser.Email = user.Email;
                oldUser.Avatar = user.Avatar;
                _context.Update(oldUser);
                await _userManager.UpdateAsync(oldUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index", "Home");
        }
        return View(user);
    }
    private bool UserExists(int id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
