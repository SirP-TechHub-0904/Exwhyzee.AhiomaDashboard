using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Pages.Shared.ViewComponents
{
    public class LongBannerViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;

        public LongBannerViewComponent(AhiomaDbContext context)
        {
            _context = context;
        }



        public async Task<IViewComponentResult> InvokeAsync()
        {
            var item = await _context.Banners.FirstOrDefaultAsync(x=>x.BannerType == Enums.BannerType.London);

            return View(item);
        }
    }

}
