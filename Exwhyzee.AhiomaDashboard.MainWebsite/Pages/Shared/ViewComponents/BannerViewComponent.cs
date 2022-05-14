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
    public class BannerViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;

        public BannerViewComponent(AhiomaDbContext context)
        {
            _context = context;
        }



        public async Task<IViewComponentResult> InvokeAsync()
        {
            var item = await _context.Banners.ToListAsync();
            var ditem = await _context.Banners.Where(x=>x.BannerType == Enums.BannerType.Washington).ToListAsync();
            ViewBag.img = ditem;
            return View(item);
        }
    }

}
