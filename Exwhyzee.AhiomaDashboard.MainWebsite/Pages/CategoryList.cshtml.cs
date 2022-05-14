using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    public class CategoryListModel : PageModel
    {
        public class CategoryModel : PageModel
        {

            private readonly AhiomaDbContext _context;
            private readonly ICategoryRepository _category;

            public CategoryModel(
                ICategoryRepository category,
                AhiomaDbContext context
                )
            {
                _category = category;
               

                _context = context;

            }


            public IList<Category> CategoryList { get; set; }

            [BindProperty]
            public string CustomerRef { get; set; }
            public async Task OnGetAsync(string customerRef)
            {
                CustomerRef = customerRef;
                CategoryList = await _category.GetAsyncAll();

            }
          

        }

    }
}
