using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Categories
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class ProductShowOnHomeModel : PageModel
    {


        private readonly AhiomaDbContext _context;

        public ProductShowOnHomeModel(AhiomaDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> OnGetAsync(long? id, long cId)
        {
            try
            {
                var check = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);


                if (check == null)
                {
                    return RedirectToPage("./CategoryPage", new { id = cId });
                }
                if (check.ShowOnHomePage == false)
                {
                    check.ShowOnHomePage = true;
                }
                else
                {
                    check.ShowOnHomePage = false;
                }
                _context.Attach(check).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                TempData["success"] = "Updated Successful";
            }
            catch (Exception p)
            {
                TempData["error"] = "Not Successful";
            }
            return RedirectToPage("./CategoryPage", new { id = cId });
        }


    }
}
