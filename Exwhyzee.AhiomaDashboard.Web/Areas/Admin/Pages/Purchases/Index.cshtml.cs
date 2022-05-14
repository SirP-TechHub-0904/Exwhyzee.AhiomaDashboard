using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Purchases
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SOA,Store,mSuperAdmin")]

    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public IndexModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public IList<Purchase> Purchase { get;set; }
        public Product Product { get;set; }

        public async Task OnGetAsync(long id)
        {
            Purchase = await _context.Purchases
                .Include(p => p.Product).Where(x=>x.ProductId == id).OrderByDescending(x=>x.DateEntered).ToListAsync();

            Product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
