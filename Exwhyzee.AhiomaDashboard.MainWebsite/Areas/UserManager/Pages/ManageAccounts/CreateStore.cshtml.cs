using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "SOA,Admin,mSuperAdmin,CustomerCare")]

    public class CreateStoreModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserLogging _log;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IMarketRepository _market;
        private readonly IHostingEnvironment _hostingEnv;

        public CreateStoreModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserLogging log,
            IHostingEnvironment hostingEnv,
        SignInManager<IdentityUser> signInManager,
            AhiomaDbContext context,
            IMarketRepository market,
            IUserProfileRepository account,
            IEmailSendService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
            _hostingEnv = hostingEnv;
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
            [Required]
            public string PhoneNumber { get; set; }

            public string Surname { get; set; }

            public string FirstName { get; set; }

            public string OtherNames { get; set; }

            public DateTime DateRegistered { get; set; }
            [Required]
            public string TenentHandle { get; set; }
            public string BusinessName { get; set; }
            public string BusinessDescription { get; set; }
            public string LogoUri { get; set; }
            public string BannerUri { get; set; }
            public bool DoYouHaveOtherBranches { get; set; }
            public string UserId { get; set; }
            public IdentityUser User { get; set; }
            public DateTime CreationTime { get; set; }
            public string CreationUserId { get; set; }
            public long MarketId { get; set; }

            public DeliveryEnum DeliveryType { get; set; }
            public TenantEnum TenantStatus { get; set; }
            public PaymentEnum PaymentType { get; set; }
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
        public List<SelectListItem> MarketListing { get; set; }
        public string LoggedInUser { get; set; }


        public async Task<ActionResult> OnGetAsync(string returnUrl = null)
        {
            LoggedInUser = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(LoggedInUser);
            var profile = await _account.GetByUserId(user.Id);
            if (profile.Status != AccountStatus.Active)
            {
                TempData["error"] = profile.Note;
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }

            var state = await _account.GetStates();
            StateListing = state.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.StateName,
                                      Text = a.StateName
                                  }).ToList();

            var mkt = await _market.GetAsyncAll();
            MarketListing = mkt.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id.ToString(),
                                      Text = a.Name
                                  }).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            if (ModelState.IsValid)
            {
                IQueryable<Tenant> checkhandler = from s in _context.Tenants
                                                  .Where(x => x.TenentHandle == Input.TenentHandle)
                                                  select s;
                // var checkhandler = await _context.Tenants.Where(x => x.TenentHandle == Input.TenentHandle).ToListAsync();
                if (checkhandler.Count() > 0)
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
                    MarketListing = mkt.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.Id.ToString(),
                                              Text = a.Name
                                          }).ToList();
                    TempData["error"] = Input.TenentHandle + "Already Taken. Try Another";
                    return Page();
                }
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.PhoneNumber };
                // var check = _userManager.Users.ToList();
                IQueryable<IdentityUser> check = from s in _context.Users
                                                 select s;
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
                    MarketListing = mkt.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.Id.ToString(),
                                              Text = a.Name
                                          }).ToList();
                    TempData["error"] = "Email Already Taken";

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
                    MarketListing = mkt.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.Id.ToString(),
                                              Text = a.Name
                                          }).ToList();
                    TempData["error"] = "Phone Number Already Taken";

                    return Page();
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var Role = await _roleManager.FindByNameAsync("Store");
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

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    UserProfile profile = new UserProfile();
                    profile.UserId = user.Id;
                    profile.DateRegistered = DateTime.UtcNow.AddHours(1);
                    profile.Surname = Input.Surname;
                    profile.FirstName = Input.FirstName;
                    profile.Roles = userroles;
                    profile.OtherNames = Input.OtherNames;
                    profile.DateRegistered = DateTime.UtcNow.AddHours(1);
                    profile.TokenAccount = code;
                    // 
                    AdminShopReferral adminref = new AdminShopReferral();
                    var checkif = await _context.AdminReferrals.FirstOrDefaultAsync(x => x.SubReferalId == Input.CreationUserId);
                    if (checkif != null)
                    {
                        adminref.MainReferalId = checkif.MainReferalId;

                        adminref.Date = DateTime.UtcNow.AddHours(1);
                    }

                    //
                    Tenant tenant = new Tenant();
                    tenant.TenentHandle = Input.TenentHandle;
                    tenant.BusinessName = Input.BusinessName;
                    tenant.BusinessDescription = Input.BusinessDescription;
                    tenant.Commission = 5;
                    tenant.DoYouHaveOtherBranches = Input.DoYouHaveOtherBranches;
                    tenant.UserId = user.Id;
                    tenant.CreationUserId = Input.CreationUserId;
                    tenant.CreationTime = DateTime.UtcNow.AddHours(1);
                    tenant.MarketId = Input.MarketId;
                    tenant.DeliveryType = Input.DeliveryType;
                    tenant.TenantStatus = Input.TenantStatus;
                    tenant.PaymentType = Input.PaymentType;

                    TenantAddress tenantAddress = new TenantAddress();
                    tenantAddress.Address = Input.ContactAddress;
                    tenantAddress.PhoneNumber = Input.AltPhoneNumber;
                    tenantAddress.State = Input.State;
                    tenantAddress.LocalGovernment = Input.LocalGovernment;



                    string id = await _account.CreateAccountTenant(wallet, profile, tenant, tenantAddress, adminref, Input.Facebook, Input.Instagram, Input.LinkedIn, Input.WhatsappNumber);

                    var soauser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == tenant.CreationUserId);

                    var mkt = await _context.Markets.FindAsync(tenant.MarketId);


                   
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //await _emailSender.SendToOne(Input.Email, "Confirm your email", "Hi",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    await _emailSender.SMSToOne(Input.PhoneNumber, "Welcome to Ahioma. Link: www.ahioma.com Kindly Login with your email is (" + Input.Email + ") and your password is (" + Input.Password + "). Kindly change your password when you login.");
                    //

                    var LogUser = await _userManager.GetUserAsync(User);
                    ///welcome soa
                    ///
                    var ahiapayUrl = Url.Page(
                       "/Account/Index",
                       pageHandler: null,
                       values: new { area = "User" },
                       protocol: Request.Scheme);
                    var dashbaord = Url.Page(
                       "/Dashboard/Index",
                       pageHandler: null,
                       values: new { area = "Store" },
                       protocol: Request.Scheme);
                    var handler = Url.Page(
                      "/Index",
                      pageHandler: null,
                      values: new { },
                      protocol: Request.Scheme);
                    var soadashboard = Url.Page(
                      "/Dashboard/Index",
                      pageHandler: null,
                      values: new { area = "SOA" },
                      protocol: Request.Scheme);

                    MailMessage mail = new MailMessage();
                    try
                    {
                        StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "NewSO.html"));
                        //create the mail message 
                        MailAddress addr = new MailAddress(user.Email);
                        string username = addr.User;
                        string domain = addr.Host;

                        string links = $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Mail Confirmation Link</a>.";
                        string mailmsg = sr.ReadToEnd();

                        mailmsg = mailmsg.Replace("{ShopName}", Input.BusinessName);
                        mailmsg = mailmsg.Replace("{shopname}", Input.BusinessName);
                        mailmsg = mailmsg.Replace("{SOEmail}", Input.Email);
                        mailmsg = mailmsg.Replace("{Password}", Input.Password);
                        mailmsg = mailmsg.Replace("{SHOPLINK}", handler + tenant.TenentHandle);
                        mailmsg = mailmsg.Replace("{SOANAME}", soauser.Fullname);
                        mailmsg = mailmsg.Replace("{SOANUMBER}", soauser.User.PhoneNumber);
                        mailmsg = mailmsg.Replace("{link}", links);
                        mailmsg = mailmsg.Replace("{ahiapay}", ahiapayUrl);
                        mailmsg = mailmsg.Replace("{shopdashboard}", dashbaord);

                        mail.Body = mailmsg;
                        sr.Close();
                    }
                    catch (Exception c) { }

                    string adminmsg = "Email " + user.Email + " <br>Roles: " + userroles;
                    await _emailSender.NewSendToOne(user.Email, "Welcome to Ahioma", mail);
                    // await _emailSender.AdminSendToMany("", "Ahioma New User", "", adminmsg);

                    ///soa
                    ///
                    MailMessage soamail = new MailMessage();
                    try
                    {
                        StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "REFNewSO.html"));
                        string mailmsg = sr.ReadToEnd();

                        mailmsg = mailmsg.Replace("|Fname|", soauser.Fullname);
                        mailmsg = mailmsg.Replace("{ShopName}", Input.BusinessName);
                        mailmsg = mailmsg.Replace("{SOEmail}", Input.Email);
                        mailmsg = mailmsg.Replace("{SHOPLINK}", handler + tenant.TenentHandle);
                        mailmsg = mailmsg.Replace("|Phone|", Input.PhoneNumber);
                        mailmsg = mailmsg.Replace("|market location|", mkt.Name);
                        mailmsg = mailmsg.Replace("{ahiapay}", ahiapayUrl);
                        mailmsg = mailmsg.Replace("{soadashboard}", dashbaord);

                        soamail.Body = mailmsg;
                        sr.Close();
                    }
                    catch (Exception c) { }

                    await _emailSender.NewSendToOne(soauser.User.Email, "You Added a New Shop", soamail);

                    //  await _emailSender.SendToOne(LogUser.Email, "New Shop", "Hi",
                    //    $" A new Account has been added to your account. with email: " +Input.Email);
                    ////  await _emailSender.SMSToOne(LogUser.PhoneNumber, "Congratulations, A new shop with email: "+Input.Email+"has been added to your account. Your vitual Market");
                    //  await _emailSender.SMSToOne(user.PhoneNumber, "Congratulations, Your shop with email: " + Input.Email+"has been Created. Check your email and Login. Your vitual Market");

                    // await _emailSender.SendToMany("onwukaemeka41@gmail.com;info@ahioma.com;Felixobagha@yahoo.com", "New Shop", "Hi",
                    //   $" A new Account has been added to ahioma. with email: " + Input.Email);
                    //// await _emailSender.SMSToOne("08068002023,07060530000", "Congratulations, A new shop with email: " + Input.Email + "has been added to your account. Your vitual Market");



                    TempData["success"] = "Account created successfully";
                    return RedirectToPage("./Stores");
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
