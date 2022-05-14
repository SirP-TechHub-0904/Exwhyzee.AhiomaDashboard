using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly ICategoryRepository _context;

        public CategoriesViewComponent(ICategoryRepository context)
        {
            _context = context;
        }


       
        public IList<Category> Categories { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IViewComponentResult> InvokeAsync(string customerRef)
        {
           ViewBag.CustomerRef = customerRef;
            var item = await _context.GetAsyncAll();

           var data = item.Where(x=>x.ShowOnHomePage == true).OrderBy(x=>x.DisplayOrder).Take(9).ToList();
            return View(data);
        }




    }

}
