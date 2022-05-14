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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Admin,SOA,Store,mSuperAdmin,Editor")]

    public class AddToPromoModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public AddToPromoModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public long CatId { get; set; }

        [BindProperty]
        public long? Tid { get; set; }
        [BindProperty]
        public int SortOrder { get; set; }
        [BindProperty]
        public int PageIndex { get; set; }
        [BindProperty]
        public int CurrentFilter { get; set; }
        [BindProperty]
        public string Uid { get; set; }
        [BindProperty]
        public int CurrentPage { get; set; }

        [BindProperty]
        public int Percentage { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id, long pid, int sortOrder, int pageIndex, int currentFilter, string uid, int currentPage)
        {

            Tid = id; SortOrder = sortOrder; PageIndex = pageIndex; CurrentFilter = currentFilter;
            Uid = uid; CurrentPage = CurrentPage;
           
            Product = await _context.Products.FindAsync(pid);
            if (Product == null)
            {
                return NotFound();
            }
            var data = await _context.PromoCategories.ToListAsync();
            ViewData["PromoCategoryId"] = new SelectList(data, "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var check = await _context.PromoCategories.Include(x => x.PromoProducts).FirstOrDefaultAsync(x => x.Id == CatId);
            var icheck = check.PromoProducts.FirstOrDefault(x => x.ProductId == Product.Id);


            if (icheck == null)
            {

                Product = await _context.Products.FindAsync(Product.Id);
                PromoProduct pmt = new PromoProduct();
                pmt.EndDate = DateTime.UtcNow.AddHours(1);
                pmt.StartDate = DateTime.UtcNow.AddHours(1);
                pmt.PromoCategoryId = CatId;
                pmt.ProductId = Product.Id;

                _context.PromoProducts.Add(pmt);

                var prod = await _context.Products.FindAsync(Product.Id);
                decimal cal = Convert.ToDecimal(Percentage) / Convert.ToDecimal(100);
                prod.OldPrice = prod.Price + (prod.Price * (cal));
                _context.Attach(prod).State = EntityState.Modified;


                await _context.SaveChangesAsync();
                TempData["success"] = "Added Successfully";
            }
            else
            {

                TempData["error"] = "Already Added";
                
                   
            }
            return RedirectToPage("./Index");
        }
    }
}
