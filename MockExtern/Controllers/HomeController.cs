using Microsoft.AspNetCore.Mvc;
using MockExtern.Models;
using MockExtern.Services;
using System.Diagnostics;

namespace MockExtern.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIntegrationService commsService;

        public HomeController(ILogger<HomeController> logger, IIntegrationService commsService)
        {
            _logger = logger;
            this.commsService = commsService;
        }

        public async Task<IActionResult> Index()
        {
            await commsService.Run();

            return View();
        }

        public IActionResult Privacy()
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