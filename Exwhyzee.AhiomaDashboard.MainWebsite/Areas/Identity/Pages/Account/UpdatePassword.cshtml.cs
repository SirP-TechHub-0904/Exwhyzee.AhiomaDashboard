using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class UpdatePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserProfileRepository _account;

        public UpdatePasswordModel(UserManager<IdentityUser> userManager, IUserProfileRepository account)
        {
            _userManager = userManager;
            _account = account;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

        }

        public IActionResult OnGet(string code = null)
        {

                return Page();
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./Logout");
            }
            var checkoldpass = await _userManager.CheckPasswordAsync(user, Input.OldPassword);
            if (checkoldpass.Equals(true))
            {
                var changepass = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.Password);
                if (changepass.Succeeded)
                {
                    var profile = await _account.GetByUserId(user.Id);
                    profile.FirstTimeLogin = true;
                    await _account.Update(profile);
                    return RedirectToPage("/Dashboard/Index", new { area = "Store" });
                }
                foreach (var error in changepass.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
           

           
            return Page();
        }
    }
}
