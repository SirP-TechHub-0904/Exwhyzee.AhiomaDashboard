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

    public class UpdateOrderStatusModel : PageModel
    {

        private readonly AhiomaDbContext _context;


        public UpdateOrderStatusModel(AhiomaDbContext context)
        {
            _context = context;

        }
        [BindProperty]
        public Order Order { get; set; }
        [BindProperty]
        public long GetOrderId { get; set; }



        [BindProperty]
        public ShopStatus ShopStatus { get; set; }

        [BindProperty]
        public LogisticStatus LogisticStatus { get; set; }

        [BindProperty]
        public OrderStatus OrderStatus { get; set; }

        [BindProperty]
        public CustomerStatus CustomerStatus { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            Order = await _context.Orders.Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.Id == id);
            return Page();
        }


        public async Task<IActionResult> OnPostUpdateAllStatusAsync()
        {

            if (OrderStatus == OrderStatus.OutOfStock)
            {
                TempData["error"] = "cant update order with OutOfStock";
                return RedirectToPage("./UpdateOrderStatus", new { id = GetOrderId });

            }
            IQueryable<OrderItem> OrderItem = from s in _context.OrderItems

          .Where(x => x.OrderId == GetOrderId && x.Status != Enums.OrderStatus.OutOfStock)
                                              select s;


            try
            {


                var iOrder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == GetOrderId);
                iOrder.Status = OrderStatus;

                _context.Attach(iOrder).State = EntityState.Modified;

             
                foreach (var i in OrderItem)
                {
                    i.Status = OrderStatus;
                    if (OrderStatus == OrderStatus.Cancel)
                    {
                        i.ShopStatus = ShopStatus.Cancel;
                        i.LogisticStatus = LogisticStatus.Cancel;
                        i.CustomerStatus = CustomerStatus.Cancel;

                    }
                    else if (OrderStatus == OrderStatus.Completed)
                    {
                        i.ShopStatus = ShopStatus.Successful;
                        i.LogisticStatus = LogisticStatus.Successful;
                        i.CustomerStatus = CustomerStatus.Successful;
                    }
                    else
                    {
                        i.ShopStatus = ShopStatus.Pending;
                        i.LogisticStatus = LogisticStatus.Pending;
                        i.CustomerStatus = CustomerStatus.Pending;
                    }

                    _context.Entry(i).State = EntityState.Modified;

                }
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["success"] = "success updated Shop Status";
                }
                catch (Exception c)
                {
                    TempData["error"] = "failed to update Shop Status";

                }


                await _context.SaveChangesAsync();

                TempData["success"] = "success";
            }
            catch (Exception c)
            {
                TempData["success"] = "Pending";
            }
            //////
            ///
            
            //foreach (var i in OrderItem)
            //{
            //    if(OrderStatus == OrderStatus.Cancel)
            //    {
            //        i.ShopStatus = ShopStatus.Cancel;
            //        i.LogisticStatus = LogisticStatus.Cancel;
            //        i.CustomerStatus = CustomerStatus.Cancel;

            //    }
            //    else if(OrderStatus == OrderStatus.Completed)
            //    {
            //        i.ShopStatus = ShopStatus.Successful;
            //        i.LogisticStatus = LogisticStatus.Successful;
            //        i.CustomerStatus = CustomerStatus.Successful;
            //    }
            //    else
            //    {
            //        i.ShopStatus = ShopStatus.Pending;
            //        i.LogisticStatus = LogisticStatus.Pending;
            //        i.CustomerStatus = CustomerStatus.Pending;
            //    }
               
            //    _context.Entry(i).State = EntityState.Modified;

            //}
            //try
            //{
            //    await _context.SaveChangesAsync();
            //    TempData["success"] = "success updated Shop Status";
            //}
            //catch (Exception c)
            //{
            //    TempData["error"] = "failed to update Shop Status";

            //}
            return RedirectToPage("./UpdateOrderStatus", new { id = GetOrderId });
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (OrderStatus == OrderStatus.OutOfStock)
            {
                TempData["error"] = "cant update order with OutOfStock";
                return RedirectToPage("./UpdateOrderStatus", new { id = GetOrderId });

            }
            try
            {


                var iOrder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == GetOrderId);
                iOrder.Status = OrderStatus;

                _context.Attach(iOrder).State = EntityState.Modified;

                IQueryable<OrderItem> OrderItem = from s in _context.OrderItems

             .Where(x => x.OrderId == GetOrderId && x.Status != Enums.OrderStatus.OutOfStock)
                                                  select s;
                foreach (var i in OrderItem)
                {
                    i.Status = OrderStatus;
                    _context.Entry(i).State = EntityState.Modified;

                }
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["success"] = "success updated Shop Status";
                }
                catch (Exception c)
                {
                    TempData["error"] = "failed to update Shop Status";

                }


                await _context.SaveChangesAsync();

                TempData["success"] = "success";
            }
            catch (Exception c)
            {
                TempData["success"] = "Pending";
            }

            return RedirectToPage("./UpdateOrderStatus", new { id = GetOrderId });
        }

        public async Task<IActionResult> OnPostShopStatus()
        {
            IQueryable<OrderItem> OrderItem = from s in _context.OrderItems

               .Where(x => x.OrderId == GetOrderId && x.Status != Enums.OrderStatus.OutOfStock)
                                              select s;
            foreach (var i in OrderItem)
            {
                i.ShopStatus = ShopStatus;
                _context.Entry(i).State = EntityState.Modified;

            }
            try
            {
                await _context.SaveChangesAsync();
                TempData["success"] = "success updated Shop Status";
            }
            catch (Exception c)
            {
                TempData["error"] = "failed to update Shop Status";

            }

            return RedirectToPage("./UpdateOrderStatus", new { id = GetOrderId });

        }


        public async Task<IActionResult> OnPostLogisticStatus()
        {
            IQueryable<OrderItem> OrderItem = from s in _context.OrderItems

               .Where(x => x.OrderId == GetOrderId && x.Status != Enums.OrderStatus.OutOfStock)
                                              select s;
            foreach (var i in OrderItem)
            {
                i.LogisticStatus = LogisticStatus;
                _context.Entry(i).State = EntityState.Modified;

            }
            try
            {
                await _context.SaveChangesAsync();
                TempData["success"] = "success updated Logistic Status";
            }
            catch (Exception c)
            {
                TempData["error"] = "failed to update Logistic Status";

            }

            return RedirectToPage("./UpdateOrderStatus", new { id = GetOrderId });

        }

        public async Task<IActionResult> OnPostCustomerStatus()
        {
            IQueryable<OrderItem> OrderItem = from s in _context.OrderItems

               .Where(x => x.OrderId == GetOrderId && x.Status != Enums.OrderStatus.OutOfStock)
                                              select s;
            foreach (var i in OrderItem)
            {
                i.CustomerStatus = CustomerStatus;
                _context.Entry(i).State = EntityState.Modified;

            }
            try
            {
                await _context.SaveChangesAsync();
                TempData["success"] = "success updated Customer Status";
            }
            catch (Exception c)
            {
                TempData["error"] = "failed to update Customer Status";

            }

            return RedirectToPage("./UpdateOrderStatus", new { id = GetOrderId });

        }

    }
}
