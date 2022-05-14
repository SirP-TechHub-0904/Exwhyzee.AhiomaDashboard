using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.VehicleLogistic
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Logistic,mSuperAdmin")]

    public class EditModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public EditModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LogisticVehicle LogisticVehicle { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LogisticVehicle = await _context.LogisticVehicle
                .Include(l => l.LogisticProfile).FirstOrDefaultAsync(m => m.Id == id);

            if (LogisticVehicle == null)
            {
                return NotFound();
            }
           ViewData["LogisticProfileId"] = new SelectList(_context.LogisticProfiles, "Id", "Id");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(LogisticVehicle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogisticVehicleExists(LogisticVehicle.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Dashboard/Vehicles", new { area = "Logistic" });
        }

        private bool LogisticVehicleExists(long id)
        {
            return _context.LogisticVehicle.Any(e => e.Id == id);
        }
    }
}
