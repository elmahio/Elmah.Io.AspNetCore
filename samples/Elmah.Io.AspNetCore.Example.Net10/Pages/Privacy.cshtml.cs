using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Elmah.Io.AspNetCore.Example.Net10.Pages
{
    public class PrivacyModel : PageModel
    {
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
