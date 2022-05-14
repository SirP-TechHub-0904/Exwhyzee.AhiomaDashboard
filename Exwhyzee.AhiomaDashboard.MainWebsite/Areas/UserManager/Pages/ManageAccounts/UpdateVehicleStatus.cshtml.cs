using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare")]

    public class UpdateVehicleStatusModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public UpdateVehicleStatusModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,

            IUserProfileRepository account, AhiomaDbContext context,
            IEmailSendService emailSender)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        [BindProperty]
        public string UserID { get; set; }
        [BindProperty]
        public AccountStatus AccountStatus { get; set; }
        public string Note { get; set; }
        [BindProperty]
        public LogisticVehicle Vehicle { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            Vehicle = await _context.LogisticVehicle.Include(x => x.LogisticProfile).FirstOrDefaultAsync(x => x.Id == id);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var iVehicle = await _context.LogisticVehicle.Include(x=>x.LogisticProfile).FirstOrDefaultAsync(x => x.Id == Vehicle.Id);
            iVehicle.VehicleStatus = Vehicle.VehicleStatus;
           
            _context.Attach(iVehicle).State = EntityState.Modified;
            string note = "";
            var profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == iVehicle.LogisticProfile.UserId);

            try
            {
                await _context.SaveChangesAsync();
                if (iVehicle.VehicleStatus == VehicleEnum.Active)
                {
                    note = "Activated";
                }
                else if (iVehicle.VehicleStatus == VehicleEnum.Disabled)
                {
                    note = "Disabled";
                }
                await _emailSender.SendToOne(profile.User.Email, "Vehicle Update", "Hi",
                       $" Your account has been "+note+". Kindly https://ahioma.com to enjoy more features.");
                await _emailSender.SMSToOne(profile.User.PhoneNumber,
                      $" Your account has been " + note + ". Kindly https://ahioma.com to enjoy more features.");

            }
            catch (DbUpdateConcurrencyException)
            {
              
                    throw;
                
            }
            TempData["success"] = "success";
           // Profile = await _account.GetByUserId(profile.UserId);
            return RedirectToPage("./UpdateVehicleStatus", new { id = iVehicle.Id });
        }

    }
}
