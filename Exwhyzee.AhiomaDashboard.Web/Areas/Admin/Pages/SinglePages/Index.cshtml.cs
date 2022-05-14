using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.SinglePages
{
    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public IndexModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public IList<WebPages> WebPages { get;set; }

        public async Task OnGetAsync()
        {
            WebPages = await _context.WebPages.ToListAsync();
        }
    }
}
