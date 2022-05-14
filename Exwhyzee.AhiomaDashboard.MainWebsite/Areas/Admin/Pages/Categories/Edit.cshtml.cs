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
using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Categories
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class EditModel : PageModel
    {
        private readonly ICategoryRepository _category;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public EditModel(ICategoryRepository category, AhiomaDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _category = category;
            _hostingEnv = env;
        }
        [BindProperty]
        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (Category == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            int imgCount = 0;
            if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count > 0)
            {
                var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

                var url = location.AbsoluteUri;
                var urlPath = location.Authority;
                var newFileName = string.Empty;
                var newFileNameThumbnail = string.Empty;
                var filePath = string.Empty;
                var filePathThumbnail = string.Empty;
                string pathdb = string.Empty;
                var file = HttpContext.Request.Form.Files.FirstOrDefault();

                if (file.Length > 0)
                {
                    filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    filePathThumbnail = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    imgCount++;
                    var now = DateTime.Now;
                    string nameproduct = Category.Name.Replace(" ", "-");
                    var uniqueFileName = $"{now.Minute}{now.Month}{now.Day}-".Trim() + nameproduct;
                    var uniqueFileNameThumbnail = $"{now.Minute}{now.Month}{now.Day}-".Trim() + nameproduct + "(1)";

                    var fileExtension = Path.GetExtension(filePath);

                    newFileName = uniqueFileName + fileExtension;
                    newFileNameThumbnail = uniqueFileNameThumbnail + fileExtension;

                    // if you wish to save file path to db use this filepath variable + newFileName
                    var fileDbPathName = $"/Category/".Trim();

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
                    Category.ImagePath = $"{fileDbPathName}/{newFileName}";
                    Category.ImageUrl = "http://" + urlPath + $"{fileDbPathName}/{newFileName}";

                }

            }


            _context.Attach(Category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(Category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CategoryExists(long id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
