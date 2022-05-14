using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Microsoft.AspNetCore.Authorization;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Markets
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class IndexModel : PageModel
    {
        private readonly IMarketRepository _context;

        public IndexModel(IMarketRepository context)
        {
            _context = context;
        }

        public IList<Market> Market { get;set; }

        public async Task OnGetAsync()
        {
            Market = await _context.GetAsyncAll();
        }
    }
}
