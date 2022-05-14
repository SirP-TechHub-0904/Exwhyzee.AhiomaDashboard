using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.Web.Services;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RecoverAccountModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly IUserLogging _log;
        private readonly ILogger<LoginModel> _logger;
        private readonly AhiomaDbContext _context;
        public RecoverAccountModel(SignInManager<IdentityUser> signInManager,
            ILogger<LoginModel> logger, IUserProfileRepository account,
            AhiomaDbContext context,
            IUserLogging log,
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _log = log;
            _account = account;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            public string SecurityAnswer { get; set; }
        }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task OnGetAsync(string customerRef, string returnUrl = null)
        {
            CustomerRef = customerRef;
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

           
            ReturnUrl = returnUrl;
        }


        public async Task<IActionResult> OnPostAsync()
        {
          
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {

                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                if(Input.SecurityAnswer == profile.SecurityAnswer)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                
                   
                       
                        var superrole = await _userManager.IsInRoleAsync(user, "mSuperAdmin");
                        var adminrole = await _userManager.IsInRoleAsync(user, "Admin");
                        var storerole = await _userManager.IsInRoleAsync(user, "Store");
                        var soarole = await _userManager.IsInRoleAsync(user, "SOA");
                        var customerrole = await _userManager.IsInRoleAsync(user, "Customer");
                        var logisticrole = await _userManager.IsInRoleAsync(user, "Logistic");
                        var SubAdmin = await _userManager.IsInRoleAsync(user, "SubAdmin");


                        if (superrole.Equals(true))
                        {
                            return RedirectToPage("./Analysis/Index", new { area = "Dashboard" });
                        }
                        else if (SubAdmin.Equals(true))
                        {
                            return RedirectToPage("./Dashboard/Index", new { area = "AdminPage" });

                        }
                        else if (storerole.Equals(true))
                        {
                           
                            if (profile.FirstTimeLogin == false)
                            {
                                return RedirectToPage("./Account/UpdatePassword", new { area = "Identity" });
                            }
                            else
                            {
                                return RedirectToPage("./Dashboard/Index", new { area = "Store" });

                            }

                        }
                        else if (soarole.Equals(true))
                        {
                            return RedirectToPage("./Dashboard/Index", new { area = "SOA" });

                        }

                        else if (logisticrole.Equals(true))
                        {
                            return RedirectToPage("./Dashboard/Index", new { area = "Logistic" });

                        }
                        else if (customerrole.Equals(true))
                        {
                            return RedirectToPage("./Dashboard/Index", new { area = "Customer" });

                        }
                       
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
                else
                {
                    var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                    ModelState.AddModelError(string.Empty, "Unable to recover account. live chat our support");
                    return Page();
                }
            ModelState.AddModelError(string.Empty, "Unable to recover account. live chat our support");
            return Page();
        }
        // If we got this far, something failed, redisplay form
        public async Task<JsonResult> OnGetSecurityQuestion(string email)
      {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);

                string da = "security question (<b>"+ profile.SecurityQuestion + "<br>)";
                return new JsonResult(da);
            }
            catch (Exception k)
            {
                return new JsonResult("security question not found");
            }
        }
    }
}
