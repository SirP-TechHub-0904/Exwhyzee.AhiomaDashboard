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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Drawing.Printing;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare")]

    public class EditLogisticModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public EditLogisticModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public LogisticProfile Logistic { get; set; }
       
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Logistic = await _context.LogisticProfiles.Include(x=>x.User).Include(x => x.UserProfile).FirstOrDefaultAsync(m => m.Id == id);

            if (Logistic == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var t = await _context.LogisticProfiles.FirstOrDefaultAsync(x => x.Id == Logistic.Id);
           
            t.CustomerCareNumber = Logistic.CustomerCareNumber;
            t.LogisticStatus = Logistic.LogisticStatus;
            t.Referee = Logistic.Referee;
            t.CompanyDocument = Logistic.CompanyDocument;
            t.Description = Logistic.Description;
            t.CompanyName = Logistic.CompanyName;

            _context.Attach(t).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProfileExists(Logistic.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./LogisticInfo", new { id = Logistic.Id });
        }

        private bool UserProfileExists(long id)
        {
            return _context.UserProfiles.Any(e => e.Id == id);
        }
    }
}
