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

    [HttpPost]
    public async Task<IActionResult> CreateCardset()
    {
        Cardset cardset = new Cardset()
        {
            UserRef = 0,
            Name = "Без назви",
            IsPublic = false,
            CreatedAt = DateTime.UtcNow,
        };
        await _repository.AddCardsetAsync(cardset);
        return Ok();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
