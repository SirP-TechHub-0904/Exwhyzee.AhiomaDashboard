using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly AhiomaDbContext _context;


        public ConfirmEmailModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUserProfileRepository account, AhiomaDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _account = account;
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            try
            {
                if (userId == null || code == null)
                {
                    TempData["error"] = "Unable to Comfirm Account.";
                    TempData["error1"] = "Go to Dashboard to Resend Confirmation Mail";
                    return RedirectToPage("/Info/Invalid");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["error"] = "Unable to Comfirm Account.";
                    TempData["error1"] = "Go to Dashboard to Resend Confirmation Mail";
                    return RedirectToPage("/Info/Invalid");
                }

                var profileupdate = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                
                if(profileupdate == null)
                {
                    TempData["error"] = "Unable to Comfirm Account.";
                    TempData["error1"] = "Go to Dashboard to Resend Confirmation Mail";
                    return RedirectToPage("/Info/Invalid");
                }
                if(profileupdate.TokenAccount == code) {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);

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
                        return RedirectToPage("/Analysis/Index", new { area = "Dashboard" });
                    }
                    else if (SubAdmin.Equals(true))
                    {
                        return RedirectToPage("/Dashboard/Index", new { area = "AdminPage" });

                    }
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
                        return RedirectToPage("./Dashboard/Index", new { area = "Logistic" });

                    }
                    else if (customerrole.Equals(true))
                    {
                        return RedirectToPage("/Dashboard/Index", new { area = "Customer" });

                    }
                    StatusMessage = "Thank you for confirming your email.";
                    return RedirectToPage("./Index", new { area = "" });

                }
                else
                {
                    TempData["error"] = "Unable to Comfirm Account.";
                    TempData["error1"] = "Go to Dashboard to Resend Confirmation Mail";
                    return RedirectToPage("/Info/Invalid");
                }
                //StatusMessage = "Thank you for confirming your email.";
                return Page();
            }catch(Exception c)
            {
                TempData["error"] = "Unable to Comfirm Account.";
                TempData["error1"] = "Go to Dashboard to Resend Confirmation Mail";
                return RedirectToPage("/Info/Invalid");
            }
        }
    }
}
