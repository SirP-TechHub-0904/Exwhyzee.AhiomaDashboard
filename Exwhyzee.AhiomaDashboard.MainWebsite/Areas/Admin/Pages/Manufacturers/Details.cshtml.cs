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

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Manufacturers
{
    [Authorize(Roles = "Admin/Content,mSuperAdmin")]

    public class DetailsModel : PageModel
    {
        private readonly IManufacturerRepository _context;

        public DetailsModel(IManufacturerRepository context)
        {
            _context = context;
        }

        public Manufacturer Manufacturer { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Manufacturer = await _context.GetById(id);

            if (Manufacturer == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
