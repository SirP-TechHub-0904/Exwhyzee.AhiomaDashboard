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

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.SOA.Pages.Orders
{
    [Authorize(Roles = "mSuperAdmin,SOA")]

    public class CompletedOrdersModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CompletedOrdersModel(AhiomaDbContext context,
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
            Customer = String.IsNullOrEmpty(sortOrder) ? "customer_desc" : "";
            OrderId = String.IsNullOrEmpty(sortOrder) ? "orderid_desc" : "";
            Amount = String.IsNullOrEmpty(sortOrder) ? "amount_desc" : "";
            Paid = String.IsNullOrEmpty(sortOrder) ? "paid_desc" : "";
            Logistic = String.IsNullOrEmpty(sortOrder) ? "logistic_desc" : "";
            Status = String.IsNullOrEmpty(sortOrder) ? "status_desc" : "";
            Date = sortOrder == "Date" ? "date_desc" : "Date";

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
            var user = await _userManager.GetUserAsync(User);
            IQueryable<OrderItem> iOrders = from s in _context.OrderItems
                                      .Include(x => x.Product.Tenant)
                                      .Include(x => x.Product.ProductPictures)
                                   .Where(x => x.Product.Tenant.CreationUserId == user.Id)
                                            select s;

            var orderids = iOrders.Select(x => x.OrderId).Distinct();
            IQueryable<Order> iOrderlist = from s in _context.Orders
                                           .Include(x => x.UserProfile)
                                           .Include(x => x.OrderItems)
                                           .Include(x => x.LogisticVehicle)
                                           .Include(x => x.UserProfile.User)
                                           .Where(x => orderids.Contains(x.Id))
                                            .OrderByDescending(x => x.DateOfOrder)
                                           select s;

            var iOrder = iOrderlist.Where(x => x.Status == Enums.OrderStatus.Completed);

            AwaitingOrders = iOrderlist.Where(x => x.Status == Enums.OrderStatus.Processing).Count();
            CompletedOrders = iOrderlist.Where(x => x.Status == Enums.OrderStatus.Completed).Count();
            CancelOrders = iOrderlist.Where(x => x.Status == Enums.OrderStatus.Cancel).Count();
            AllOrders = iOrderlist.Count();
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

            switch (sortOrder)
            {

                case "Date":
                    iOrder = iOrder.OrderBy(s => s.DateOfOrder);
                    break;

                case "date_desc":
                    iOrder = iOrder.OrderByDescending(s => s.DateOfOrder);
                    break;

                case "Customer":
                    iOrder = iOrder.OrderBy(s => s.UserProfile.Fullname);
                    break;

                case "customer_desc":
                    iOrder = iOrder.OrderByDescending(s => s.UserProfile.Fullname);
                    break;

                case "OrderId":
                    iOrder = iOrder.OrderBy(s => s.OrderId);
                    break;

                case "orderId_desc":
                    iOrder = iOrder.OrderByDescending(s => s.OrderId);
                    break;

                default:
                    iOrder = iOrder.OrderByDescending(s => s.DateOfOrder);
                    break;
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
