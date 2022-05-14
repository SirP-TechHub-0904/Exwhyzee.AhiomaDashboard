using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
