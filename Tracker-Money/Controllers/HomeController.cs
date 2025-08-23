using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tracker_Money.Models;
using Tracker_Money.Services;

namespace Tracker_Money.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILanguageService _languageService;

        public HomeController(ILogger<HomeController> logger, ILanguageService languageService)
        {
            _logger = logger;
            _languageService = languageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Localization()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
