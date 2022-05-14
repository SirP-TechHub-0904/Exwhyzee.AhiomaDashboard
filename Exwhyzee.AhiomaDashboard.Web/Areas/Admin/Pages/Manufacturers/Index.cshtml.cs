using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Manufacturers;
using Microsoft.AspNetCore.Authorization;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Manufacturers
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class IndexModel : PageModel
    {
        private readonly IManufacturerRepository _context;

        public IndexModel(IManufacturerRepository context)
        {
            _context = context;
        }

        public IList<Manufacturer> Manufacturer { get;set; }

        public async Task OnGetAsync()
        {
            Manufacturer = await _context.GetAsyncAll();
        }
    }
}
