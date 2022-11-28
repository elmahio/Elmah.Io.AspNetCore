using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore70.Example.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Elmah.Io.AspNetCore70.Example.Controllers
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
            // Logging can be used for breadcrumbs
            //_logger.LogInformation("Requesting the frontpage");

            // Breadcrumbs can also be added manually
            //ElmahIoApi.AddBreadcrumb(new Client.Breadcrumb(action: "Navigation", message: "Requesting the frontpage"), HttpContext);

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
                // Either use the Ship extension method
                e.Ship(HttpContext);

                // Or the Log method on ElmahIoApi
                //ElmahIoApi.Log(e, HttpContext);
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