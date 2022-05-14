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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.CategoryPromo
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class DeleteModel : PageModel
    {
        private readonly AhiomaDbContext _context;

        public DeleteModel(AhiomaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PromoCategory Market { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Market = await _context.PromoCategories.FindAsync(id);

            if (Market == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = await _context.PromoCategories.FindAsync(id);
            _context.PromoCategories.Remove(data);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
