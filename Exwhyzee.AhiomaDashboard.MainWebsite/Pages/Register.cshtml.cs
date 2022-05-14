using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
//using System.Management;
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
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserLogging _log;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IMessageRepository _message;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserLogging log,
            AhiomaDbContext context,
            SignInManager<IdentityUser> signInManager,
            IUserProfileRepository account, IHostingEnvironment hostingEnv,
            IEmailSendService emailSender, IMessageRepository message)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _account = account;
            _context = context;
            _log = log;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _hostingEnv = hostingEnv;
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

            //public string Surname { get; set; }

            //public string FirstName { get; set; }

            //public string OtherNames { get; set; }

            public DateTime DateRegistered { get; set; }
            public string Role { get; set; }

            //[Display(Name = "Next of Kin")]
            //public string NextOfKin { get; set; }
            //[Display(Name = "Next of Kin Phone Number")]
            //public string NextOfKinPhoneNumber { get; set; }

            //[Display(Name = "Referee Name")]
            //public string RefereeName { get; set; }

            //[Display(Name = "Referee Phone")]
            //public string RefereePhone { get; set; }

            //[Display(Name = "Contact Address")]
            //public string ContactAddress { get; set; }
            //[Display(Name = "Alt Phone Number")]
            //public string AltPhoneNumber { get; set; }


            //public string State { get; set; }
            //public string LocalGovernment { get; set; }
        }

        public List<SelectListItem> RoleListing { get; set; }
public List<SelectListItem> StateListing { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task OnGetAsync(string customerRef, string returnUrl = null)
        {
            CustomerRef = customerRef;
            ReturnUrl = returnUrl;

           

            var list = await _roleManager.Roles.Where(x=>x.Name.Contains("SOA") || x.Name.Contains("Customer")).ToListAsync();
            RoleListing = list.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id,
                                      Text = a.Name
                                  }).ToList();

            var state = await _account.GetStates();
            StateListing = state.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.StateName,
                                      Text = a.StateName
                                  }).ToList();
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var check = _userManager.Users.ToList();

                if (check.Select(x => x.Email).Contains(Input.Email))
                {
                    TempData["error"] = "Email already taken";

                    return Page();
                }

                if (check.Select(x => x.PhoneNumber).Contains(Input.PhoneNumber))
                {
                    TempData["error"] = "Phone Number Already Taken";

                    return Page();
                }
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email, PhoneNumber = Input.PhoneNumber, LockoutEnabled= false };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var Role = await _roleManager.FindByIdAsync(Input.Role);
                    var CustomerRole = await _roleManager.FindByNameAsync("Customer");
                    if (CustomerRole != null)
                    {
                  
                            await _userManager.AddToRoleAsync(user, CustomerRole.Name);
                        
                      
                    }
                    var Userroles = "";
                    try
                    {
                         Userroles = await _account.FetchUserRoles(user.Id);
                    }catch(Exception c) { }
                    // _logger.LogInformation("User created a new account with password.");
                    //create wallet
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    Wallet wallet = new Wallet();
                    wallet.UserId = user.Id;
                    wallet.Balance = 0;
                    wallet.WithdrawBalance = 0;
                    wallet.CreationTime = DateTime.UtcNow.AddHours(1);
                    wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                   

                    UserProfile profile = new UserProfile();
                    profile.UserId = user.Id;
                    profile.DateRegistered = DateTime.UtcNow.AddHours(1);
                    profile.Roles = Userroles;
                    profile.Status = Enums.AccountStatus.Pending;
                    profile.TokenAccount = code;

                    UserAddress address = new UserAddress();
                    address.UserId = user.Id;

                    UserReferee referee = new UserReferee();
                    referee.UserId = user.Id;
                   

                    string id = await _account.CreateAccount(wallet, profile, address, referee, null);
                   
                    
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
                    MailMessage mail = new MailMessage();
                    try
                    {
                        StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "NewCustomer.html"));
                        //create the mail message 
                        MailAddress addr = new MailAddress(user.Email);
                        string username = addr.User;
                        string domain = addr.Host;
                       
                        string links = $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Mail Confirmation Link</a>.";
                        string mailmsg = sr.ReadToEnd();
                        mailmsg = mailmsg.Replace("{customer}", username);
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
                    catch(Exception c) { }

                    //string adminmsg = "Email "+ user.Email +" <br>Roles: "+ Userroles;
                    //await _emailSender.NewSendToOne(user.Email, "Welcome to Ahioma", mail);
                    //await _emailSender.AdminSendToMany("", "Ahioma New User", "", adminmsg);
                   // await _emailSender.SMSToOne(user.PhoneNumber, "Welcome to ahioma. Your vitual Market.\r\n Kindly check your email for more details");

                  

                    AddMessageDto emails = new AddMessageDto();
                    emails.Content = "newuser name: " + user.Email;
                    emails.Recipient = "onwukaemeka41@gmail.comAhioma";
                    emails.NotificationType = Enums.NotificationType.Email;
                    emails.NotificationStatus = Enums.NotificationStatus.NotSent;
                    emails.Retries = 0;
                    emails.Title = "new user";

                    var stsl = await _message.AddMessage(emails);
                    await _signInManager.SignInAsync(user, isPersistent: false);
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

                    return RedirectToPage("./Dashboard/Index", new { area = "Customer" });

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
