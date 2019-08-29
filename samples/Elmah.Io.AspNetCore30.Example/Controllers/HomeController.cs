using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore30.Example.Models;

namespace Elmah.Io.AspNetCore30.Example.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            throw new Exception("Do you know what happened to the neanderthals, Bernard? We ate them.");
        }

        public IActionResult Privacy()
        {
            try
            {
                var i = 0;
                var result = 42 / i;
            }
            catch (DivideByZeroException e)
            {
                e.Ship(HttpContext);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
