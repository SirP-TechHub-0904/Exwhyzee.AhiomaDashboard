
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    [AllowAnonymous]
    public class SoaModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IUserLogging _log;
        private readonly AhiomaDbContext _context;
        private readonly IMessageRepository _message;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="account"></param>
        public SoaModel(
            UserManager<IdentityUser> userManager,
            IHostingEnvironment hostingEnv,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IUserProfileRepository account,
            IUserLogging log,
            AhiomaDbContext context,
            IEmailSendService emailSender, IMessageRepository message)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _hostingEnv = hostingEnv;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _log = log;
            _context = context;
            _message = message;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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

            //[DataType(DataType.Password)]
            //[Display(Name = "Confirm password")]
            //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            //public string ConfirmPassword { get; set; }
            [Required]
            public string PhoneNumber { get; set; }

            public string Surname { get; set; }

            public string FirstName { get; set; }

            public string OtherNames { get; set; }

            public DateTime DateRegistered { get; set; }
          

            [Display(Name = "Next of Kin")]
            public string NextOfKin { get; set; }
            [Display(Name = "Next of Kin Phone Number")]
            public string NextOfKinPhoneNumber { get; set; }

            [Display(Name = "Referee Name")]
            public string RefereeName { get; set; }

            [Display(Name = "Referee Phone")]
            public string RefereePhone { get; set; }

            [Display(Name = "Contact Address")]
            public string ContactAddress { get; set; }
            [Display(Name = "Alt Phone Number")]
            public string AltPhoneNumber { get; set; }


            public string State { get; set; }
            public string LocalGovernment { get; set; }
            [Display(Name = "Profile Image")]
            public string ProfileUrl { get; set; }
            [Display(Name = "Id Card Front")]
            public string IDCardFront { get; set; }
            [Display(Name = "Id Card Back")]
            public string IDCardBack { get; set; }
            [Display(Name = "Referral Id")]
            [Required]
             public string ReferralLink { get; set; }

        }

        public List<SelectListItem> RoleListing { get; set; }
        public List<SelectListItem> StateListing { get; set; }
        public string fullname { get; set; }
        public string IDNumber { get; set; }
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string referralid, string returnUrl = null)
        {
           
            ReturnUrl = returnUrl;
            var check = _userManager.Users.ToList();
            var profcheck = await _account.GetUsersInRole("SOA");

            if (!String.IsNullOrEmpty(referralid))
            {
                if (profcheck.Select(x => x.IdNumber).Contains(referralid))
                {
                    var pro = await _context.UserProfiles.FirstOrDefaultAsync(x => x.IdNumber == referralid);
                    fullname = pro.Fullname;
                    IDNumber = pro.IdNumber;
                }
               
            }

            //var list = await _roleManager.Roles.Where(x => x.Name.Contains("SOA") || x.Name.Contains("Customer")).ToListAsync();
            //RoleListing = list.Select(a =>
            //                      new SelectListItem
            //                      {
            //                          Value = a.Id,
            //                          Text = a.Name
            //                      }).ToList();

            //var state = await _account.GetStates();
            //StateListing = state.Select(a =>
            //                      new SelectListItem
            //                      {
            //                          Value = a.StateName,
            //                          Text = a.StateName
            //                      }).ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var check = _userManager.Users.ToList();
                var profcheck = await _account.GetUsersInRole("SOA");


                if (profcheck.Select(x => x.IdNumber).Contains(Input.ReferralLink))
                {

                }
                else
                {
                    TempData["error"] = "Referral Id Not a Valid SOA Id";
                    var state = await _account.GetStates();
                    StateListing = state.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.StateName,
                                              Text = a.StateName
                                          }).ToList();
                    return Page();
                }
                if (check.Select(x => x.Email).Contains(Input.Email))
                {
                    TempData["error"] = "Email Already Taken";
                    var state = await _account.GetStates();
                    StateListing = state.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.StateName,
                                              Text = a.StateName
                                          }).ToList();
                    return Page();
                }

                if (check.Select(x => x.PhoneNumber).Contains(Input.PhoneNumber))
                {
                    TempData["error"] = "Phone Number Already Taken";
                    var state = await _account.GetStates();
                    StateListing = state.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.StateName,
                                              Text = a.StateName
                                          }).ToList();
                    return Page();
                }
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.PhoneNumber, LockoutEnabled = false };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                      var CustomerRole = await _roleManager.FindByNameAsync("Customer");
                      var SOA = await _roleManager.FindByNameAsync("SOA");
                    if (CustomerRole != null)
                    {
                         await _userManager.AddToRoleAsync(user, CustomerRole.Name);
                       
                    }
                    if (SOA != null)
                    {
                        await _userManager.AddToRoleAsync(user, SOA.Name);

                    }
                    var Userroles = "";
                    try
                    {
                        Userroles = await _account.FetchUserRoles(user.Id);
                    }
                    catch (Exception c) { }
                    // _logger.LogInformation("User created a new account with password.");
                    //create wallet
                    Wallet wallet = new Wallet();
                    wallet.UserId = user.Id;
                    wallet.Balance = 0;
                    wallet.WithdrawBalance = 0;
                    wallet.CreationTime = DateTime.UtcNow.AddHours(1);
                    wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                    AdminReferral referralsystem = new AdminReferral();
                    var getacc = await _context.UserProfiles.FirstOrDefaultAsync(x => x.IdNumber == Input.ReferralLink);
                    if(getacc != null)
                    {
                        var checkif = await _context.AdminReferrals.FirstOrDefaultAsync(x => x.SubReferalId == getacc.UserId);
                        if (checkif != null)
                        {
                            referralsystem.MainReferalId = checkif.MainReferalId;
                            referralsystem.SubReferalId = user.Id;
                            referralsystem.Date = DateTime.UtcNow.AddHours(1);
                        }
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    UserProfile profile = new UserProfile();
                    profile.UserId = user.Id;
                    profile.DateRegistered = DateTime.UtcNow.AddHours(1);
                    profile.Roles = Userroles;
                    profile.Status = Enums.AccountStatus.Pending;
                    profile.Note = "Account Awaiting Approval and verification";
                    profile.Surname = Input.Surname;
                    profile.FirstName = Input.FirstName;
                    profile.ReferralLink = Input.ReferralLink;
                    profile.TokenAccount = code;
                    profile.OtherNames = Input.OtherNames;

                    UserAddress address = new UserAddress();
                    address.UserId = user.Id;
                    //address.Address = Input.ContactAddress;
                    //address.State = Input.State;
                    //address.LocalGovernment = Input.LocalGovernment;

                    UserReferee referee = new UserReferee();
                    referee.UserId = user.Id;
                    //referee.FullName = Input.RefereeName;
                    //referee.PhoneNumber = Input.RefereePhone;

                   
                    string id = await _account.CreateAccount(wallet, profile, address, referee, referralsystem);
                    if (id != null)
                    {
                      
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);
                        var ahiapayUrl = Url.Page(
                       "/Account/Index",
                       pageHandler: null,
                       values: new { area = "User" },
                       protocol: Request.Scheme);
                       
                        try
                        {
                            MailMessage mail = new MailMessage();
                            StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "NewSOA.html"));
                            //create the mail message 
                           
                            string links = $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Mail Confirmation Link</a>.";
                            string mailmsg = sr.ReadToEnd();
                            mailmsg = mailmsg.Replace("|Fname|", profile.Fullname);
                            mailmsg = mailmsg.Replace("{link}", links);
                            mailmsg = mailmsg.Replace("{linkhref}", callbackUrl);
                            mailmsg = mailmsg.Replace("{ahiapay}", ahiapayUrl);

                            mail.Body = mailmsg;
                            sr.Close();

                            AddMessageDto i = new AddMessageDto();
                            i.Content = mailmsg;
                            i.Recipient = user.Email;
                            i.NotificationType = Enums.NotificationType.Email;
                            i.NotificationStatus = Enums.NotificationStatus.NotSent;
                            i.Retries = 0;
                            i.Title = "Account Confirmation";
                            var stsa = await _message.AddMessage(i);
                        }
                        catch (Exception c) { }
                        //await _emailSender.SendToOne(Input.Email, "Confirm your email", "Hi",
                        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                        //await _emailSender.SMSToOne(user.PhoneNumber, "Welcome to Ahioma, where you earn from every shop. Your vitual Market");

                        try
                        {
                            MailMessage mail = new MailMessage();
                            StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "SOAREF.html"));
                            //create the mail message 
                            var refby = await _context.UserProfiles.FirstOrDefaultAsync(x => x.IdNumber == profile.ReferralLink);
                         
                            string mailmsg = sr.ReadToEnd();
                            mailmsg = mailmsg.Replace("|Fname|", refby.Fullname);
                            mailmsg = mailmsg.Replace("|RegName|", profile.Fullname);
                            
                            mail.Body = mailmsg;
                            sr.Close();

                            AddMessageDto i = new AddMessageDto();
                            i.Content = mailmsg;
                            i.Recipient = user.Email;
                            i.NotificationType = Enums.NotificationType.Email;
                            i.NotificationStatus = Enums.NotificationStatus.NotSent;
                            i.Retries = 0;
                            i.Title = "New Downline";
                            var stsa = await _message.AddMessage(i);
                        }
                        catch (Exception c) { }

                        await _signInManager.SignInAsync(user, isPersistent: false);
                       
                    }
                    TempData["error"] = "Account Created With Error. Contact Us Now";
                        return RedirectToPage("./Initializing");
                    
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
