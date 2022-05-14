using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.WebBanner
{
    [Authorize(Roles = "Content,mSuperAdmin")]

    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public DetailsModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public Banner Banner { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Banner = await _context.Banners.FirstOrDefaultAsync(m => m.Id == id);

            if (Banner == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
