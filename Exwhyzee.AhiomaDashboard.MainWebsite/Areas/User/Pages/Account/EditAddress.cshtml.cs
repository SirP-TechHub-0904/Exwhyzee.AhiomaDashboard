using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    /// <summary>
    /// 
    /// </summary>
    public class EditAddressModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _profile;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _hostingEnv;
        public EditAddressModel(IUserProfileRepository profile, RoleManager<IdentityRole> roleManager, AhiomaDbContext context,
            UserManager<IdentityUser> userManager,
             IHostingEnvironment hostingEnv
            )
        {
            _context = context;
            _profile = profile;
            _roleManager = roleManager;
            _userManager = userManager;
            _hostingEnv = hostingEnv;
        }
        public UserProfile UserProfile { get; set; }
        [BindProperty]
        public UserAddress UserAddress { get; set; }

        [BindProperty]
        public string Surname { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        public IList<ProductCart> ProductCartsBUser { get; set; }


        public List<SelectListItem> StateListing { get; set; }

        [BindProperty]
        public long Aid { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            Aid = id;
            var user = await _userManager.GetUserAsync(User);
            UserProfile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
          
            var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == id);

        

            var state = await _profile.GetStates();
            StateListing = state.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.StateName,
                                      Text = a.StateName
                                  }).ToList();
            UserAddress = address;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == Aid);
            address.Address = UserAddress.Address;
            address.State = UserAddress.State;
            address.LocalGovernment = UserAddress.LocalGovernment;
            _context.Attach(address).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Profile", new { area = "User" });
        }

    }
}
