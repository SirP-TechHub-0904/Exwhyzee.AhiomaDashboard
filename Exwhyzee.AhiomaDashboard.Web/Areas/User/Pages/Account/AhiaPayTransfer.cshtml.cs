using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class AhiaPayTransferModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWalletRepository _walletAppService;
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;

        private readonly IHostingEnvironment _hostingEnv;

        public AhiaPayTransferModel(IHostingEnvironment env,
            UserManager<IdentityUser> userManger, IEmailSendService emailSender, IWalletRepository walletAppService, Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _hostingEnv = env;
            _userManager = userManger;
            _walletAppService = walletAppService;
            _context = context;
            _emailSender = emailSender;

        }

        public IList<IdentityUser> Users { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public TransferMoney transferMoney { get; set; }

        public class TransferMoney
        {
            public string SenderId { get; set; }
            public string ReceiverId { get; set; }
            [Required]
            public decimal Amount { get; set; }

            public DateTime DateOfTransaction { get; set; }

            public TransactionTypeEnum Status { get; set; }
            [Required]
            public string Note { get; set; }
            public bool OneNairaOneView { get; set; }
            [Required]
            public string PhoneNumber { get; set; }
        }

        public string LoggedInUser { get; set; }

        public string ReturnUrl { get; set; }
        public decimal Balance { get; set; }

        public UserProfile UserProfile { get; set; }
        public async Task OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            LoggedInUser = _userManager.GetUserId(HttpContext.User);
            var userbalance = await _walletAppService.GetWallet(LoggedInUser);
            Balance = userbalance.WithdrawBalance;
            UserProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == LoggedInUser);
            //var users = _userManager.Users.Where(x => x.UserName != "mJinmcever").ToList();
            //Users = users;


        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var check = "";
                    Thread.Sleep(2000);
                    string uid = _userManager.GetUserId(HttpContext.User);
                    var profile = await _userManager.FindByIdAsync(uid);
                    var mprofile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == profile.Id);
                    var senderwallet = await _walletAppService.GetWallet(transferMoney.SenderId);
                    if (mprofile.DisableAhiaPayTransfer == true)
                    {
                        Balance = senderwallet.WithdrawBalance;
                        LoggedInUser = _userManager.GetUserId(HttpContext.User);
                        var users = _userManager.Users.Where(x => x.UserName != "mJinmcever").ToList();
                        Users = users;
                        UserProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == LoggedInUser);
                        StatusMessage = "Error! AhiaPay Transfer Disabled";
                        return Page();
                    }

                    if (transferMoney.PhoneNumber == profile.PhoneNumber)
                    {
                        Balance = senderwallet.WithdrawBalance;
                        LoggedInUser = _userManager.GetUserId(HttpContext.User);
                        var users = _userManager.Users.Where(x => x.UserName != "mJinmcever").ToList();
                        Users = users;
                        UserProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == LoggedInUser);
                        StatusMessage = "Error! You can't transfer to yourself.";
                        return Page();
                    }


                    if (senderwallet == null)
                    {
                        //   var userbalance = await _walletAppService.GetWallet(transferMoney.SenderId);
                        Balance = senderwallet.WithdrawBalance;
                        LoggedInUser = _userManager.GetUserId(HttpContext.User);
                        var users = _userManager.Users.Where(x => x.UserName != "mJinmcever").ToList();
                        Users = users;
                        UserProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == LoggedInUser);
                        StatusMessage = "Error! Your Wallet not found.";
                        return Page();
                    }
                    else
                    {
                        if (senderwallet.WithdrawBalance < transferMoney.Amount)
                        {
                            // var userbalance = await _walletAppService.GetWallet(transferMoney.SenderId);
                            Balance = senderwallet.WithdrawBalance;
                            LoggedInUser = _userManager.GetUserId(HttpContext.User);
                            var users = _userManager.Users.ToList();
                            Users = users;
                            UserProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == LoggedInUser);
                            StatusMessage = "Error! Insufficient balance. Credit your account or send amount below your balance.";
                            return Page();
                        }

                        TempData["phone"] = JsonSerializer.Serialize(transferMoney.PhoneNumber);
                        TempData["note"] = JsonSerializer.Serialize(transferMoney.Note);
                        TempData["amt"] = JsonSerializer.Serialize(transferMoney.Amount);
                        return RedirectToPage("./ConfirmAhiaPayTransfer");
                        
                    }
                   
                    return RedirectToPage("Index");
                }

                return Page();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<JsonResult> OnGetFullname(string phone)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);

                string da = profile.Fullname + " (" + profile.IdNumber + ")";
                return new JsonResult(da);
            }
            catch (Exception k)
            {
                return new JsonResult("not found or wrong number");
            }
        }
        private string Token(byte Length)
        {
            char[] Chars = new char[] {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };
            string String = string.Empty;
            Random Random = new Random();

            for (byte a = 0; a < Length; a++)
            {
                String += Chars[Random.Next(0, 10)];
            };

            return (String);
        }

    }
}
