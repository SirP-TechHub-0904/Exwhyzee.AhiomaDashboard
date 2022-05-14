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
    public class ThirdPartyCountViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        public ThirdPartyCountViewComponent(AhiomaDbContext context, IHostingEnvironment hostingEnv)
        {
            _context = context;
            _hostingEnv = hostingEnv;
        }


       public int Product { get; set; }
        public IList<ProductDto> NewProductList { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(string id, long tid)
        {
            IQueryable<Product> productIQ = from s in _context.Products
                                                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(x => x.ProductPictures)
                .Include(x => x.Tenant.UserProfile)
                .Include(x => x.Tenant).Where(x => x.ThirdPartyUserId == id && x.TenantId == tid)
                                            select s;

            Product = await productIQ.CountAsync();

            ViewBag.count = Product;
          
            return View();
        }
      

    }

}
