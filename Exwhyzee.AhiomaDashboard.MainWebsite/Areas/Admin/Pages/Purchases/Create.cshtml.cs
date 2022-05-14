using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Purchases
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SOA,Store,mSuperAdmin")]

    public class CreateModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public CreateModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [BindProperty]
        public long ProductId { get; set; }
        [BindProperty]
        public long ProductItemId { get; set; }
        public IActionResult OnGet(long id)
        {
            ProductId = id;
            return Page();
        }

        [BindProperty]
        public Purchase Purchase { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
            Purchase.ProductId = ProductItemId;
            Purchase.DateEntered = DateTime.UtcNow.AddHours(1);
            _context.Purchases.Add(Purchase);

           
            await _context.SaveChangesAsync();
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == Purchase.ProductId);
            product.Price = Purchase.UnitSellingPrice;
            product.Quantity = product.Quantity + Purchase.Quantity;
            product.Logproduct = product.Logproduct + "<br/> product purchase updated with purchase id " + Purchase.Id;

            _context.Attach(product).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new { id = Purchase.ProductId });
        }
    }
}
