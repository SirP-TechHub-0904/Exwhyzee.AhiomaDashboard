using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Balance;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Manager.Pages.Wallets
{
    [Authorize(Roles = "Security,mSuperAdmin")]

    public class UpdateWalletModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IUserLogging _log;

        private readonly IFlutterTransactionService _flutterTransactionAppService;


        public UpdateWalletModel(
                UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IUserLogging log,
                SignInManager<IdentityUser> signInManager,
                IFlutterTransactionService flutterTransactionAppService,
                IUserProfileRepository account, AhiomaDbContext context,
                IEmailSendService emailSender)
        {
            _userManager = userManager;
            _context = context;
            _log = log;
            _flutterTransactionAppService = flutterTransactionAppService;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;

        }
        public Balance Balance { get; set; }
        public decimal TLedgerBalance { get; set; }
        public decimal TWithdrawableBalance { get; set; }

        public decimal FTLedgerBalance { get; set; }
        public decimal FTWithdrawableBalance { get; set; }
        public IList<WalletDto> Wallet { get; set; }
        [BindProperty]
        public string UID { get; set; }
        public async Task<IActionResult> OnGetAsync(string uid)
        {
            UID = uid;
            string trackcart = HttpContext.Session.GetString("UpdateWalletKey");
            //if(trackcart == null)
            //{
            //    return RedirectToPage("./AuthWith2fa", new { uid = uid });
            //}
            var wallet = await _context.Wallets.FirstOrDefaultAsync(x=>x.UserId == uid);
            var allwallets = new WalletDto
            {
                UserId = wallet.UserId,
                DateUpdated = wallet.LastUpdateTime,
                Fullname = NameWallet(wallet.UserId),
                IdNumber = IDNameWallet(wallet.UserId),
                WithdrawBalance = wallet.WithdrawBalance,
                LedgerBalance = wallet.Balance
                
            };
           if(wallet == null)
            {
                return RedirectToPage("./WalletStatusPage", new { status = "notfound" });
            }

            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            HttpContext.Session.Remove("UpdateWalletKey");
            return RedirectToPage("./WalletStatusPage", new { uid = UID });
        }
            private string NameWallet(string id)
        {
            try
            {
                var userprofile = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
                return userprofile.Fullname;
            }catch(Exception k)
            {
                return "";
            }
        }
        private string IDNameWallet(string id)
        {
            try
            {
                var userprofile = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
                return userprofile.IdNumber;
            }
            catch (Exception k)
            {
                return "";
            }
        }
    }
}


