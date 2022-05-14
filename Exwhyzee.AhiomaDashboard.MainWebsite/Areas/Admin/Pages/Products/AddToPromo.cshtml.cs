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

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Products
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
        public async Task<IActionResult> OnGetAsync(long? id, long pid, int sortOrder, int pageIndex, int currentFilter, string uid, int CurrentPage)
        {

            Tid = id; SortOrder = sortOrder; PageIndex = pageIndex; CurrentFilter = currentFilter;
            Uid = uid; CurrentPage = CurrentPage;
            if (id == null)
            {
                return NotFound();
            }
            Product = await _context.Products.FindAsync(pid);
            if (Product == null)
            {
                return NotFound();
            }
            var data = await _context.PromoCategories.ToListAsync();
            ViewData["PromoCategoryId"] = new SelectList(data, "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products.FindAsync(Product.Id);
            PromoProduct pmt = new PromoProduct();
            pmt.EndDate = DateTime.UtcNow.AddHours(1);
            pmt.StartDate = DateTime.UtcNow.AddHours(1);
            pmt.PromoCategoryId = CatId;
            pmt.ProductId = Product.Id;

            _context.PromoProducts.Add(pmt);
            await _context.SaveChangesAsync();


            return RedirectToPage("./Index");
        }
    }
}
