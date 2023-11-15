using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Elmah.Io.AspNetCore80.Example.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            try
            {
                var i = 0;
                var result = 42 / i;
            }
            catch (DivideByZeroException e)
            {
                e.Ship(HttpContext);

                // Or the Log method on ElmahIoApi
                //ElmahIoApi.Log(e, HttpContext);
            }
        }
    }

}
