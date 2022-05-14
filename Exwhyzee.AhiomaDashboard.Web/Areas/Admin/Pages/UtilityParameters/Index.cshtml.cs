using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.UtilityParameters
{
    [Authorize(Roles = "mSuperAdmin")]

    public class IndexModel : PageModel
    {
        private readonly AhiomaDbContext _context;

        public IndexModel(AhiomaDbContext context)
        {
            _context = context;
        }

        public IList<UtilityParameter> iModel { get; set; }

        public async Task OnGetAsync()
        {
            iModel = await _context.UtilityParameters.OrderByDescending(x=>x.ParamType).ToListAsync();
        }
    }
}
