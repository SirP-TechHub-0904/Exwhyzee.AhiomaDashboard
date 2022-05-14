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
    public class PromoMobileProductItemsViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        public PromoMobileProductItemsViewComponent(AhiomaDbContext context, IHostingEnvironment hostingEnv)
        {
            _context = context;
            _hostingEnv = hostingEnv;
        }


        public IQueryable<PromoProduct> Product { get; set; }
        public IQueryable<ProductDto> NewProductList { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {

            IQueryable<PromoProduct> productIQ = from s in _context.PromoProducts
                                               .Include(p => p.PromoCategory)
               .Include(p => p.Product)

               .Include(p => p.Product.Tenant)
               .Include(p => p.Product.Tenant.Market)
               .Include(p => p.Product.ProductPictures)
               .Where(x=>x.PromoCategoryId == id)

                                                 select s;


            NewProductList = productIQ.Select(x => new ProductDto
            {
                Id = x.Product.Id,
                Name = x.Product.Name,
                Category = x.Product.Category,
                Manufacturer = x.Product.Manufacturer,
                ProductPictures = x.Product.ProductPictures,
                Market = x.Product.Tenant.Market,
                Tenant = x.Product.Tenant,
                ImageThumbnail = x.Product.ProductPictures.FirstOrDefault().PictureUrlThumbnail,
                Published = x.Product.Published,
                Price = x.Product.Price,
                OldPrice = x.Product.OldPrice,
                ShortDescription = x.Product.ShortDescription

            });
            NewProductList = NewProductList.OrderBy(a => Guid.NewGuid()).Take(12);

            ViewBag.product = NewProductList;
            return View();
        }

    }

}
