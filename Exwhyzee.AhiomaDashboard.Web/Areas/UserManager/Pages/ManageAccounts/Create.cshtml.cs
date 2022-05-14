using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; 
        private readonly SignInManager<IdentityUser> _signInManager;

        //private readonly IEmailSender _emailSender;
        private readonly IUserProfileRepository _account;

        public CreateModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IUserProfileRepository account
            /*IEmailSender emailSender*/)
        {
            _userManager = userManager;
            //_emailSender = emailSender;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            public string PhoneNumber { get; set; }

            public string Surname { get; set; }

            public string FirstName { get; set; }

            public string OtherNames { get; set; }

            public DateTime DateRegistered { get; set; }
            public string Role { get; set; }

        }

        public List<SelectListItem> RoleListing { get; set; }


        public async Task OnGetAsync(string returnUrl = null)
        {
            var list = await _roleManager.Roles.ToListAsync();
            RoleListing = list.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id,
                                      Text = a.Name
                                  }).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
           
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.PhoneNumber };
                var check = _userManager.Users.ToList();

                if (check.Select(x => x.Email).Contains(Input.Email))
                {
                    TempData["error"] = "Email already taken";

                    return Page();
                }

                if (check.Select(x => x.PhoneNumber).Contains(Input.PhoneNumber))
                {
                    TempData["error"] = "phone number already taken";

                    return Page();
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var Role = await _roleManager.FindByIdAsync(Input.Role);
                    if (Role != null)
                    {
                        await _userManager.AddToRoleAsync(user, Role.Name);

                    }

                    //create wallet
                    Wallet wallet = new Wallet();
                    wallet.UserId = user.Id;
                    wallet.Balance = 0;
                    wallet.CreationTime = DateTime.UtcNow.AddHours(1);
                    wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);


                    UserProfile profile = new UserProfile();
                    profile.UserId = user.Id;
                    profile.DateRegistered = DateTime.UtcNow.AddHours(1);
                    profile.Surname = Input.Surname;
                    profile.FirstName = Input.FirstName;
                    profile.OtherNames = Input.OtherNames;
                    profile.DateRegistered = DateTime.UtcNow.AddHours(1);

                    string id = await _account.CreateAccount(wallet, profile, null, null, null);

                    //await _signInManager.SignInAsync(user, isPersistent: false);

                    //var resultcheck = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, true, lockoutOnFailure: false);
                    //if (User.Identity.IsAuthenticated)
                    //{

                    //    return LocalRedirect(returnUrl);
                    //}

                    //else
                    //{
                    //    var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    //    return Page();
                    //}
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    TempData["success"] = "Account created successfully";
                    return RedirectToPage("./Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
