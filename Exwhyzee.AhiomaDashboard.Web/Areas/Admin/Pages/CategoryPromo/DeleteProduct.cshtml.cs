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

    public class DeleteProductModel : PageModel
    {
        private readonly AhiomaDbContext _context;

        public DeleteProductModel(AhiomaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PromoProduct Market { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id, long cid)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = await _context.PromoProducts.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }
            _context.PromoProducts.Remove(data);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Details", new { id = cid });
        }

    }
}
