using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Elmah.Io.AspNetCore.Example.Net10.Pages
{
    public class IndexModel(ILogger<IndexModel> logger) : PageModel
    {
        public void OnGet()
        {
            // Logging can be used for breadcrumbs
            logger.LogInformation("Requesting the frontpage");

            // Breadcrumbs can also be added manually
            //ElmahIoApi.AddBreadcrumb(new Client.Breadcrumb(action: "Navigation", message: "Requesting the frontpage"), HttpContext);

            throw new Exception("Do you know what happened to the neanderthals, Bernard? We ate them.");
        }
    }
}
