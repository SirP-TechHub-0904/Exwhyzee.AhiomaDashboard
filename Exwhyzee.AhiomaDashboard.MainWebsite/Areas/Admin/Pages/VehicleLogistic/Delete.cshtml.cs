using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.VehicleLogistic
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Logistic,mSuperAdmin")]

    public class DeleteModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public DeleteModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LogisticVehicle = await _context.LogisticVehicle.FindAsync(id);

            if (LogisticVehicle != null)
            {
                _context.LogisticVehicle.Remove(LogisticVehicle);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
