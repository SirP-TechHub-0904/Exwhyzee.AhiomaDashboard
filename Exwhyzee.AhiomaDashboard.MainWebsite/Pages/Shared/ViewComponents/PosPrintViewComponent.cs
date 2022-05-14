using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class PosPrintViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        public PosPrintViewComponent(AhiomaDbContext context, IHostingEnvironment hostingEnv)
        {
            _context = context;
            _hostingEnv = hostingEnv;
        }



        public IQueryable<OrderItem> OrderItem { get; set; }
        public IList<OrderItem> OrderItems { get; set; } 
        public IList<OrderItem> ReversedOrderItems { get; set; }
        public Order Order { get; set; }
        public async Task<IViewComponentResult> InvokeAsync(long id, long tid)
        {
            Order = await _context.Orders
                .Include(x => x.LogisticVehicle)
                .Include(x => x.OrderItems)
                .Include(x => x.OrderItems)
                .Include(x => x.UserProfile)
                .Include(x => x.UserProfile.User)
                .Include(x => x.UserProfile.UserAddresses)
                .FirstOrDefaultAsync(x => x.Id == id);

            IQueryable<OrderItem> OrderItem = from s in _context.OrderItems

              .Include(x => x.Order)
              .Include(x => x.Product)

              .Include(x => x.Product.Tenant)
              .Include(x => x.Product.Tenant.User)
              .Include(x => x.Product.Tenant.UserProfile)
              .Include(x => x.Product.ProductPictures)
              .Include(x => x.Product.Tenant.TenantAddresses)
              .Where(x => x.OrderId == id && x.Product.TenantId == tid)
                                              select s;
            OrderItems = await OrderItem.Where(x => x.Status != Enums.OrderStatus.Reversed).ToListAsync();
            ReversedOrderItems = await OrderItem.Where(x => x.Status == Enums.OrderStatus.Reversed).ToListAsync();

            ViewBag.Order = Order;
            ViewBag.OrderItems = OrderItems;
            return View();
        }
   


    }

}
