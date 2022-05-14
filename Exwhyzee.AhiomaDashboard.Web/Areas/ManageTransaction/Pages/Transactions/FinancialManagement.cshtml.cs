using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    public class FinancialManagementModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly IOrderRepository _order;

        public FinancialManagementModel(AhiomaDbContext context, IOrderRepository order)
        {
            _context = context;
            _order = order;
        }

        public Order Order { get; set; }
        public IQueryable<OrderItem> OrderItems { get; set; }
        public IQueryable<Transaction> Transactions { get; set; }
        public IList<long> TenantIds { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            Order = await _context.Orders.Include(x => x.OrderItems).Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.Id == id);
            OrderItems = from s in _context.OrderItems
                                                  .Include(p => p.Product)
                                                  .Include(p => p.Product.Tenant)
                  .Where(x => x.OrderId == id && x.Status != Enums.OrderStatus.OutOfStock)
                         select s;

            Transactions = from s in _context.Transactions
                                                  
                  .Where(x => x.TrackCode == Order.TrackCode)
                         select s;

          
            var orderStatus = await OrderItems.ToListAsync();
            var tenantIds = orderStatus.Select(x => x.Product.TenantId).Distinct().ToList();

            TenantIds = tenantIds;

            return Page();
        }
        [BindProperty]
        public long GetOrderId { get; set; }
        public async Task<IActionResult> OnPostMoveToLedger()
        {
            string data = await _order.ProcessOrderToLedger(GetOrderId);
            return RedirectToPage("FinancialManagement", new { id = GetOrderId });
        }

        public async Task<IActionResult> OnPostMoveToAvailable()
        {
            string data = await _order.ProcessOrderToWithdrawable(GetOrderId);
            return RedirectToPage("FinancialManagement", new { id = GetOrderId });
        }
    }
}
