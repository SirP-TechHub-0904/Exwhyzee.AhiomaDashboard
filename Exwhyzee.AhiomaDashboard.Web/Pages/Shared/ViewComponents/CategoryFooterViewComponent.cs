using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Pages.Shared.ViewComponents
{
    public class CategoryFooterViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _context;

        public CategoryFooterViewComponent(ICategoryRepository context)
        {
            _context = context;
        }


       
        public IList<Category> Categories { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(string customerRef)
        {
            ViewBag.CustomerRef = customerRef;
            var item = await _context.GetAsyncAll();

            return View(item.Take(7).ToList());
        }




    }

}
