using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class ProductCountByCategoryViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;
        public ProductCountByCategoryViewComponent(AhiomaDbContext context)
        {
            _context = context;
        }


       
        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            var count = await _context.Products.Where(x => x.CategoryId == id).CountAsync();

            TempData["count"] = count;
            return View();
        }
      

    }

}
