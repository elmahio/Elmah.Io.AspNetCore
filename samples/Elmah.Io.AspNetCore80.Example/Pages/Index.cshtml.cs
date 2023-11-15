using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Elmah.Io.AspNetCore80.Example.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Logging can be used for breadcrumbs
            _logger.LogInformation("Requesting the frontpage");

            // Breadcrumbs can also be added manually
            //ElmahIoApi.AddBreadcrumb(new Client.Breadcrumb(action: "Navigation", message: "Requesting the frontpage"), HttpContext);

            throw new Exception("Do you know what happened to the neanderthals, Bernard? We ate them.");
        }
    }
}
