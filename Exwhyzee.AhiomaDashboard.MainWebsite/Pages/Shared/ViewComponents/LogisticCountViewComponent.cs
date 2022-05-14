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
    public class LogisticCountViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;

        public LogisticCountViewComponent(AhiomaDbContext context)
        {
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync(long id)
        {
            var item =  _context.LogisticVehicle.Where(x => x.LogisticProfileId == id);
            TempData["active"] = item.Where(x => x.VehicleStatus == Enums.VehicleEnum.Active).Count();
            TempData["all"] = item.Count();

            return View();
        }




    }

}
