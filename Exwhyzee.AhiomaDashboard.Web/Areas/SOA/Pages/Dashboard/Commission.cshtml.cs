using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.SOA.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SOA,mSuperAdmin")]

    public class CommissionModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
