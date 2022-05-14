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

    public class DetailsModel : PageModel
    {
        private readonly AhiomaDbContext _context;

        public DetailsModel(AhiomaDbContext context)
        {
            _context = context;
        }

        public PromoCategory Market { get; set; }
        public IQueryable<PromoProduct> PromoProducts { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Market = await _context.PromoCategories.Include(x=>x.PromoProducts).FirstOrDefaultAsync(x=>x.Id == id);

            IQueryable<PromoProduct> iPromo = from s in _context.PromoProducts                                              
               .Include(x => x.Product)
               .Where(x => x.PromoCategoryId == id)
                                            select s;

            PromoProducts = iPromo;
            if (Market == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
