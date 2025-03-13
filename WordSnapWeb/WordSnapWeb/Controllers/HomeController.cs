using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WordSnapWeb.Models;

namespace WordSnapWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWordSnapRepository _repository;

    public HomeController(ILogger<HomeController> logger, IWordSnapRepository repository)
    {
        _logger = logger;
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
    public async Task<IActionResult> SearchCardset(string searchQuery)
    {
        var cardsets = await _repository.GetCardsetsFromSearchAsync(searchQuery);
        return View("SearchResults", cardsets);
    }


    [HttpPost]
    public async Task<IActionResult> CreateCardset()
    {
        Cardset cardset = new Cardset()
        {
            UserRef = 82,
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
