using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Products
{
    public class IProductDModel : PageModel
    {
        public readonly AhiomaDbContext _context;

        public IProductDModel(AhiomaDbContext context)
        {
            _context = context;
        }

        public Product Product { get; set; }

        [BindProperty]
        public long ProductId { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id = 0)
        {
            if (id != null)
            {
                Product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Tenant)
                .Include(p => p.Manufacturer).FirstOrDefaultAsync(m => m.Id == id);

            }

            return Page();
        }
        [BindProperty]
        public long GetPId { get; set; }
        public async Task<IActionResult> OnPostGProductD()
        {
            try
            {

                Product = await _context.Products.FindAsync(GetPId);

                if (Product != null)
                {
                    return RedirectToPage("IProductD", new { id = Product.Id });
                }

                return RedirectToPage();
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostProductD()
        {
            try
            {

                Product = await _context.Products.FindAsync(ProductId);

                if (Product != null)
                {
                    _context.Products.Remove(Product);
                    await _context.SaveChangesAsync();
                }
                TempData["success"] = "Updated Successfully";
                return RedirectToPage();
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage();
            }
        }

    }
}
