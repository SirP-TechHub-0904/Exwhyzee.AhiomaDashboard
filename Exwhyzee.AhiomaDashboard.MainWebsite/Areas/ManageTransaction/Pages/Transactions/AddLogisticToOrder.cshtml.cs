using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.ManageTransaction.Pages.Transactions
{
    public class AddLogisticToOrderModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public AddLogisticToOrderModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public long? OrderId { get; set; }
      
        [BindProperty]
        public long VehicleId { get; set; }
        public List<SelectListItem> LogisticListing { get; set; }
        public Order Order { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Order = await _context.Orders
                .Include(x => x.OrderItems)
                .Include(x => x.LogisticVehicle)
                .Include(x => x.Transaction)
                .Include(x => x.UserProfile.User)
                .Include(x => x.LogisticVehicle.LogisticProfile)
                .Include(x => x.UserProfile)
                .Include(x => x.UserProfile.UserAddresses)
                .FirstOrDefaultAsync(x => x.Id == id);
            OrderId = id;
            var logistic = await _context.LogisticProfiles.Where(x => x.LogisticStatus == Enums.LogisticEnum.Active).ToListAsync();
            LogisticListing = logistic.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id.ToString(),
                                      Text = a.CompanyName
                                  }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var orderupdate = await _context.Orders.FirstOrDefaultAsync(x => x.Id == OrderId);
            orderupdate.LogisticVehicleId = VehicleId;
            _context.Attach(orderupdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception s)
            {
               
            }

            return RedirectToPage("./OrderDetails", new { id = orderupdate.Id });
        }

    }
}
