using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Identity.Pages.Account.Manage
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public partial class PhoneNumberModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSendService _emailSender;
        private readonly AhiomaDbContext _context;
        public PhoneNumberModel(
            UserManager<IdentityUser> userManager, AhiomaDbContext context,
            SignInManager<IdentityUser> signInManager,
            IEmailSendService emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
        }

        public string Username { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            //[Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        //private async Task LoadAsync(IdentityUser user)
        //{
        //    var email = await _userManager.GetEmailAsync(user);
        //    Email = email;

        //    Input = new InputModel
        //    {
        //        NewEmail = email,
        //    };

        //    IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        //}
        public bool IsPhoneConfirmed { get; set; }
        [BindProperty]
        public string Code { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            PhoneNumber = user.PhoneNumber;
            if (user.PhoneNumberConfirmed != true)
            {
                string abc = Guid.NewGuid().ToString();
                string vcode = abc.Substring(1, 5).ToUpper();
                try
                {
                    var profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == user.Id);
                    profile.VerificationCode = vcode;

                    _context.Attach(profile).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                }
                catch (Exception d)
                {

                }
               

                string message = "C0NFIRM C0DE: " + vcode;
                await _emailSender.SMSToOne(user.PhoneNumber, message);
            }
            IsPhoneConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostSendVerificationPhoneAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            try
            {
                var profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == user.Id);
                if(profile.VerificationCode == Code.ToUpper())
                {
                    var upuser = await _userManager.GetUserAsync(User);
                    upuser.PhoneNumberConfirmed = true;
                    await _userManager.UpdateAsync(upuser);
                    TempData["Confirmed"] = "Phone Number Verified";
                    return RedirectToPage();
                }
                else
                {

                }
               
            }
            catch (Exception d)
            {

            }


            TempData["error"] = "unable to verify";
            return RedirectToPage();
        }

    }
}
