using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class MobileMarketCategoryViewComponent : ViewComponent
    {
        private readonly IMarketRepository _context;

        public MobileMarketCategoryViewComponent(IMarketRepository context)
        {
            _context = context;
        }


       
        public IList<Market> Markets { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(string customerRef)
        {
            ViewBag.CustomerRef = customerRef;
            var item = await _context.GetAsyncAll();
            item = item.OrderBy(x=>x.SortOrder).Take(7).ToList();
            return View(item);
        }




    }

}
