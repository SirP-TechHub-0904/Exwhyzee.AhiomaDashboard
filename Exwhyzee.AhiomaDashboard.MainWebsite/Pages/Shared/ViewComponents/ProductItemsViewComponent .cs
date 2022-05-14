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
    public class ProductItemsViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        public ProductItemsViewComponent(AhiomaDbContext context, IHostingEnvironment hostingEnv)
        {
            _context = context;
            _hostingEnv = hostingEnv;
        }


       public IQueryable<Product> Product { get; set; }
        public IQueryable<ProductDto> NewProductList { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {

            IQueryable<Product> productIQ = from s in _context.Products
                                               .Include(p => p.Category)
               .Include(p => p.Manufacturer)
               .Include(p => p.ProductPictures)
               .Include(p => p.Tenant.Market).OrderByDescending(x => x.CreatedOnUtc)
               .Where(x => x.Published == true).Where(x=>x.CategoryId == id && x.ShowOnHomePage == true)
                                            select s; 
        

            NewProductList = productIQ.Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category,
                Manufacturer = x.Manufacturer,
                ProductPictures = x.ProductPictures,
                Market = x.Tenant.Market,
                Tenant = x.Tenant,
                Quantity = x.Quantity,
                ImageThumbnail = x.ProductPictures.FirstOrDefault().PictureUrlThumbnail,
                Published = x.Published,
                Price = x.Price,
                ShortDescription = x.ShortDescription

            });
            NewProductList = NewProductList.Where(x => x.ImageThumbnail != "noimage").OrderBy(a => Guid.NewGuid()).Take(12);

            ViewBag.product = NewProductList;
            return View();
        }
        public string ValidImage(long id)
        {
            string imgpath = "noimage";
            try
            {
                var pic = _context.ProductPictures.Where(x => x.ProductId == id).ToList();
                foreach (var i in pic)
                {

                    string webRootPath = _hostingEnv.WebRootPath;
                    var fullPaththum = webRootPath + i.PictureUrlThumbnail;
                    if (System.IO.File.Exists(fullPaththum))
                    {
                        imgpath = i.PictureUrlThumbnail;
                        break;
                    }
                }
                return imgpath;
            }
            catch (Exception c)
            {
                return imgpath;
            }
        }



    }

}
