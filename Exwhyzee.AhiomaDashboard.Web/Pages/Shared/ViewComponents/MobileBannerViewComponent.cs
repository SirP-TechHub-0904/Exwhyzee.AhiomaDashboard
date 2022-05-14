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
    public class MobileBannerViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;

        public MobileBannerViewComponent(AhiomaDbContext context)
        {
            _context = context;
        }



        public async Task<IViewComponentResult> InvokeAsync()
        {
           // var item = await _context.Banners.ToListAsync();
            IQueryable<Banner> ibanna = from s in _context.Banners
                                              .Where(x => x.BannerType == Enums.BannerType.WebMobile).Take(4)
                                             select s;
            return View(ibanna.ToList());
        }
    }

}
