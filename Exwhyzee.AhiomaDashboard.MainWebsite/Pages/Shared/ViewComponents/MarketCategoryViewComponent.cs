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
    public class MarketCategoryViewComponent : ViewComponent
    {
        private readonly IMarketRepository _context;

        public MarketCategoryViewComponent(IMarketRepository context)
        {
            _context = context;
        }


       
        public IList<Market> Markets { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(string customerRef)
        {
            ViewBag.CustomerRef = customerRef;
            var item = await _context.GetAsyncAll();

            return View(item.Take(11).ToList());
        }




    }

}
