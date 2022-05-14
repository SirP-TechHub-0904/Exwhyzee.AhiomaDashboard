using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages
{
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        [BindProperty]
        public string CustomerRef { get; set; }
        public void OnGet(string customerRef)
        {
            CustomerRef = customerRef;
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
