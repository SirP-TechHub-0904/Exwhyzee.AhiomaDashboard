using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare")]

    public class UpdateItemStatusModel : PageModel
    {
      
        private readonly AhiomaDbContext _context;
      

        public UpdateItemStatusModel(AhiomaDbContext context)
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
                iOrderItem.Status = OrderItem.Status;

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
