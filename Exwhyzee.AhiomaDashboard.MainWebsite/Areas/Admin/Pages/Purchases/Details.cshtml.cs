using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Purchases
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SOA,Store,mSuperAdmin")]

    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public DetailsModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public Purchase Purchase { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Purchase = await _context.Purchases
                .Include(p => p.Product).FirstOrDefaultAsync(m => m.Id == id);

            if (Purchase == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
