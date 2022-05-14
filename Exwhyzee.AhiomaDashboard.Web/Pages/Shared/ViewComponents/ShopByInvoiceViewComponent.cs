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

namespace Exwhyzee.AhiomaDashboard.Web.Pages.Shared.ViewComponents
{
    public class ShopByInvoiceViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        public ShopByInvoiceViewComponent(AhiomaDbContext context, IHostingEnvironment hostingEnv)
        {
            _context = context;
            _hostingEnv = hostingEnv;
        }


        public IList<OrderItem> OrderItem { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {


            IQueryable<OrderItem> iOrderItem = from s in _context.OrderItems

              .Include(x => x.Order)
              .Include(x => x.Product)

              .Include(x => x.Product.Tenant)
              .Include(x => x.Product.Tenant.User)
              .Include(x => x.Product.Tenant.UserProfile)
              .Include(x => x.Product.ProductPictures)
              .Include(x => x.Product.Tenant.TenantAddresses)
              .Where(x => x.OrderId == id)
                                              select s;
            var orderStatus = await iOrderItem.ToListAsync();
            var order = orderStatus.Select(x => x.Product.TenantId).Distinct().ToList();

            ViewBag.ids = order;

            ViewBag.oId = id;

            return View();
        }
   


    }

}
