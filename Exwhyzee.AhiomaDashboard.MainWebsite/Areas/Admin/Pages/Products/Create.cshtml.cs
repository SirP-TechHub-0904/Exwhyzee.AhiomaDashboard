using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Exwhyzee.Enums;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Http;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using System.ComponentModel.DataAnnotations;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Admin,SOA,Store,mSuperAdmin,Customer,Editor,SubAdmin")]

    public class CreateModel : PageModel
    {

        private readonly IProductRepository _product;
        private readonly IPictureService _pictureservice;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICategoryRepository _category;
        private readonly IUserLogging _log;
        private readonly IEmailSendService _emailSender;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IUserProfileRepository _account;
        public CreateModel(IPictureService pictureservice, IEmailSendService emailSender, IUserLogging log, IProductRepository product, IUserProfileRepository account, UserManager<IdentityUser> userManager, IHostingEnvironment hostingEnv, AhiomaDbContext context, ICategoryRepository category)
        {
            _context = context;
            _product = product;
            _pictureservice = pictureservice;
            _category = category;
            _hostingEnv = hostingEnv;
            _emailSender = emailSender;
            _userManager = userManager;
            _account = account;
            _log = log;
        }

        public long TenantId { get; set; }
        [BindProperty]
        public string UID { get; set; }
        public async Task<IActionResult> OnGet(long tid, string uid)

        {
            UID = uid;
            var user = await _userManager.GetUserAsync(User);
            var profile = await _account.GetByUserId(user.Id);
            if (profile.Status != AccountStatus.Active)
            {
                TempData["error"] = profile.Note;
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }

            var category = await _category.GetAsyncCategoryByStoreAll(tid);
            ViewData["CategoryId"] = new SelectList(category, "CategoryId", "CategoryName");
            ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.SubCategories, "Id", "Name");
            TenantId = tid;

            return Page();
        }

        [BindProperty]
        public Product Product { get; set; }
        [BindProperty]
        public string Colors { get; set; }
        [BindProperty]
        public string Sizes { get; set; }



        [BindProperty]
        public decimal IPrice { get; set; }
        [BindProperty]
        public int IQuantity { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var tenantcommission = await _context.Tenants.FindAsync(Product.TenantId);
            Product.CreatedOnUtc = DateTime.UtcNow.AddHours(1);
            Product.UpdatedOnUtc = DateTime.UtcNow.AddHours(1);
            Product.Commision = tenantcommission.Commission;
            Product.ThirdPartyUserId = user.Id;
            //Product.Commision = 5;
            Product.Status = Enums.EntityStatus.Active;
            Product.ReOrderLevel = 50;

            var id = await _product.Insert(Product);


            //add colors
            try
            {
                string[] color = Colors.Split(',');
                foreach (var i in color)
                {
                    ProductColor pcolor = new ProductColor();
                    pcolor.ItemColor = i;
                    pcolor.ProductId = id;
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
                    psize.ProductId = id;
                    _context.ProductSizes.Add(psize);

                }
            }
            catch (Exception f)
            {
                TempData["error1"] = "Size not in good format: product saved without size";
                //return RedirectToPage("./Edit", new { id = id });
            }


            await _context.SaveChangesAsync();
            //add sizes
            //add quantity
            Purchase purchase = new Purchase();
            purchase.DateEntered = DateTime.UtcNow.AddHours(1);
            purchase.ProductId = id;
            purchase.Quantity = IQuantity;
            purchase.UnitSellingPrice = IPrice;
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            var productvalues = await _context.Products.FindAsync(purchase.ProductId);
            productvalues.Price = IPrice;
            productvalues.Quantity = IQuantity;
            _context.Attach(productvalues).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            if (id > 0)
            {
                var ProductUpdate = await _product.GetById(id);
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
                        //                       foreach (var file in files)
                        //                       {
                        //                           try
                        //                           {
                        //                               //byte[] fileData = null;
                        //                               //using (var binaryReader = new BinaryReader(HttpContext.Request..InputStream))
                        //                               //{
                        //                               //    fileData = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                        //                               //}
                        //                               //ImageConverter imageConverter = new System.Drawing.ImageConverter();
                        //                               //System.Drawing.Image image = imageConverter.ConvertFrom(fileData) as System.Drawing.Image;
                        //                               //image.Save(imageFullPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                        //                               Image image = Image.FromStream(file.OpenReadStream(), true, true);
                        //                               var newImage = new Bitmap(175, 175);
                        //                           }
                        //                           catch (ArgumentException aex)
                        //                           {
                        //                               TempData["error"] = "The image was not uploaded is not from a valid source. kindly resave with paint and continue";

                        //                               return RedirectToPage("./Edit", new { id = id });

                        //                           }
                        //                           if (file.Length > 0)
                        //                           {
                        //                               filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        //                               filePathThumbnail = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        //                               imgCount++;
                        //                               var now = DateTime.Now;
                        //                               string des = ProductUpdate.ShortDescription;

                        //                               des = des.Replace(",", " ").Replace(".", " ").Replace("+", " ").Replace("-", " ").Replace(";", " ").Replace(":", " ").Replace(">", " ").Replace("<", " ").Replace("?", " ").Replace("\"", " ").Replace("'", " ").Replace("|", " ");
                        //                               des = des.Replace(" ", "-").Replace("--", "-").Replace("-", "-");
                        //                               des = des.Replace(" ", "-").Replace("--", "-").Replace("-", "-");
                        //                               string nameproduct = des.Replace(" ", "-").Replace("--", "-").Replace("-", "-");


                        //                               var uniqueFileName = $"{now.Millisecond}{now.Minute}{now.Second}{now.Day}-".Trim() + nameproduct;
                        //                               var uniqueFileNameThumbnail = $"{now.Millisecond}{now.Minute}{now.Second}{now.Day}-".Trim() + nameproduct + "(1)";

                        //                               var fileExtension = Path.GetExtension(filePath);

                        //                               newFileName = uniqueFileName + fileExtension;
                        //                               newFileNameThumbnail = uniqueFileNameThumbnail + fileExtension;

                        //                               // if you wish to save file path to db use this filepath variable + newFileName
                        //                               var fileDbPathName = $"/Products/".Trim();

                        //                               filePath = $"{_hostingEnv.WebRootPath}{fileDbPathName}".Trim();

                        //                               if (!(Directory.Exists(filePath)))
                        //                                   Directory.CreateDirectory(filePath);

                        //                               var fileName = "";
                        //                               fileName = filePath + $"{newFileName}".Trim();


                        //                               // copy the file to the desired location from the tempMemoryLocation of IFile and flush temp memory
                        //                               Image image = Image.FromStream(file.OpenReadStream(), true, true);
                        //                               if(image.Height > 500)
                        //                               {
                        //                                   //add img main
                        //                                   try
                        //                                   {

                        //                                       FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                        //                                       // Create a byte array of file stream length
                        //                                       byte[] ImageData = new byte[fs.Length];

                        //                                       //Read block of bytes from stream into the byte array
                        //                                       fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
                        //                                       //Close the File Stream
                        //                                       fs.Close();

                        //                                       Image returnImage = null;
                        //                                       using (MemoryStream ms = new MemoryStream(ImageData))
                        //                                       {
                        //                                           returnImage = Image.FromStream(ms);

                        //                                           Bitmap sourceImage = new Bitmap(returnImage);
                        //                                           using (Bitmap objBitmap = new Bitmap(500, 500))
                        //                                           {
                        //                                               objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                        //                                               using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                        //                                               {
                        //                                                   // Set the graphic format for better result cropping   
                        //                                                   objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        //                                                   objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        //                                                   objGraphics.DrawImage(sourceImage, 0, 0, 175, 175);

                        //                                                   // Save the file path, note we use png format to support png file   
                        //                                                   objBitmap.Save(fileName, ImageFormat.Jpeg);
                        //                                               }
                        //                                           }
                        //                                       }
                        //                                   }
                        //                                   catch (ArgumentException aex)
                        //                                   {
                        //                                       TempData["error"] = "The image is not from a valid source. kindly resave with paint and continue";
                        //                                   }
                        //                               }
                        //                               else
                        //                               {
                        //using (FileStream fs = System.IO.File.Create(fileName))
                        //                               {
                        //                                   file.CopyTo(fs);
                        //                                   fs.Flush();
                        //                               }
                        //                               }


                        //                               var fileDbPathNameThumbnail = $"/Products/Thumbnail/".Trim();

                        //                               filePath = $"{_hostingEnv.WebRootPath}{fileDbPathNameThumbnail}".Trim();

                        //                               if (!(Directory.Exists(filePath)))
                        //                                   Directory.CreateDirectory(filePath);

                        //                               var fileNameThumbnail = "";
                        //newFileNameThumbnail = uniqueFileNameThumbnail + fileExtension;
                        //                               fileNameThumbnail = filePath + $"{newFileNameThumbnail}".Trim();
                        //                               //thumbnail
                        //                               try
                        //                               {

                        //                                   FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                        //                                   // Create a byte array of file stream length
                        //                                   byte[] ImageData = new byte[fs.Length];

                        //                                   //Read block of bytes from stream into the byte array
                        //                                   fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
                        //                                   //Close the File Stream
                        //                                   fs.Close();

                        //                                   Image returnImage = null;
                        //                                   using (MemoryStream ms = new MemoryStream(ImageData))
                        //                                   {
                        //                                       returnImage = Image.FromStream(ms);

                        //                                       Bitmap sourceImage = new Bitmap(returnImage);
                        //                                       using (Bitmap objBitmap = new Bitmap(175, 175))
                        //                                       {
                        //                                           objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                        //                                           using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                        //                                           {
                        //                                               // Set the graphic format for better result cropping   
                        //                                               objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        //                                               objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        //                                               objGraphics.DrawImage(sourceImage, 0, 0, 175, 175);

                        //                                               // Save the file path, note we use png format to support png file   
                        //                                               objBitmap.Save(fileNameThumbnail, ImageFormat.Jpeg);
                        //                                           }
                        //                                       }
                        //                                   }
                        //                               }
                        //                               catch (ArgumentException aex)
                        //                               {
                        //                                   TempData["error"] = "The image is not from a valid source. kindly resave with paint and continue";
                        //                               }
                        //                               // copy the file to the desired location from the tempMemoryLocation of IFile and flush temp memory
                        //                               //using (FileStream fst = System.IO.File.Create(fileNameThumbnail))
                        //                               //{
                        //                               //    file.CopyTo(fst);
                        //                               //    fst.Flush();
                        //                               //}

                        //                               #region Save Image Propertie to Db
                        //                               var img = new ProductPicture()
                        //                               {
                        //                                   ProductId = ProductUpdate.Id,
                        //                                   PictureUrl = $"{fileDbPathName}{newFileName}",
                        //                                   PicturePath = "http://" + urlPath + $"{fileDbPathName}{newFileName}",
                        //                                   //
                        //                                   PictureUrlThumbnail = $"{fileDbPathNameThumbnail}{newFileNameThumbnail}",
                        //                                   PicturePathThumbnail = "http://" + urlPath + $"{fileDbPathNameThumbnail}{newFileNameThumbnail}",

                        //                                   CreatedDateTimeUtc = DateTime.UtcNow.AddHours(1),
                        //                                   IsDefault = imgCount == 1 ? true : false,
                        //                               };
                        //                               var saveImageToDb = await _product.InsertImg(img);
                        //                               TempData["success1"] = "image saved";

                        //                               #endregion

                        //                               if (imgCount >= 5)
                        //                                   break;
                        //                           }
                        //                       }

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

                        pictureupdate = await _pictureservice.InsertPicture(fileBinary, contentType, ProductUpdate.Id, urlPath, fileExtension);
                    }
                    #endregion
                    if (pictureupdate == true)
                    {
                        TempData["success"] = "product saved";
                        //try
                        //{
                        //    var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                        //    var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                        //    var mainurllink = urllink.AbsoluteUri;
                        //    var lognew = await _log.LogData(user.UserName, "", mainurllink);
                        //    Userlog.Logs = Userlog.Logs + "<br/>" + lognew;

                        //    var lognewmain = await _log.LogData(user.UserName, "productid " + Product.Id, "");
                        //    Product.Logproduct = Product.Logproduct + "<br/>" + lognewmain;
                        //    _context.Attach(Product).State = EntityState.Modified;
                        //    await _context.SaveChangesAsync();

                        //}
                        //catch (Exception s)
                        //{

                        //}

                        // var productinfo = await _context.Products.Include(x => x.Tenant.User).FirstOrDefaultAsync(x=>x.Id == Product.Id);
                        // await _emailSender.SendToOne(productinfo.Tenant.User.Email, "New Product", "Hi",
                        // $" A new product as been added to your shop. Name:" + productinfo.Name);

                        // await _emailSender.SendToMany("onwukaemeka41@gmail.com;info@ahioma.com;ahiomainfo@gmail.com", "New Product", "Hi",
                        // $" A new product as been added to "+ productinfo.Tenant.BusinessName + " shop. Name:" + productinfo.Name);

                        return RedirectToPage("./Index", new { id = Product.TenantId, uid = UID });
                    }
                    else if (pictureupdate == false)
                    {
                        //try
                        //{
                        //    var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                        //    var lognew = await _log.LogData(user.UserName, "", "");
                        //    Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                        //    _context.Attach(Userlog).State = EntityState.Modified;
                        //    await _context.SaveChangesAsync();

                        //}
                        //catch (Exception s)
                        //{

                        //}
                        ///unpublish
                        ///
                        //var img = ValidImage(Product.Id);
                        //if (img == "noimage")
                        //{
                        //    try
                        //    {
                        //        var productupdate = await _context.Products.FirstOrDefaultAsync(x => x.Id == Product.Id);
                        //        productupdate.Published = false;
                        //        _context.Attach(productupdate).State = EntityState.Modified;


                        //        await _context.SaveChangesAsync();
                        //    }
                        //    catch (Exception c)
                        //    {

                        //    }
                        //}
                        TempData["error"] = "The image was not uploaded is not from a valid source. kindly resave with paint and continue";
                        return RedirectToPage("./Edit", new { id = id });

                    }
                }
                catch (Exception c)
                {
                    //try
                    //{
                    //    var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    //    var lognew = await _log.LogData(user.UserName, "", "");
                    //    Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                    //    _context.Attach(Userlog).State = EntityState.Modified;
                    //    await _context.SaveChangesAsync();

                    //}
                    //catch (Exception s)
                    //{

                    //}
                    TempData["success"] = "product saved with error";
                    return RedirectToPage("./Edit", new { id = id });
                }

            }

            return Page();
        }

        public string ValidImage(long id)
        {
            string imgpath = "noimage";
            try
            {
                // var pic = _context.ProductPictures.Where(x => x.ProductId == id).ToList();

                IQueryable<ProductPicture> pic = from s in _context.ProductPictures
                                                 .Where(x => x.ProductId == id)
                                                 select s;

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
