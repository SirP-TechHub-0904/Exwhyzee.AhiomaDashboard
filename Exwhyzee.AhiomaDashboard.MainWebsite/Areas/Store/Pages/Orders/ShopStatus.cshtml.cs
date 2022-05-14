using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Store.Pages.Orders
{
    [Authorize(Roles = "Store,mSuperAdmin")]

    public class ShopStatusModel : PageModel
    {
      
        private readonly AhiomaDbContext _context;
      

        public ShopStatusModel(AhiomaDbContext context)
        {
            _context = context;
          
        }
        [BindProperty]
       public OrderItem OrderItem { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            OrderItem = await _context.OrderItems.Include(x => x.Product.Tenant).Include(x => x.Product).Include(x=>x.Order).FirstOrDefaultAsync(x => x.Id == id);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {

                var iOrderItem = await _context.OrderItems.FirstOrDefaultAsync(x => x.Id == OrderItem.Id);
                iOrderItem.ShopStatus = OrderItem.ShopStatus;

                _context.Attach(iOrderItem).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                TempData["success"] = "success";
            }catch(Exception c)
            {
                TempData["success"] = "Pending";
            }

            return RedirectToPage("./ItemOrderList", new { id = OrderItem.Id });
        }

    }
}
