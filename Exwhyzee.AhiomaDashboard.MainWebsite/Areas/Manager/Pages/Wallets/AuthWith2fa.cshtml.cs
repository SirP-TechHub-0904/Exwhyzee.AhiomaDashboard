using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Manager.Pages.Wallets
{
    [Authorize]
    public class AuthWith2faModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AuthWith2faModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly AhiomaDbContext _context;

        public AuthWith2faModel(AhiomaDbContext context, SignInManager<IdentityUser> signInManager, ILogger<AuthWith2faModel> logger, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> OnGetAsync(string uid = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }
            PostUserId = user.Id;

            //var result = await _signInManager.SignInAsync(user);
            //if (result.Succeeded)
            //{

            //}
                return Page();
        }
        private const string GoogleApiTokenInfoUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={0}";

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }
            var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            //validate admin
            var adminuser = await _context.UserProfiles.Include(x=>x.User).FirstOrDefaultAsync(x => x.IdNumber == "0000001");
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(adminuser.User);

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            bool isCorrectPIN = tfa.ValidateTwoFactorPIN(unformattedKey, authenticatorCode);
            if (isCorrectPIN == true)
            {
              
            }
            else
            {
            }
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, false, false);

            if (result.Succeeded)
            {
                HttpContext.Session.SetString("UpdateWalletKey", Guid.NewGuid().ToString());
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return RedirectToPage("./UpdateWallet", new { uid = UserId });
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
