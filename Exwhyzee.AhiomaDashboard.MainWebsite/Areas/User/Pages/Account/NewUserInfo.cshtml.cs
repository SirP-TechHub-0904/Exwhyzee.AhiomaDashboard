using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    [Authorize]
    public class NewUserInfoModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;
        private readonly UserManager<IdentityUser> _userManager;


        public NewUserInfoModel(AhiomaDbContext context, UserManager<IdentityUser> userManager, IUserProfileRepository account

            )
        {
            _context = context;
            _userManager = userManager;
            _account = account;
        }
        public UserProfile UserProfile { get; set; }
        [BindProperty]
        public UserAddress UserAddress { get; set; }

        [BindProperty]
        public string Surname { get; set; }
        [BindProperty]
        public string OtherName { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        public IList<ProductCart> ProductCartsBUser { get; set; }


        public List<SelectListItem> AddressListing { get; set; }
        public List<SelectListItem> StateListing { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }

        [BindProperty]
        public bool NoAddress { get; set; }
        [BindProperty]
        public long AddressUId { get; set; }

        [BindProperty]
        public long AddressUpdateId { get; set; }

        [BindProperty]
        public string UserId { get; set; }

        [BindProperty]
        public string UserIdx { get; set; }


        public async Task<IActionResult> OnGetAsync(string customerRef, string uxi, long id)
        {
            CustomerRef = customerRef;
            var user = await _userManager.GetUserAsync(User);
            UserId = user.Id;
            AddressUId = id;
            UserProfile = await _context.UserProfiles.Include(x=>x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
            
           
                    var addressli = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == id);
                    

                    var statei = await _account.GetStates();
                    StateListing = statei.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.StateName,
                                              Text = a.StateName
                                          }).ToList();
                    return Page();
               
        }
      
        public async Task<IActionResult> OnPostAsync()
        {
            var data = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == AddressUpdateId);
            data.Address = UserAddress.Address;
            data.State = UserAddress.State;
            data.Default = true;
            data.LocalGovernment = UserAddress.LocalGovernment;

            _context.Entry(data).State = EntityState.Modified;

            //
            var profilex = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == UserIdx);
            profilex.Surname = Surname;
            profilex.FirstName = FirstName;
            profilex.OtherNames = OtherName;
            _context.Entry(profilex).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("/Account/DeliveryMethod", new { area = "User" });
        }

    }
}
