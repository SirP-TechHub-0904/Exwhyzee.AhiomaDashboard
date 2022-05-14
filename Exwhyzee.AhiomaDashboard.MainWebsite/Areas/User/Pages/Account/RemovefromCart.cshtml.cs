using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    public class RemovefromCartModel : PageModel
    {
        private readonly AhiomaDbContext _context;

        public RemovefromCartModel(AhiomaDbContext context
            )
        {
            _context = context;
        }


        public async Task<IActionResult> OnGetAsync(long id)
        {
            var productCarts = await _context.ProductCarts.FirstOrDefaultAsync(x => x.Id == id);
         
            if (productCarts != null)
            {
                _context.ProductCarts.Remove(productCarts);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./MyCart");
        }
    }

}
