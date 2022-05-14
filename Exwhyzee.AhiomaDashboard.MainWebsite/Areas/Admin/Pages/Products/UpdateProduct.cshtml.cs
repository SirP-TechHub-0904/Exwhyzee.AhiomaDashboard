using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Products
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SOA,Store,mSuperAdmin,Editor")]

    public class UpdateProductModel : PageModel
    {
        private readonly IProductRepository _product;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public UpdateProductModel(IProductRepository product, AhiomaDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _product = product;
            _hostingEnv = env;
        }


        [BindProperty]
        public Product Product { get; set; }

        public IList<ProductPicture> Pictures { get; set; }

        [BindProperty]
        public long ProductId { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Product == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", Product.CategoryId);
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", Product.ManufacturerId);
            Pictures = await _product.GetProductPictureAsyncAll(id);
            ProductId = Product.Id;
            return Page();
        }

       
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
                var message = string.Join(" | ", ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage));
                return Page();
            }


            //_context.Attach(Product).State = EntityState.Modified;

            try
            {
                var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

                var url = location.AbsoluteUri;
                var urlPath = location.Authority;
                #region product Image(s)
                int imgCount = 0;
                if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count > 0)
                {
                    var newFileName = string.Empty;
                    var newFileNameThumbnail = string.Empty;
                    var filePath = string.Empty;
                    var filePathThumbnail = string.Empty;
                    string pathdb = string.Empty;
                    var files = HttpContext.Request.Form.Files;
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            filePathThumbnail = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            imgCount++;
                            var now = DateTime.Now;
                            string nameproduct = Product.ShortDescription.Replace(" ", "-");
                            var uniqueFileName = $"{now.Year}{now.Month}{now.Day}-".Trim() + nameproduct;

                            var fileExtension = Path.GetExtension(filePath);

                            newFileName = uniqueFileName + fileExtension;
                            newFileNameThumbnail = uniqueFileName + fileExtension;

                            // if you wish to save file path to db use this filepath variable + newFileName
                            var fileDbPathName = $"/Products/".Trim();

                            filePath = $"{_hostingEnv.WebRootPath}{fileDbPathName}".Trim();

                            if (!(Directory.Exists(filePath)))
                                Directory.CreateDirectory(filePath);

                            var fileName = "";
                            fileName = filePath + $"{newFileName}".Trim();


                            // copy the file to the desired location from the tempMemoryLocation of IFile and flush temp memory
                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                            var fileDbPathNameThumbnail = $"/Products/Thumbnail".Trim();

                            filePath = $"{_hostingEnv.WebRootPath}{fileDbPathNameThumbnail}".Trim();

                            if (!(Directory.Exists(filePath)))
                                Directory.CreateDirectory(filePath);

                            var fileNameThumbnail = "";
                            fileNameThumbnail = filePath + $"{newFileName}".Trim();
                            //thumbnail
                            Image image = Image.FromStream(file.OpenReadStream(), true, true);
                            var newImage = new Bitmap(200, 200);
                            using (var g = Graphics.FromImage(newImage))
                            {
                                g.DrawImage(image, 0, 0, 200, 200);
                                newImage.Save(fileNameThumbnail, ImageFormat.Png);

                            }

                            // copy the file to the desired location from the tempMemoryLocation of IFile and flush temp memory
                            //using (FileStream fst = System.IO.File.Create(fileNameThumbnail))
                            //{
                            //    file.CopyTo(fst);
                            //    fst.Flush();
                            //}

                            #region Save Image Propertie to Db
                            var img = new ProductPicture()
                            {
                                ProductId = Product.Id,
                                PictureUrl = $"{fileDbPathName}/{newFileName}",
                                PicturePath = "http://" + urlPath + $"{fileDbPathName}/{newFileName}",
                                //
                                PictureUrlThumbnail = $"{fileDbPathNameThumbnail}/{newFileName}",
                                PicturePathThumbnail = "http://" + urlPath + $"{fileDbPathNameThumbnail}/{newFileName}",

                                CreatedDateTimeUtc = DateTime.UtcNow.AddHours(1),
                                IsDefault = imgCount == 1 ? true : false,
                            };
                            var saveImageToDb = await _product.InsertImg(img);

                            #endregion

                            if (imgCount >= 5)
                                break;
                        }
                    }
                }
                #endregion

                await _product.Update(Product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./UpdateProduct", new { id = Product.Id });
        }

        public async Task<IActionResult> OnPostPictureDelete(long id, long pid)
        {
            try
            {
                await _product.DeleteProductImg(id);

            }
            catch (Exception e)
            {

            }
            return RedirectToPage("./UpdateProduct", new { id = pid });


        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
