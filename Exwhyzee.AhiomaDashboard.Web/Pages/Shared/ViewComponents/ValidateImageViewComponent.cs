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
    public class ValidateImageViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        public ValidateImageViewComponent(AhiomaDbContext context, IHostingEnvironment hostingEnv)
        {
            _context = context;
            _hostingEnv = hostingEnv;
        }


       public IList<Product> Product { get; set; }
        public IList<ProductDto> NewProductList { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            Product = await _context.Products.Where(x=>x.CategoryId == id && x.ShowOnHomePage == true)
       .Include(p => p.Category)
       .Include(p => p.Manufacturer)
       .Include(p => p.ProductPictures)
       .Include(p => p.Tenant.Market).OrderByDescending(x => x.CreatedOnUtc)
       .Where(x => x.Published == true)
       .ToListAsync();

            NewProductList = Product.Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category,
                Manufacturer = x.Manufacturer,
                ProductPictures = x.ProductPictures,
                Market = x.Tenant.Market,
                Tenant = x.Tenant,
                ImageThumbnail = ValidImage(x.Id),
                Published = x.Published,
                Price = x.Price,
                ShortDescription = x.ShortDescription

            }).ToList();
            NewProductList = NewProductList.Where(x => x.ImageThumbnail != "noimage").OrderBy(a => Guid.NewGuid()).Take(6).ToList();

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
