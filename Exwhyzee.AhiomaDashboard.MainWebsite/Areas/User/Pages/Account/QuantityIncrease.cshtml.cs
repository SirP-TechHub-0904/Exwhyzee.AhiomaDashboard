using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    public class QuantityIncreaseModel : PageModel
    {
        private readonly AhiomaDbContext _context;

        public QuantityIncreaseModel(AhiomaDbContext context
            )
        {
            _context = context;
        }


        public async Task<IActionResult> OnGetAsync(long id, string qtype)
        {
            var productCarts = await _context.ProductCarts.FirstOrDefaultAsync(x => x.Id == id);
            if(qtype == "add")
            {
                productCarts.Quantity = productCarts.Quantity + 1;
            } 
            else if(qtype == "sub")
            {
                productCarts.Quantity = productCarts.Quantity - 1;

            }
            _context.Attach(productCarts).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("./MyCart");
        }
    }
}