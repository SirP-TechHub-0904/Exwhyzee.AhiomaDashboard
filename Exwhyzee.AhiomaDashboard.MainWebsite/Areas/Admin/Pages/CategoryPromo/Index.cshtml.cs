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

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.CategoryPromo
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class IndexModel : PageModel
    {
        private readonly AhiomaDbContext _context;

        public IndexModel(AhiomaDbContext context)
        {
            _context = context;
        }

        public IList<PromoCategory> Market { get;set; }

        public async Task OnGetAsync()
        {
            Market = await _context.PromoCategories.ToListAsync();
        }
    }
}
