using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Reconsile.Pages.Data
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin")]

    public class UpdateProfileAddreessModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;

        public UpdateProfileAddreessModel(
            UserManager<IdentityUser> userManager, AhiomaDbContext context)
        {
            _userManager = userManager;
            _context = context;

        }


        public async Task<ActionResult> OnGetAsync()
        {
            
            try
            {
                IQueryable<UserAddress> address = from s in _context.UserAddresses
                                          .Include(p => p.UserProfile)
                                          .Where(x => x.UserProfileId == null)
                                               select s;

                foreach (var i in address)
                {
                    var prof = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == i.UserId);
                    var ads = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == i.Id);
                    ads.UserProfileId = prof.Id;
                    _context.Attach(ads).State = EntityState.Modified;

                    
                }await _context.SaveChangesAsync();
                TempData["success"] = "Password change successful";
            }
            catch (Exception d) { }

            return Page();
        }
    }

}
