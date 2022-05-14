using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class SubCategoriesViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;

        public SubCategoriesViewComponent(AhiomaDbContext context)
        {
            _context = context;
        }


       

        public async Task<IViewComponentResult> InvokeAsync(string customerRef, long id)
        {
            ViewBag.CustomerRef = customerRef;
            try
            {
                var item = await _context.SubCategories.Include(x=>x.Category).Where(x => x.CategoryId == id).ToListAsync();

                return View(item);
            }catch(Exception c)
            {
                TempData["null"] = "null";
                return View();
            }
        }




    }

}
