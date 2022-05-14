using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Microsoft.AspNetCore.Identity;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Categories
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class UpdateSortOrderModel : PageModel
    {
        private readonly IProductRepository _product;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IPictureService _pictureservice;
        private readonly IUserLogging _log;
        private readonly IEmailSendService _emailSender;

        public UpdateSortOrderModel(IProductRepository product, UserManager<IdentityUser> userManager, IPictureService pictureservice, AhiomaDbContext context, IHostingEnvironment env, IEmailSendService emailSender, IUserLogging log)
        {
            _context = context;
            _product = product;
            _hostingEnv = env;
            _emailSender = emailSender;
            _log = log;
            _pictureservice = pictureservice;
            _userManager = userManager;
        }


        [BindProperty]
        public Product Product { get; set; }
        [BindProperty]
        public string Colors { get; set; }
        [BindProperty]
        public string Sizes { get; set; }
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
                .Include(p => p.SubCategory)
                .Include(p => p.Manufacturer)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
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
            var user = await _userManager.GetUserAsync(User);
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
                if(Product.SubCategoryId == null)
                {
                    TempData["error"] = "Select a sub category";
                    Product = await _context.Products
        .Include(p => p.Category)
        .Include(p => p.Manufacturer)
        .FirstOrDefaultAsync(m => m.Id == Product.Id);

                    if (Product == null)
                    {
                        return NotFound();
                    }

                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", Product.CategoryId);
                    ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name", Product.ManufacturerId);
                    Pictures = await _product.GetProductPictureAsyncAll(Product.Id);
                    ProductId = Product.Id;
                    return Page();
                }
                //add colors
                try
                {
                    string[] color = Colors.Split(',');
                    foreach (var i in color)
                    {
                        ProductColor pcolor = new ProductColor();
                        pcolor.ItemColor = i;
                        pcolor.ProductId = Product.Id;
                        _context.ProductColors.Add(pcolor);

                    }
                }
                catch (Exception f)
                {
                    TempData["error1"] = "Color not in good format: product saved without color";
                    //return RedirectToPage("./Edit", new { id = id });


                }

                try
                {
                    string[] size = Sizes.Split(',');
                    foreach (var i in size)
                    {
                        ProductSize psize = new ProductSize();
                        psize.ItemSize = i;
                        psize.ProductId = Product.Id;
                        _context.ProductSizes.Add(psize);

                    }
                }
                catch (Exception f)
                {
                    TempData["error1"] = "Size not in good format: product saved without size";
                    //return RedirectToPage("./Edit", new { id = id });
                }
                _context.Attach(Product).State = EntityState.Modified;


                await _context.SaveChangesAsync();
                try
                {
                    var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

                    var url = location.AbsoluteUri;
                    var urlPath = location.Authority;
                    bool pictureupdate = false;
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
                       

                        var form = await HttpContext.Request.ReadFormAsync();
                        var httpPostedFile = form.Files.FirstOrDefault();
                        if (httpPostedFile == null)
                        {

                            TempData["error"] = "No image uploaded";


                        }
                        var fileBinary = new byte[0];
                        using (var fileStream = httpPostedFile.OpenReadStream())
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            fileBinary = fileBytes;
                        }
                        // var fileBinary = PictureService.GetDownloadBits(httpPostedFile);

                        var qqFileNameParameter = "qqfilename";
                        var fileName = httpPostedFile.FileName;
                        if (String.IsNullOrEmpty(fileName) && form.ContainsKey(qqFileNameParameter))
                            fileName = form[qqFileNameParameter].ToString();
                        //remove path (passed in IE)
                        fileName = Path.GetFileName(fileName);

                        var contentType = httpPostedFile.ContentType;

                        var fileExtension = Path.GetExtension(fileName);
                        if (!String.IsNullOrEmpty(fileExtension))
                            fileExtension = fileExtension.ToLowerInvariant();

                        //contentType is not always available 
                        //that's why we manually update it here
                        //http://www.sfsu.edu/training/mimetype.htm
                        if (String.IsNullOrEmpty(contentType))
                        {
                            switch (fileExtension)
                            {
                                case ".bmp":
                                    contentType = "image/bmp";
                                    break;
                                case ".gif":
                                    contentType = "image/gif";
                                    break;
                                case ".jpeg":
                                case ".jpg":
                                case ".jpe":
                                case ".jfif":
                                case ".pjpeg":
                                case ".pjp":
                                    contentType = "image/jpeg";
                                    break;
                                case ".png":
                                    contentType = "image/png";
                                    break;
                                case ".tiff":
                                case ".tif":
                                    contentType = "image/tiff";
                                    break;
                                default:
                                    break;
                            }
                        }

                        pictureupdate = await _pictureservice.InsertPicture(fileBinary, contentType, Product.Id, urlPath, fileExtension);
                    }
                    #endregion
                    if (pictureupdate == true)
                    {
                        var productinfo = await _context.Products.Include(x => x.Tenant.User).FirstOrDefaultAsync(x => x.Id == Product.Id);

                        TempData["success"] = "product saved";
                        try
                        {
                            var Userlog = await _context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.Id);
                            var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                            var mainurllink = urllink.AbsoluteUri;
                            var lognew = await _log.LogData(user.UserName, "", mainurllink);
                            Userlog.Logs = Userlog.Logs + "<br/>" + lognew;

                            var lognewmain = await _log.LogData(user.UserName, "productid " + Product.Id, "");
                            productinfo.Logproduct = productinfo.Logproduct + "<br/>" + lognewmain;
                            _context.Attach(productinfo).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                        }
                        catch (Exception s)
                        {

                        }

                    
                        //await _emailSender.SendToMany("onwukaemeka41@gmail.com;info@ahioma.com;ahiomainfo@gmail.com", "New Product", "Hi",
                   //$" product image updated " + productinfo.Tenant.BusinessName + " shop. Name:" + productinfo.Name);

                        return RedirectToPage("./Edit", new { id = Product.Id });
                    }
                    else if (pictureupdate == false)
                    {
                        try
                        {
                            var Userlog = await _context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.Id);
                            var lognew = await _log.LogData(user.UserName, "", "");
                            Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                            _context.Attach(Userlog).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                        }
                        catch (Exception s)
                        {

                        }
                        TempData["error"] = "The image was not uploaded is not from a valid source. kindly resave with paint and continue";
                        return RedirectToPage("./Edit", new { id = Product.Id });
                    }
                }
                catch (Exception c)
                {
                    try
                    {
                        var Userlog = await _context.UserProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.Id);
                        var lognew = await _log.LogData(user.UserName, "", "");
                        Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                        _context.Attach(Userlog).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception s)
                    {

                    }
                    TempData["success"] = "product saved with error";
                    return RedirectToPage("./Edit", new { id = Product.Id });
                }


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
            TempData["success"] = "updated successful";
            return RedirectToPage("./Edit", new { id = Product.Id });
        }

        public static void Crop(int Width, int Height, Stream streamImg, string saveFilePath)
        {
            Bitmap sourceImage = new Bitmap(streamImg);
            using (Bitmap objBitmap = new Bitmap(Width, Height))
            {
                objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                {
                    // Set the graphic format for better result cropping   
                    objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    objGraphics.DrawImage(sourceImage, 0, 0, Width, Height);

                    // Save the file path, note we use png format to support png file   
                    objBitmap.Save(saveFilePath);
                }
            }
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
            return RedirectToPage("./Edit", new { id = pid });


        }


        public JsonResult OnGetPictureDelete(long id, long pid)
        {
            return new JsonResult("");
        }
        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }

}
