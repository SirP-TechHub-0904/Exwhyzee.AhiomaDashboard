using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Settings
{
    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public DetailsModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public Setting Setting { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
          

            Setting = await _context.Settings.FirstOrDefaultAsync();

            if (Setting == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
