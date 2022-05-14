using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginWith2faModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserProfileRepository _account;
        private readonly AhiomaDbContext _context;

        public LoginWith2faModel(AhiomaDbContext context, IUserProfileRepository account, SignInManager<IdentityUser> signInManager, ILogger<LoginWith2faModel> logger, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _account = account;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Authenticator code")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }
        [BindProperty]
        public string PostUserId { get; set; }
        [BindProperty]
        public string UserId { get; set; }
        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
           // var user = await _userManager.FindByIdAsync(uid);
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                TempData["error"] = "Unable to Process your Login. Try Again";
                return RedirectToPage("/Login");
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }
PostUserId = user.Id;
            ReturnUrl = returnUrl;
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

           // returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                TempData["error"] = "Unable to Process your Login. Try Again";
                return RedirectToPage("/Login");
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                if (returnUrl != null)
                {
                    if (returnUrl.Contains("BuyNow"))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return LocalRedirect(returnUrl);
                    }
                }
                var superrole = await _userManager.IsInRoleAsync(user, "mSuperAdmin");
                var adminrole = await _userManager.IsInRoleAsync(user, "Admin");
                var storerole = await _userManager.IsInRoleAsync(user, "Store");
                var soarole = await _userManager.IsInRoleAsync(user, "SOA");
                var customerrole = await _userManager.IsInRoleAsync(user, "Customer");
                var logisticrole = await _userManager.IsInRoleAsync(user, "Logistic");


                if (superrole.Equals(true))
                {
                    return RedirectToPage("/Analysis/Index", new { area = "Dashboard" });
                }
                //else if (adminrole.Equals(true))
                //{
                //    return RedirectToPage("/Dashboard/Index", new { area = "Admin" });

                //}
                else if (storerole.Equals(true))
                {
                    var profile = await _account.GetByUserId(user.Id);
                    if (profile.FirstTimeLogin == false)
                    {
                        return RedirectToPage("/Account/UpdatePassword", new { area = "Identity" });
                    }
                    else
                    {
                        return RedirectToPage("/Dashboard/Index", new { area = "Store" });

                    }

                }
                else if (soarole.Equals(true))
                {
                    return RedirectToPage("/Dashboard/Index", new { area = "SOA" });

                }

                else if (logisticrole.Equals(true))
                {
                    return RedirectToPage("/Dashboard/Index", new { area = "Logistic" });

                }
                else if (customerrole.Equals(true))
                {
                    return RedirectToPage("/Dashboard/Index", new { area = "Customer" });

                }
                return LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return Page();
            }
        }
    }
}
