using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Products
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Content,SOA,Store,mSuperAdmin,Editor")]

    public class DeleteProductImageModel : PageModel
    {
        private readonly IProductRepository _product;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public DeleteProductImageModel(IProductRepository product, AhiomaDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _product = product;
            _hostingEnv = env;
        }
        public async Task<IActionResult> OnGetAsync(long id, long pid)
        {
            try
            {
              //  var img = await _product.GetProductImgByProductId(pid);
                var img = await _context.ProductPictures.FirstOrDefaultAsync(x => x.Id == id);
                string webRootPath = _hostingEnv.WebRootPath;
                var fileName = "";
                fileName = img.PictureUrl;
                var fullPath = webRootPath + fileName;
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);

                }
                await _product.DeleteProductImg(id);
                //
               var fileNamethmb = img.PictureUrlThumbnail;
                var fullPaththum = webRootPath + fileNamethmb;
                if (System.IO.File.Exists(fullPaththum))
                {
                    System.IO.File.Delete(fullPaththum);

                }
            }
            catch (Exception e)
            {

            }
            return RedirectToPage("./Edit", new { id = pid });


        }
    }
}
