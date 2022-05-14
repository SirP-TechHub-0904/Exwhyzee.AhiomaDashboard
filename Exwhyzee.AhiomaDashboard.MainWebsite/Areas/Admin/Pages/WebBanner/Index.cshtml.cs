using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.WebBanner
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class IndexModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public IndexModel(AhiomaDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _hostingEnv = env;
        }

        public IList<Banner> Banner { get;set; }

        public async Task OnGetAsync()
        {
            Banner = await _context.Banners.ToListAsync();
        }

        public async Task<IActionResult> OnPostBannerDelete(long id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var barner = await _context.Banners.FindAsync(id);

                if (barner != null)
                {
                    _context.Banners.Remove(barner);
                    await _context.SaveChangesAsync();
                    string webRootPath = _hostingEnv.WebRootPath;
                    var fileName = "";
                    fileName = barner.UrlPath;
                    var fullPath = webRootPath + fileName;
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                       
                    }
                }

               

            }
            catch (Exception e)
            {

            }
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostActivateBanner(long id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var barner = await _context.Banners.FindAsync(id);

                if (barner != null)
                {
                    if(barner.Disable == true)
                    {
                        barner.Disable = false;
                    }
                    else
                    {
                        barner.Disable = true;
                    }
                    _context.Attach(barner).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                   
                }



            }
            catch (Exception e)
            {

            }
            return RedirectToPage("./Index");
        }


    }
}
