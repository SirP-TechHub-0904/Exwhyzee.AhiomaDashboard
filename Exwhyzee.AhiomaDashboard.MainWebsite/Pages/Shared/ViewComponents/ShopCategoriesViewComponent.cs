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
    public class ShopCategoriesViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;

        public ShopCategoriesViewComponent(AhiomaDbContext context)
        {
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            var item = await _context.StoreCategories.Include(x=>x.Category).Include(x=>x.Category.SubCategories).Where(x=>x.TenantId == id).ToListAsync();

            return View(item.ToList());
        }




    }

}
