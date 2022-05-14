using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class TransferToBankModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IUserProfileRepository _profile;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFlutterTransactionService _flutterTransactionAppService;
        private readonly IMessageRepository _message;

        public TransferToBankModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, IUserProfileRepository profile,
            IFlutterTransactionService flutterTransactionAppService, UserManager<IdentityUser> userManager, IMessageRepository message)
        {
            _context = context;
            _profile = profile;
            _userManager = userManager;
            _flutterTransactionAppService = flutterTransactionAppService;
            _message = message;
        }
        public Wallet Wallet { get; set; }
        [BindProperty]
        public decimal Amount { get; set; }
       

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
                return Page();
        }



        public long ProfileId { get; set; }
        public UserProfile Profile { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Thread.Sleep(5000);
            var user = await _userManager.GetUserAsync(User);
            
            Profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (Profile.DisableBankTransfer == true)
            {
                TempData["error"] = "You have been Suspended from Transfer";
                return RedirectToPage("/Account/Index", new { Area = "User" });

            }
            if (String.IsNullOrEmpty(Profile.AccountName))
            {
                TempData["error"] = "You have not Added Account Number";
                return RedirectToPage("/Account/Index", new { Area = "User" });

            }
            if (Profile.Status != Enums.AccountStatus.Active)
            {


                TempData["error"] = "Your Account Has Not Been Verified. Kindly Update your Profile and ID cards To Enable Your Withdrawal. Thank You";
                try {

                    AddMessageDto sms = new AddMessageDto();
                    sms.Content = "Your Account Has Not Been Verified " + Profile.Fullname ;
                    sms.Recipient = "onwukaemeka41@gmail.comAhioma";
                    sms.NotificationType = Enums.NotificationType.Email;
                    sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                    sms.Retries = 0;
                    sms.Title = "Your Account Has Not Been Verified " + Profile.Fullname;
                    //
                    var stss = await _message.AddMessage(sms);
                } catch (Exception d) { }
                return RedirectToPage("/Account/Index", new { Area = "User" });

            }

            Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (Amount < 500)
            {
                TempData["error"] = "Minimum Amount is N500.";

                return Page();
            }
            if (Amount > Wallet.WithdrawBalance)
            {
                TempData["error"] = "Insufficient Amount";
               
                return Page();
            }
            
            TempData["amt"] = JsonSerializer.Serialize(Amount);
            return RedirectToPage("./TransferToBankValidation");
        }
    }

}
