using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Models;

namespace WordSnapWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWordSnapRepository _repository;
    private readonly UserManager<ApplicationUser> _users;

    public HomeController(ILogger<HomeController> logger, IWordSnapRepository repository, UserManager<ApplicationUser> users)
    {
        _logger = logger;
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _users = users ?? throw new ArgumentNullException(nameof(users));
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SearchCardset(string searchQuery, int page = 1, int pageSize = 9)
    {
        var allCardsets = await _repository.GetCardsetsFromSearchAsync(searchQuery);

        var totalItems = allCardsets.Count();
        var pagedCardsets = allCardsets
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        ViewBag.SearchQuery = searchQuery;

        return View("SearchResults", pagedCardsets);
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCardset()
    {
        Cardset cardset = new Cardset()
        {
            UserRef = _users.GetUserId(User),
            Name = "Без назви",
            IsPublic = true,
            CreatedAt = DateTime.Now,
        };
        await _repository.AddCardsetAsync(cardset);
        return RedirectToAction("Details", "Cardset", new { cardsetId = cardset.Id });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
