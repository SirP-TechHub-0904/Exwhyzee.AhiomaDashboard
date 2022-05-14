using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    public class ReverseTransferModel : PageModel
    {

        private readonly AhiomaDbContext _context;
        private readonly IFlutterTransactionService _flutterTransactionAppService;
        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSendService _emailSender;

        public ReverseTransferModel(
          IFlutterTransactionService flutterTransactionAppService,
          IEmailSendService emailSender,
          AhiomaDbContext context,
         
          UserManager<IdentityUser> userManager)
        {
           
            _flutterTransactionAppService = flutterTransactionAppService;
            _userManager = userManager;
           
            _context = context;
            _emailSender = emailSender;
        }
        [BindProperty]
        public string TransactionId { get; set; }
        public Wallet Wallet { get; set; }
        [BindProperty]
        public decimal Amount { get; set; }
        [BindProperty]
        public decimal iAmount { get; set; }

        public async Task<IActionResult> OnGetAsync(string uid, int tid)
        {
            var user = await _userManager.FindByIdAsync(uid);
            var Profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var transferitem = await _context.AhiaPayTransfers.FirstOrDefaultAsync(x => x.Id == tid);
            AhiaPayTransfer transfer = new AhiaPayTransfer();
            transfer.Amount = transferitem.Amount;
            transfer.DateOfTransfer = DateTime.UtcNow.AddHours(1);
            transfer.Description = "Reverse AhiaPay Transfer";
            transfer.Sender = "Ahioma";
            transfer.Status = Enums.TransferEnum.Reversed;
            transfer.TransferReference = transferitem.TransferReference;
            transfer.UserId = user.Id;
            _context.AhiaPayTransfers.Add(transfer);
            await _context.SaveChangesAsync();


            Wallet.WithdrawBalance += transfer.Amount;
            Wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
            _context.Entry(Wallet).State = EntityState.Modified;


            WalletHistory history1 = new WalletHistory();
            history1.Amount = transfer.Amount;
            history1.CreationTime = DateTime.UtcNow.AddHours(1);
            history1.LedgerBalance = Wallet.Balance;
            history1.AvailableBalance = Wallet.WithdrawBalance;
            history1.WalletId = Wallet.Id;
            history1.UserId = Wallet.UserId; 
            history1.TransactionType = "Cr";

            history1.UserProfileId = Profile.Id;
            history1.Source = "reserve transfer";
            _context.WalletHistories.Add(history1);

            await _context.SaveChangesAsync();

            TempData["success"] = "Transfer Processed Successfully";

            string email = user.Email;
            string phone = user.PhoneNumber;
            string Title = "Hi " + Profile.Surname;
            string SMS = "Your Ahia Pay Reverse.";
            string Subject = "Ahia Pay Bank Reverse of N"+ transfer.Amount +" was successfull";
            string Message = string.Format("<h4>Reversal to Your Wallet was Successful. wallet credited Immediately with {0}", transfer.Amount);

            await _emailSender.SendToOne(email, Subject, Title, Message);
            await _emailSender.SMSToOne(phone, SMS);



            return RedirectToPage("./AllTransfer");
        }
        
        //public async Task<IActionResult> OnPostAsync()
        //{
        //    var response = await _flutterTransactionAppService.VerifyTransaction(TransactionId);

        //    return Page();
        //}
        }
}
