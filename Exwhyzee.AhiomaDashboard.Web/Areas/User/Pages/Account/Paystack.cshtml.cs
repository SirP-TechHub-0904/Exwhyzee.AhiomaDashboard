using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.PayStack;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class PaystackModel : PageModel
    {
     
        private readonly IWalletRepository _walletAppService;
        private readonly IUserProfileRepository _profile;
        private readonly ITransactionRepository _transactionAppService;
        private readonly IPaystackTransactionService _paystackTransactionAppService;
        private readonly UserManager<IdentityUser> _userManager;

        public PaystackModel(ITransactionRepository transactionAppService,
            IPaystackTransactionService paystackTransactionAppService,
            IUserProfileRepository profile,
            UserManager<IdentityUser> userManager, IWalletRepository walletAppService)
        {
            _transactionAppService = transactionAppService;
            _paystackTransactionAppService = paystackTransactionAppService;
            _userManager = userManager;
            _profile = profile;
            _walletAppService = walletAppService;
        }

        [BindProperty]
        [Required, Range(100, 100000)]
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

                var secretKey = "sk_test_b2362aab90fdb6d342f39d7f938ce3619c71b231";

                int amountInKobo = (int)Amount * 100;

                var response = await _paystackTransactionAppService.InitializeTransaction(secretKey, user.Email, amountInKobo, transaction.Id, userprofile.Surname,
                    userprofile.OtherNames);

                if (response.status == true)
                {
                    return Redirect(response.data.authorization_url);
                }

                return Page();
            }
            catch (Exception c)
            {
                StatusMessage = $"Erro: " + c;
                return Page();
            }

        }


    }

}
