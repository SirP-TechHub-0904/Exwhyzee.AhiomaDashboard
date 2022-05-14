using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.Data.Repository.TenantAddresses;
using Exwhyzee.AhiomaDashboard.Data.Repository.TenantSocialMedias;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.Data.Dtos;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "Admin,Store,SOA,mSuperAdmin,CustomerCare,Logistic")]

    public class LogisticInfoModel : PageModel
    {

        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly UserManager<IdentityUser> _userManager;
        public LogisticInfoModel(AhiomaDbContext context, IHostingEnvironment env,
            UserManager<IdentityUser> userManager)
        {
         
            _context = context;
            _hostingEnv = env;
            _userManager = userManager;
        }
       


        public LogisticProfile Logistic { get; set; }
        public int Count { get; set; }
        public IList<LogisticVehicle> LogisticVehicle { get; set; }
      

        public async Task<IActionResult> OnGetAsync(long? id)
        {

            Logistic = await _context.LogisticProfiles.Include(x => x.UserProfile).Include(x => x.UserProfile.User).Include(x=>x.LogisticVehicles).FirstOrDefaultAsync(x => x.Id == id);
                return Page();
        }

        public async Task<IActionResult> OnPostLogoUpload(long id)
        {
            var tenants = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == id);
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
                    var filePath = string.Empty;
                    string pathdb = string.Empty;
                    var files = HttpContext.Request.Form.Files;
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            imgCount++;
                            var now = DateTime.Now;

                            var fileExtension = Path.GetExtension(filePath);

                            newFileName = tenants.TenentHandle.Replace(".", "").Replace("/","")+"-Logo-"+ $"{now.Minute}{now.Month}{now.Day}".Trim()+ fileExtension;

                            // if you wish to save file path to db use this filepath variable + newFileName
                            var fileDbPathName = $"/Banner/".Trim();

                            filePath = $"{_hostingEnv.WebRootPath}{fileDbPathName}".Trim();

                            if (!(Directory.Exists(filePath)))
                                Directory.CreateDirectory(filePath);

                            var fileName = "";
                            fileName = filePath + $"{newFileName}".Trim();

                            //delete
                            string webRootPath = _hostingEnv.WebRootPath;
                            var fileNamedelete = "";
                            fileNamedelete = tenants.LogoUri;
                            var fullPathdelete = webRootPath + fileNamedelete;
                            if (System.IO.File.Exists(fullPathdelete))
                            {
                                System.IO.File.Delete(fullPathdelete);

                            }
                            // copy the file to the desired location from the tempMemoryLocation of IFile and flush temp memory
                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }



                            #region Save Image Propertie to Db

                            tenants.LogoUri = $"{fileDbPathName}/{newFileName}";
                            _context.Attach(tenants).State = EntityState.Modified;

                            await _context.SaveChangesAsync();
                        };

                        #endregion

                        if (imgCount >= 2)
                            break;
                    }
                }

                #endregion



            }
            catch (Exception e)
            {

            }
            return RedirectToPage("./StoreInfo", new { id = id });
        }

        public async Task<IActionResult> OnPostBannerUpload(long id)
        {
            var tenants = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == id);

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
                    var filePath = string.Empty;
                    string pathdb = string.Empty;
                    var files = HttpContext.Request.Form.Files;
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            imgCount++;
                            var now = DateTime.Now;

                            var fileExtension = Path.GetExtension(filePath);

                            newFileName = tenants.TenentHandle.Replace(".", "").Replace("/", "") + "-Banner-" + $"{now.Minute}{now.Month}{now.Day}".Trim()+fileExtension;

                            // if you wish to save file path to db use this filepath variable + newFileName
                            var fileDbPathName = $"/Banner/".Trim();

                            filePath = $"{_hostingEnv.WebRootPath}{fileDbPathName}".Trim();

                            if (!(Directory.Exists(filePath)))
                                Directory.CreateDirectory(filePath);

                            var fileName = "";
                            fileName = filePath + $"{newFileName}".Trim();


                            //delete
                            string webRootPath = _hostingEnv.WebRootPath;
                            var fileNamedelete = "";
                            fileNamedelete = tenants.BannerUri;
                            var fullPathdelete = webRootPath + fileNamedelete;
                            if (System.IO.File.Exists(fullPathdelete))
                            {
                                System.IO.File.Delete(fullPathdelete);

                            }
                            // copy the file to the desired location from the tempMemoryLocation of IFile and flush temp memory
                            using (FileStream fs = System.IO.File.Create(fileName))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }



                            #region Save Image Propertie to Db

                            tenants.BannerUri = $"{fileDbPathName}/{newFileName}";
                            _context.Attach(tenants).State = EntityState.Modified;

                                await _context.SaveChangesAsync();
                               
                        };

                        #endregion

                        if (imgCount >= 2)
                            break;
                    }
                }

                #endregion

            }
            catch (Exception e)
            {

            }
            return RedirectToPage("./StoreInfo", new { id = id });
        }

      
        public async Task<IActionResult> OnPostRemoveCategory(long id)
        {
            var storecat = await _context.StoreCategories.FirstOrDefaultAsync(x => x.Id == id);
            
            _context.StoreCategories.Remove(storecat);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("./StoreInfo", new { id = id });
        }

    }
}
