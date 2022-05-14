using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Logistic.Pages.Dashboard
{
   
        [Authorize(Roles = "Logistic")]

        public class VehiclesModel : PageModel
        {

            private readonly AhiomaDbContext _context;
            private readonly UserManager<IdentityUser> _userManager;
            public VehiclesModel(AhiomaDbContext context,
                UserManager<IdentityUser> userManager)
            {

                _context = context;
                _userManager = userManager;
            }


            public IList<LogisticVehicle> LogisticVehicle { get; set; }
        public long LogisticId { get; set; }

            public async Task<IActionResult> OnGetAsync()
            {
                var user = await _userManager.GetUserAsync(User);
            var logi = _context.LogisticProfiles.FirstOrDefault(x => x.UserId == user.Id);
            LogisticId = logi.Id;
            LogisticVehicle = _context.LogisticVehicle.Include(x => x.LogisticProfile).Where(x => x.LogisticProfile.UserId == user.Id).ToList();
                return Page();
            }
        }
}
