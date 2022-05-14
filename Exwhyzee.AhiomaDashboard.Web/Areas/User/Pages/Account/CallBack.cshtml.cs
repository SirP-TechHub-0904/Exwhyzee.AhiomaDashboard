using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.PayStack;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    public class CallBackModel : PageModel
    {
        private readonly IWalletRepository _walletAppService;
        private readonly IUserProfileRepository _profile;
        private readonly ITransactionRepository _transactionAppService;
        private readonly IPaystackTransactionService _paystackTransactionAppService;
        private readonly IConfiguration _config;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;


        public CallBackModel(ITransactionRepository transactionAppService,
          IPaystackTransactionService paystackTransactionAppService,
          IUserProfileRepository profile, SignInManager<IdentityUser> signInManager,
          UserManager<IdentityUser> userManager, IWalletRepository walletAppService,
          IConfiguration configuration)
        {
            _transactionAppService = transactionAppService;
            _paystackTransactionAppService = paystackTransactionAppService;
            _userManager = userManager;
            _profile = profile;
            _walletAppService = walletAppService;
            _signInManager = signInManager;
            _config = configuration;
        }

       

        [TempData]
        public string StatusMessage { get; set; }

        public decimal Amount { get; set; }

        public int Check { get; set; }

        public decimal WalletTotal { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var callbackUrl = Url.Page(
                       "/Account/Index",
                       pageHandler: null,
                       values: new { area = "User"},
                       protocol: Request.Scheme);
            TempData["url"] = callbackUrl;
            //var secretKey = _config["SecretKey"];
            var secretKey = "sk_test_b2362aab90fdb6d342f39d7f938ce3619c71b231";
            var tranxRef = HttpContext.Request.Query["reference"].ToString();
            if (tranxRef != null)
            {
                var response = await _paystackTransactionAppService.VerifyTransaction(tranxRef, secretKey);

                var id = long.Parse(response.data.metadata.CustomFields.FirstOrDefault(x => x.DisplayName == "Transaction Id").Value);
                var transaction = await _transactionAppService.GetById(id);

                // var user = await _userManager.GetUserAsync(User);

                var wallet = await _walletAppService.GetWallet(transaction.UserId);

                if (response.status)
                {


                    Amount = transaction.Amount;

                    if (transaction == null)
                    {
                        StatusMessage = $"Transaction with Reference {tranxRef} was successful. But Wallet was not updated. Please contact Help Desk.";
                        return Page();
                    }
                    else if (!string.IsNullOrEmpty(transaction.TransactionReference))
                    {
                        StatusMessage = $"Transaction with Reference {tranxRef} was successful.";
                        return Page();
                    }
                    else
                    {

                        WalletTotal = wallet.Balance;
                        transaction.WalletId = wallet.Id;
                        transaction.Status = Enums.EntityStatus.Success;
                        transaction.TransactionReference = tranxRef;
                        var update = await _transactionAppService.Update(transaction);

                        wallet.Balance += transaction.Amount;
                        await _walletAppService.Update(wallet);
                        var walletcurrent = await _walletAppService.GetWallet(transaction.UserId);
                        var user = await _userManager.FindByIdAsync(transaction.UserId);

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        StatusMessage = $"Transaction with Reference {tranxRef} was successful.";

                        string emailMessageBody = StatusMessage + " Your Wimbig Wallet Balance is N" + walletcurrent.Balance + ". Play more @ https://wimbig.com Thanks.";
                        var emailMessage = string.Format("{0};??{1};??{2};??{3}", "Transaction Notification", "Transaction Notification", "Thanks " /*+ user.FullName*/, emailMessageBody);

                        string smsMessage = StatusMessage + " Your Wimbig Wallet Balance is N" + walletcurrent.Balance;

                        //await SendMessage(emailMessage, user.Email, MessageChannel.Email, MessageType.Activation);

                        //await SendMessage(smsMessage, user.PhoneNumber, MessageChannel.SMS, MessageType.Activation);



                        return Page();
                    }


                }
                else
                {
                    transaction.WalletId = wallet.Id;
                    transaction.Status = Enums.EntityStatus.Failed;
                    transaction.TransactionReference = tranxRef;
                    var update = await _transactionAppService.Update(transaction);
                    StatusMessage = $"Transaction with Reference {tranxRef} failed.";
                    return Page();

                }

            }

            return Page();
        }


    }

}
