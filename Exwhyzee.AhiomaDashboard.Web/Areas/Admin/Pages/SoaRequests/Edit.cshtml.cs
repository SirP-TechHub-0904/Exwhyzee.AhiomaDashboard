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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.SoaRequests
{
    public class EditModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public EditModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SoaRequest SoaRequest { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SoaRequest = await _context.SoaRequests.FirstOrDefaultAsync(m => m.Id == id);

            if (SoaRequest == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var check = await _context.UserProfiles.FirstOrDefaultAsync(x => x.IdNumber == SoaRequest.IdNumber);
            if (check != null) {
                var soa = await _context.SoaRequests.FindAsync(SoaRequest.Id);
                soa.IdNumber = SoaRequest.IdNumber;
                soa.DateSentToSoa = DateTime.UtcNow.AddHours(1);
                soa.SoaRequestStatus = Enums.SoaRequestStatus.Assigned;
            _context.Attach(soa).State = EntityState.Modified;


                await _context.SaveChangesAsync();
            }
            else
            {
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private bool SoaRequestExists(long id)
        {
            return _context.SoaRequests.Any(e => e.Id == id);
        }
    }
}
