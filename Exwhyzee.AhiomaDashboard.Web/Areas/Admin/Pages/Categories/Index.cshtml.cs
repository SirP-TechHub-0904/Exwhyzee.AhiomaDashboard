using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Microsoft.AspNetCore.Authorization;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Categories
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class IndexModel : PageModel
    {
        private readonly ICategoryRepository _context;

        public IndexModel(ICategoryRepository context)
        {
            _context = context;
        }

        public IList<Category> Category { get; set; }

        public async Task OnGetAsync()
        {
            Category = await _context.GetAsyncAll();
        }
    }
}
