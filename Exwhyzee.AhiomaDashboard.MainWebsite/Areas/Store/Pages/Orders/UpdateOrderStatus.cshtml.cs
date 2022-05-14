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

    public class UpdateOrderStatusModel : PageModel
    {
      
        private readonly AhiomaDbContext _context;
      

        public UpdateOrderStatusModel(AhiomaDbContext context)
        {
            _context = context;
          
        }
        [BindProperty]
       public Order Order { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            Order = await _context.Orders.Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.Id == id);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {

                var iOrder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == Order.Id);
                iOrder.Status = Order.Status;

                _context.Attach(iOrder).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                TempData["success"] = "success";
            }catch(Exception c)
            {
                TempData["success"] = "Pending";
            }

            return RedirectToPage("./OrderDetails", new { id = Order.Id });
        }

    }
}
