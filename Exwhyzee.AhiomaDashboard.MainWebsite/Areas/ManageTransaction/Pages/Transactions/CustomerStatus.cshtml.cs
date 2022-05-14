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

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.ManageTransaction.Pages.Transactions
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare")]

    public class CustomerStatusModel : PageModel
    {
      
        private readonly AhiomaDbContext _context;
      

        public CustomerStatusModel(AhiomaDbContext context)
        {
            _context = context;
          
        }
        [BindProperty]
       public OrderItem OrderItem { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            OrderItem = await _context.OrderItems.Include(x => x.Product.Tenant).Include(x => x.Product).Include(x=>x.Order).FirstOrDefaultAsync(x => x.Id == id);
            try
            {

                var iOrderItem = await _context.OrderItems.FirstOrDefaultAsync(x => x.Id == OrderItem.Id);
                if (iOrderItem.CustomerStatus == CustomerStatus.Pending)
                {
                    iOrderItem.CustomerStatus = CustomerStatus.Successful;
                }
                else if (iOrderItem.CustomerStatus == CustomerStatus.Cancel)
                {
                    iOrderItem.CustomerStatus = CustomerStatus.Successful;
                }
                else if (iOrderItem.CustomerStatus == CustomerStatus.None)
                {
                    iOrderItem.CustomerStatus = CustomerStatus.Successful;
                }
                else
                {
                    iOrderItem.CustomerStatus = CustomerStatus.Pending;
                }


                _context.Attach(iOrderItem).State = EntityState.Modified;
                TrackOrder itrack = new TrackOrder();
                itrack.OrderItemId = iOrderItem.Id;
                itrack.Status = "ORDER AWAITING CUSTOMER CONFIRMATION";
                itrack.Date = DateTime.UtcNow.AddHours(1);
                _context.TrackOrders.Add(itrack);

                await _context.SaveChangesAsync();

                TempData["success"] = "success";
            }
            catch (Exception c)
            {
                TempData["success"] = "Pending";
            }

            return RedirectToPage("./ItemOrderList", new { id = OrderItem.Id });
        }

    }
}
