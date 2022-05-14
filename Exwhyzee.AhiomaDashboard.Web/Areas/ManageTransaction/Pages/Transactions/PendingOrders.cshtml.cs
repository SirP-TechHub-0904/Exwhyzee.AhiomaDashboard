using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    [Authorize(Roles = "Admin,mSuperAdmin,CustomerCare")]

    public class PendingOrdersModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PendingOrdersModel(AhiomaDbContext context,
            UserManager<IdentityUser> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }
        public PaginatedList<Order> OrderItem { get; set; }

        public int AwaitingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelOrders { get; set; }
        public int AllOrders { get; set; }
        public int Pending { get; set; }
        public int Reversed { get; set; }
        public int Count { get; set; }
        public long Oid { get; set; }

        public int PageSize { get; set; }
        public int? CurrentPage { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public string Date { get; set; }
        public string Customer { get; set; }
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string Paid { get; set; }
        public string Logistic { get; set; }
        public string Status { get; set; }
        public long? OId { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id, string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            OId = id;
            
            CurrentFilter = searchString;
            CurrentSort = sortOrder;

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            IQueryable<Order> iOrders = from s in _context.Orders
                                                .Include(p => p.LogisticVehicle)
                .Include(p => p.OrderItems)
                .Include(p => p.UserProfile)
                .OrderByDescending(x => x.DateOfOrder)
                                        select s;

            var iOrder = iOrders.Where(x => x.Status == Enums.OrderStatus.Pending);


            AwaitingOrders = iOrders.Where(x => x.Status == Enums.OrderStatus.Processing).Count();
            CompletedOrders = iOrders.Where(x => x.Status == Enums.OrderStatus.Completed).Count();
            CancelOrders = iOrders.Where(x => x.Status == Enums.OrderStatus.Cancel).Count();
            AllOrders = iOrders.Count();
            Pending = iOrders.Where(x => x.Status == Enums.OrderStatus.Pending).Count();
            Reversed = iOrders.Where(x => x.Status == Enums.OrderStatus.Reversed).Count();

            if (!String.IsNullOrEmpty(searchString))
            {

                iOrder = iOrder.Where(s => (s.OrderId != null) && s.OrderId.ToUpper().Contains(searchString)

                    || (s.AmountPaid.ToString() != null) && s.AmountPaid.ToString().ToUpper().Contains(searchString)
                    || (s.OrderAmount.ToString() != null) && s.OrderAmount.ToString().ToUpper().Contains(searchString)
                    || (s.LogisticAmount.ToString() != null) && s.LogisticAmount.ToString().ToUpper().Contains(searchString)

                     || (s.UserProfile.Fullname != null) && s.UserProfile.Fullname.ToUpper().Contains(searchString)
                     || (s.UserProfile.IdNumber.ToString() != null) && s.UserProfile.IdNumber.ToString().ToUpper().Contains(searchString)
                     || (s.DateOfOrder != null) && s.DateOfOrder.ToString().ToUpper().Contains(searchString)
                    );

            }

            Count = iOrder.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;
            CurrentPage = pageIndex;
            OrderItem = await PaginatedList<Order>.CreateAsync(
                iOrder.AsNoTracking(), pageIndex ?? 1, pageSize);


            return Page();
        }
    }
}
