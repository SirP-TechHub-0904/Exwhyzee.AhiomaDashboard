using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.WebBanner
{
    [Authorize(Roles = "Content,mSuperAdmin")]

    public class CreateModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public CreateModel(AhiomaDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _hostingEnv = env;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Banner Banner { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

                var url = location.AbsoluteUri;
                var urlPath = location.Authority;
                #region Image(s)
                int imgCount = 0;
                if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count > 0)
                {
                    var newFileName = string.Empty;
                    var filePath = string.Empty;
                    string pathdb = string.Empty;
                    var file = HttpContext.Request.Form.Files.FirstOrDefault();
                   
                        if (file.Length > 0)
                        {
                        //
                        //Image imgg = Image.FromFile(file.FileName);
                        //    Bitmap img = new Bitmap(imgg);

                        //    var imageHeight = img.Height;
                        //    var imageWidth = img.Width;
                        //if(Banner.BannerType == Enums.BannerType.Washington)
                        //{
                        //    if(file.Length > 300 || imageHeight < 300)
                        //    {
                        //        TempData["error"] = "image must be 750 by 300px";
                        //        return Page();
                        //    }
                        //    if (imageWidth > 750 || imageWidth < 750)
                        //    {
                        //        TempData["error"] = "image must be 750 by 300px";
                        //        return Page();
                        //    }
                        //}

                            filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            imgCount++;
                            var now = DateTime.Now;

                            var fileExtension = Path.GetExtension(filePath);

                            newFileName = $"{now.Minute}{now.Month}{now.Day}-".Trim()+ file.FileName;

                            // if you wish to save file path to db use this filepath variable + newFileName
                            var fileDbPathName = $"/Banner/".Trim();

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


                        var link = Url.Page(
                "/Index",
                pageHandler: null,
                values: new { area = "" },
                protocol: Request.Scheme);
                        #region Save Image Propertie to Db

                        Banner.UrlPath = $"{fileDbPathName}{newFileName}";
                            Banner.UrlLink = link + $"{fileDbPathName}{newFileName}";

                            Banner.DateUtc = DateTime.UtcNow.AddHours(1);
                           
                            _context.Banners.Add(Banner);
                            await _context.SaveChangesAsync();
                        };
                       
                        #endregion

                    
                }
            
                #endregion

        }
            catch (Exception f)
            {

            }
            

            return RedirectToPage("./Index");
}
    }
}
