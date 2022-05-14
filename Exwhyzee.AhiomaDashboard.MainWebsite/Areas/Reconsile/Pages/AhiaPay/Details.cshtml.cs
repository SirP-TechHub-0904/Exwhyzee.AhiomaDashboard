using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Reconsile.Pages.AhiaPay
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin")]

    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public DetailsModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public Wallet Wallet { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Wallet = await _context.Wallets.FirstOrDefaultAsync(m => m.Id == id);

            if (Wallet == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
