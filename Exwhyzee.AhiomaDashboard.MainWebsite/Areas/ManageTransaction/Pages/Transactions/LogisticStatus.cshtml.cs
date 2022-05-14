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

    public class LogisticStatusModel : PageModel
    {
      
        private readonly AhiomaDbContext _context;
      

        public LogisticStatusModel(AhiomaDbContext context)
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
                if (iOrderItem.LogisticStatus == LogisticStatus.Pending)
                {
                    iOrderItem.LogisticStatus = LogisticStatus.Successful;
                }
                else if (iOrderItem.LogisticStatus == LogisticStatus.Cancel)
                {
                    iOrderItem.LogisticStatus = LogisticStatus.Successful;
                }
                else if (iOrderItem.LogisticStatus == LogisticStatus.None)
                {
                    iOrderItem.LogisticStatus = LogisticStatus.Successful;
                }
                else
                {
                    iOrderItem.LogisticStatus = LogisticStatus.Pending;
                }


                _context.Attach(iOrderItem).State = EntityState.Modified;

                TrackOrder itrack = new TrackOrder();
                itrack.OrderItemId = iOrderItem.Id;
                itrack.Status = "ORDER SHIPPED";
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
