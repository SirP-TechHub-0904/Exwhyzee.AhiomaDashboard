using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Pages.Shared.ViewComponents
{
    public class MarketFooterViewComponent : ViewComponent
    {
        private readonly IMarketRepository _context;

        public MarketFooterViewComponent(IMarketRepository context)
        {
            _context = context;
        }


       
        public IList<Market> Markets { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var item = await _context.GetAsyncAll();

            return View(item.Take(7).ToList());
        }




    }

}
