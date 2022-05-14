using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]

    public class ProductInfoModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
