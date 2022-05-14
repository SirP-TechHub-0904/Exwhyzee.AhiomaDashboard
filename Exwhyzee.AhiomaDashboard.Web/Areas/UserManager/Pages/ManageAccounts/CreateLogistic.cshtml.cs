using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "SOA,Admin,mSuperAdmin,CustomerCare")]

    public class CreateLogisticModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserLogging _log;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IMarketRepository _market;

        public CreateLogisticModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserLogging log,
        SignInManager<IdentityUser> signInManager,
            AhiomaDbContext context,
            IMarketRepository market,
            IUserProfileRepository account,
            IEmailSendService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
            _log = log;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _market = market;
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

            public string UserId { get; set; }
            public IdentityUser User { get; set; }
            public long? UserProfileId { get; set; }
            public UserProfile UserProfile { get; set; }
            public DateTime CreationTime { get; set; }
            public string CompanyName { get; set; }
            public string Description { get; set; }
            public string CompanyDocument { get; set; }
            public string Referee { get; set; }
            public string CustomerCareNumber { get; set; }
            [Display(Name = "Contact Address")]
            public string ContactAddress { get; set; }
            [Display(Name = "Alt Phone Number")]
            public string AltPhoneNumber { get; set; }
            public string State { get; set; }
            public string LocalGovernment { get; set; }
            public string Facebook { get; set; }
            public string Instagram { get; set; }
            public string WhatsappNumber { get; set; }
            public string LinkedIn { get; set; }
        }

        public List<SelectListItem> StateListing { get; set; }
        public string LoggedInUser { get; set; }


        public async Task<ActionResult> OnGetAsync(string returnUrl = null)
        {
            //LoggedInUser = _userManager.GetUserId(HttpContext.User);
            //var user = await _userManager.FindByIdAsync(LoggedInUser);
            //var profile = await _account.GetByUserId(user.Id);
            //if(profile.Status != AccountStatus.Active)
            //{
            //    TempData["error"] = profile.Note;
            //    return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            //}

            var state = await _account.GetStates();
            StateListing = state.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.StateName,
                                      Text = a.StateName
                                  }).ToList();

          
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            if (ModelState.IsValid)
            {

              
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.PhoneNumber };
                var check = _userManager.Users.ToList();

                if (check.Select(x => x.Email).Contains(Input.Email))
                {
                    LoggedInUser = _userManager.GetUserId(HttpContext.User);

                    var state = await _account.GetStates();
                    StateListing = state.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.StateName,
                                              Text = a.StateName
                                          }).ToList();

                    var mkt = await _market.GetAsyncAll();
                  
                    TempData["error"] = "Email already taken";

                    return Page();
                }

                if (check.Select(x => x.PhoneNumber).Contains(Input.PhoneNumber))
                {
                    LoggedInUser = _userManager.GetUserId(HttpContext.User);

                    var state = await _account.GetStates();
                    StateListing = state.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.StateName,
                                              Text = a.StateName
                                          }).ToList();

                    var mkt = await _market.GetAsyncAll();
                  
                    TempData["error"] = "phone number already taken";

                    return Page();
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var Role = await _roleManager.FindByNameAsync("Logistic");
                    if (Role != null)
                    {
                        await _userManager.AddToRoleAsync(user, Role.Name);
                        await _userManager.AddToRoleAsync(user, "Customer");

                    }
                    string userroles = await _account.FetchUserRoles(user.Id);
                    //create wallet
                    Wallet wallet = new Wallet();
                    wallet.UserId = user.Id;
                    wallet.Balance = 0;
                    wallet.WithdrawBalance = 0;
                    wallet.CreationTime = DateTime.UtcNow.AddHours(1);
                    wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);


                    UserProfile profile = new UserProfile();
                    profile.UserId = user.Id;
                    profile.DateRegistered = DateTime.UtcNow.AddHours(1);
                    profile.Surname = Input.Surname;
                    profile.FirstName = Input.FirstName;
                    profile.Roles = userroles;
                    profile.OtherNames = Input.OtherNames;
                    profile.DateRegistered = DateTime.UtcNow.AddHours(1);

                    LogisticProfile logistic = new LogisticProfile();
                    logistic.UserId = user.Id;
                    logistic.CreationTime = DateTime.UtcNow.AddHours(1);
                    logistic.CompanyName = Input.CompanyName;
                    logistic.LogisticStatus = LogisticEnum.Disabled;
                    logistic.Description = Input.Description;
                    logistic.CustomerCareNumber = Input.CustomerCareNumber;
                    logistic.CompanyDocument = Input.CompanyDocument;

                    UserAddress address = new UserAddress();
                    address.Address = Input.ContactAddress;
                    address.State = Input.State;
                    address.LocalGovernment = Input.LocalGovernment;

                    string id = await _account.CreateAccountLogistic(wallet, profile, logistic, address, Input.Facebook, Input.Instagram, Input.LinkedIn, Input.WhatsappNumber);



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
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendToOne(Input.Email, "Confirm your email", "Hi",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    await _emailSender.SMSToOne(Input.PhoneNumber, "Welcome to Ahioma Logistics. Link: www.ahioma.com Kindly Login with your email and your password is ("+Input.Password+"). Kindly change your password when you login.");
                    //
                    try
                    {

                        var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                        var mainurllink = urllink.AbsoluteUri;
                        var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                        var lognew = await _log.LogData(Input.Email, "", mainurllink);
                        Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                        _context.Attach(Userlog).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception s)
                    {

                    }
                    //
                    var LogUser = await _userManager.GetUserAsync(User);

                    await _emailSender.SendToOne(LogUser.Email, "New Logistic Company", "Hi",
                      $" A new Account has been Created. with email: " +Input.Email);
                   // await _emailSender.SMSToOne(LogUser.PhoneNumber, "Congratulations, A new Logistic Company with email: " + Input.Email+"has been Created. Your vitual Market");

                    //await _emailSender.SendToMany("onwukaemeka41@gmail.com;info@ahioma.com;Felixobagha@yahoo.com", "New Logistic Company", "Hi",
                    //  $" A new Account has been added to ahioma. with email: " + Input.Email);
                    try
                    {
                        var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                        var mainurllink = urllink.AbsoluteUri;
                        var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == LogUser.Id);
                        var lognew = await _log.LogData(LogUser.UserName, "", mainurllink);
                        Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                        _context.Attach(Userlog).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception s)
                    {

                    }






                    TempData["success"] = "Account created successfully";
                    return RedirectToPage("./AllLogistic");
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
