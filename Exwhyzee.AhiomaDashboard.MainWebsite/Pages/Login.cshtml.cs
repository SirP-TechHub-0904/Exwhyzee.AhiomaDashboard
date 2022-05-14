using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly IUserLogging _log;
        private readonly ILogger<LoginModel> _logger;
        private readonly AhiomaDbContext _context;
        public LoginModel(SignInManager<IdentityUser> signInManager, 
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

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
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

           // returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

       
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {

                    var passcheck = await _userManager.CheckPasswordAsync(user, Input.Password);
                    //if(passcheck == true && user.LockoutEnabled == true)
                    //{
                        
                    //        _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                    //        return RedirectToPage("/Account/Lockout", new { area = "Identity" });
                        
                    //}
                    if (passcheck == true && user.TwoFactorEnabled == true)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            var userprofile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
                            if (userprofile.UserAddresses.Count > 0)
                            {
                                if (userprofile.UserAddresses.FirstOrDefault().Address == null)
                                {
                                    TempData["error"] = "Kindly Update your Profile and scroll down to update address";
                                    return RedirectToPage("/Account/Profile", new { area = "User", returnUrl = ReturnUrl });

                                }
                                else if (userprofile.UserAddresses.FirstOrDefault().State == null)
                                {
                                    TempData["error"] = "Kindly Update your Profile and scroll down to update address";
                                    return RedirectToPage("/Account/Profile", new { area = "User", returnUrl = ReturnUrl });
                                }
                                else if (userprofile.UserAddresses.FirstOrDefault().LocalGovernment == null)
                                {
                                    TempData["error"] = "Kindly Update your Profile and scroll down to update address";
                                    return RedirectToPage("/Account/Profile", new { area = "User", returnUrl = ReturnUrl });
                                }
                            }
                            else
                            {
                                TempData["error"] = "Kindly Update your Profile and scroll down to update address";
                                return RedirectToPage("/Account/Profile", new { area = "User", returnUrl = ReturnUrl });
                            }

                            _logger.LogInformation("User logged in.");
                            try
                            {
                                var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                                var mainurllink = urllink.AbsoluteUri;
                                var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                                var lognew = await _log.LogData(user.UserName, "", mainurllink);
                                Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                                _context.Attach(Userlog).State = EntityState.Modified;
                                await _context.SaveChangesAsync();

                            }
                            catch (Exception s)
                            {

                            }
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
                                var profile = await _account.GetByUserId(user.Id);
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
                            return LocalRedirect(returnUrl);
                        }
                        if (result.RequiresTwoFactor)
                        {
                            return RedirectToPage("/Account/LoginWith2fa", new { area = "Identity", ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        }
                        if (result.IsLockedOut)
                        {
                            _logger.LogWarning("User account locked out.");
                            return RedirectToPage("./Lockout");
                        }
                        else
                        {
                            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                            return Page();
                        }
                    }
                    else if(passcheck == true)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User logged in.");
                        try
                        {
                            var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                            var mainurllink = urllink.AbsoluteUri;
                            var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                            var lognew = await _log.LogData(user.UserName, "", mainurllink);
                            Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                            _context.Attach(Userlog).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                        }
                        catch (Exception s)
                        {

                        }
                        var userprofile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
                        if (userprofile.UserAddresses.Count > 0)
                        {
                            if (userprofile.UserAddresses.FirstOrDefault().Address == null)
                            {
                                TempData["error"] = "Kindly Update your Profile and scroll down to update address";
                                return RedirectToPage("/Account/Profile", new { area = "User", returnUrl = ReturnUrl });

                            }
                            else if (userprofile.UserAddresses.FirstOrDefault().State == null)
                            {
                                TempData["error"] = "Kindly Update your Profile and scroll down to update address";
                                return RedirectToPage("/Account/Profile", new { area = "User", returnUrl = ReturnUrl });
                            }
                            else if (userprofile.UserAddresses.FirstOrDefault().LocalGovernment == null)
                            {
                                TempData["error"] = "Kindly Update your Profile and scroll down to update address";
                                return RedirectToPage("/Account/Profile", new { area = "User", returnUrl = ReturnUrl });
                            }
                        }
                        else
                        {
                            TempData["error"] = "Kindly Update your Profile and scroll down to update address";
                            return RedirectToPage("/Account/Profile", new { area = "User", returnUrl = ReturnUrl });
                        }
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
                            var profile = await _account.GetByUserId(user.Id);
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
                        return LocalRedirect(returnUrl);
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
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
