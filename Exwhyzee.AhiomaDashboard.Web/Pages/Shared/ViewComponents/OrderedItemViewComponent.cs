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
    public class OrderedItemViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        public OrderedItemViewComponent(AhiomaDbContext context, IHostingEnvironment hostingEnv)
        {
            _context = context;
            _hostingEnv = hostingEnv;
        }


        public OrderItem OrderItem { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {

            OrderItem = await _context.OrderItems
                 .Include(x => x.Order)
                 .Include(x => x.Product)

                 .Include(x => x.Product.Tenant)
                 .Include(x => x.Product.Tenant.User)
                 .Include(x => x.Product.Tenant.UserProfile)
                 .Include(x => x.Product.ProductPictures)
                 .Include(x => x.Product.Tenant.TenantAddresses)
                 .FirstOrDefaultAsync(x => x.Id == id);
            return View(OrderItem);
        }
   


    }

}
