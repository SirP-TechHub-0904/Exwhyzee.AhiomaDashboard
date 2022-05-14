using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Dashboard.Pages.Commission
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]

    public class UpdateShopCommissionModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public UpdateShopCommissionModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tenant Shop { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Shop = await _context.Tenants
                .Include(p => p.UserProfile).FirstOrDefaultAsync(m => m.Id == id);

            if (Shop == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int take = 1, int skip = 0)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var UpdateShop = await _context.Tenants.FirstOrDefaultAsync(m => m.Id == Shop.Id);
            UpdateShop.Commission = Shop.Commission;
            _context.Attach(UpdateShop).State = EntityState.Modified;

            //var products = await _context.Products.Include(x => x.Tenant).ToListAsync();

            IQueryable<Product> products = from s in _context.Products
                                                   .Where(x => x.TenantId == Shop.Id)
                                                   .Skip(skip).Take(take)
                                           select s;

            foreach (var item in products)
            {
                var UpdateProduct = await _context.Products
               .Include(p => p.Tenant).FirstOrDefaultAsync(m => m.Id == item.Id);
                UpdateProduct.Commision = Shop.Commission;
                _context.Attach(UpdateProduct).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception s)
            {

            }
            TempData["msg"] = "success" + take + "take, " + skip+" skip";
            return RedirectToPage("./CommissionUpdate", new { id = Shop.Id });
        }

        private bool PurchaseExists(long id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}
