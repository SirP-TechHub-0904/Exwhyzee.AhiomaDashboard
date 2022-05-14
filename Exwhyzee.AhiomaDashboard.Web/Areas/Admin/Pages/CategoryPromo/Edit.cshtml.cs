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
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.IO;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.CategoryPromo
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class EditModel : PageModel
    {
        private readonly AhiomaDbContext _market;
        private readonly IUserProfileRepository _account;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;


        public EditModel(AhiomaDbContext market, IHostingEnvironment hostingEnv, IUserProfileRepository account, AhiomaDbContext context)
        {
            _market = market;
            _context = context;
            _account = account;
            _hostingEnv = hostingEnv;
        }

        [BindProperty]
        public PromoCategory Market { get; set; }
      

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           

            Market = await _market.PromoCategories.FindAsync(id);

            if (Market == null)
            {
                return NotFound();
            }
            return Page();
        }

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
                var marketUpdate = await _context.PromoCategories.FindAsync(Market.Id);

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
                        filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        imgCount++;
                        var now = DateTime.Now;

                        var fileExtension = Path.GetExtension(filePath);

                        newFileName = $"{now.Minute}{now.Month}{now.Day}-".Trim() + file.FileName;

                        // if you wish to save file path to db use this filepath variable + newFileName
                        var fileDbPathName = $"/Market/".Trim();

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



                        #region Save Image Propertie to Db
                        string webRootPath = _hostingEnv.WebRootPath;
                        var fileNamedelete = "";
                        fileNamedelete = marketUpdate.Banner;
                        var fullPath = webRootPath + fileNamedelete;
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);

                        }
                        marketUpdate.Banner = $"{fileDbPathName}{newFileName}";

                    };

                    #endregion

                }

                #endregion


                _context.Entry(marketUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;

            }

            return RedirectToPage("./Index");
        }

    }
}
