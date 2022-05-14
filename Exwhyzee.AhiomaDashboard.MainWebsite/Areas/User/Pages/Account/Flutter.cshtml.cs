using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.PayStack;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class FlutterModel : PageModel
    {
     
        private readonly IWalletRepository _walletAppService;
        private readonly IUserProfileRepository _profile;
        private readonly ITransactionRepository _transactionAppService;
        private readonly IFlutterTransactionService _flutterTransactionAppService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserLogging _log;
        private readonly AhiomaDbContext _context;

        public FlutterModel(ITransactionRepository transactionAppService,
            IFlutterTransactionService flutterTransactionAppService,
            IUserLogging log,
            AhiomaDbContext context,
            IUserProfileRepository profile,
            UserManager<IdentityUser> userManager, IWalletRepository walletAppService)
        {
            _transactionAppService = transactionAppService;
            _flutterTransactionAppService = flutterTransactionAppService;
            _userManager = userManager;
            _log = log;
            _context = context;
            _profile = profile;
            _walletAppService = walletAppService;
        }

        [BindProperty]
        [Required, Range(100, 10000000)]
        public decimal Amount { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public Wallet Wallet { get; set; }
        public async Task<ActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var userprofile = await _profile.GetByUserId(user.Id);

            if (userprofile == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Wallet = await _walletAppService.GetWallet(user.Id);
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            try
            {


                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                var userprofile = await _profile.GetByUserId(user.Id);

                if (userprofile == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }
                var wallet = await _walletAppService.GetWallet(user.Id);
                var transaction = await _transactionAppService.CreateTransaction(new Transaction
                {
                    Amount = Amount,
                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
                    Status = EntityStatus.Pending,
                    TransactionType = TransactionTypeEnum.Credit,
                    UserId = user.Id,
                    WalletId = wallet.Id,
                    Description = "Online Transaction"
                });

              
                string amountInnaira = Amount.ToString("0");
                var return_url = Url.Page(
                      "/Account/XyzAhiomaCallLink",
                      pageHandler: null,
                      values: new { area = "User", transactiontype = "deposit", from = "web" },
                      protocol: Request.Scheme);
                //var return_url = Url.Page(
                //     "/Account/FlutterCallBack",
                //     pageHandler: null,
                //     values: new { area = "User" },
                //     protocol: Request.Scheme);
                //string return_url = "https://www.ahioma.com/User/Account/FlutterCallBack";
                string currency = "NGN";
                string tx_id = transaction.Id.ToString();
                string pay_option = "card";
                int consumer_id = (int)userprofile.Id;
                string consumer_mc = GetMACAddress();
                string email = user.Email;
                string phone = user.PhoneNumber;
                string name = userprofile.Fullname;
                string title = "AHIOMA";
                string des = "Ahoma desc";
                string logo = "https://ahioma.com/images/NEW-VF.png";

                var response = await _flutterTransactionAppService.InitializeTransaction(tx_id, amountInnaira, currency, return_url,
                    pay_option, consumer_id, consumer_mc, email, phone, name, title, des, logo, "web");

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
                if (response.status == "success")
                {
                    return Redirect(response.data.link);
                }

                return Page();
            }
            catch (Exception c)
            {
                StatusMessage = $"Erro: " + c;
                return Page();
            }

        }

        public string GetMACAddress()
        {
            string mac_src = "";
            string macAddress = "";

            foreach (System.Net.NetworkInformation.NetworkInterface nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                {
                    mac_src += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            while (mac_src.Length < 12)
            {
                mac_src = mac_src.Insert(0, "0");
            }

            for (int i = 0; i < 11; i++)
            {
                if (0 == (i % 2))
                {
                    if (i == 10)
                    {
                        macAddress = macAddress.Insert(macAddress.Length, mac_src.Substring(i, 2));
                    }
                    else
                    {
                        macAddress = macAddress.Insert(macAddress.Length, mac_src.Substring(i, 2)) + "-";
                    }
                }
            }
            return macAddress;
        }

    }

}
