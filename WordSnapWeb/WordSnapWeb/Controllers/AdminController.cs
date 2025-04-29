using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Models;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _users;

    public AdminController(UserManager<ApplicationUser> users)
    {
        _users = users;
    }

    public IActionResult Users()
    {
        var all = _users.Users.OrderBy(u => u.UserName).ToList();
        return View(all);
    }

    [HttpPost]
    public async Task<IActionResult> BanUser(string userId)
    {
        if (userId == null)
        {
            return BadRequest();
        }
        var user = await _users.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        await _users.DeleteAsync(user);

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    public async Task<IActionResult> LockUser(string userId, int days = 1)
    {
        var user = await _users.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        await _users.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(days));

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    public async Task<IActionResult> UnlockUser(string userId)
    {
        var user = await _users.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        await _users.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);

        return RedirectToAction(nameof(Users));
    }

}
