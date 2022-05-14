using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]


    public class DataModel
    {
       public string account_bank { get; set; }
        public string account_number { get; set; }
        public int amount { get; set; }
        public string narration { get; set; }
        public string currency { get; set; }
        public string reference { get; set; }
        public string callback_url { get; set; }
        public string debit_currency { get; set; }
    }
    public class ComfirmTransferModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IUserProfileRepository _profile;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFlutterTransactionService _flutterTransactionAppService;
        private readonly IUserLogging _log;
        private readonly IWalletRepository _walletAppService;
        private readonly IEmailSendService _emailSender;
        private readonly ITransactionRepository _transactionAppService;



        public ComfirmTransferModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, ITransactionRepository transactionAppService, IUserLogging log, IWalletRepository walletAppService, IUserProfileRepository profile,
            IFlutterTransactionService flutterTransactionAppService, UserManager<IdentityUser> userManager, IEmailSendService emailSender)
        {
            _context = context;
            _profile = profile;
            _userManager = userManager;
            _log = log;
            _walletAppService = walletAppService;
            _transactionAppService = transactionAppService;
            _emailSender = emailSender;
            _flutterTransactionAppService = flutterTransactionAppService;
        }
        public Wallet Wallet { get; set; }
        [BindProperty]
        public decimal Amount { get; set; }
        [BindProperty]
        public decimal iAmount { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            Profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);

            Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var data = TempData["comfirmamt"] as string;

            if(data == null)
            {
                return RedirectToPage("./Invalid");
            }

            var result = JsonSerializer.Deserialize<decimal>(data);
            iAmount = result;
            return Page();
        }



        public long ProfileId { get; set; }
        public UserProfile Profile { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            Profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
            string bankcode = "";
            var bankname = await _flutterTransactionAppService.GetBanks();
            foreach (var i in bankname.data)
            {
                if (i.name == Profile.BankName)
                {
                    bankcode = i.code;
                    break;
                }
            }
            //
            Thread.Sleep(3000);
            Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (Amount < 500)
            {
                TempData["error"] = "Minimum Amount is N500.";

                return RedirectToPage("/Account/TransferToBank", new { area = "User" });
            }
            if (Amount > Wallet.WithdrawBalance)
            {
                TempData["error"] = "Insufficient Amount";

                return RedirectToPage("/Account/TransferToBank", new { area = "User" });
            }

            DataModel data = new DataModel();
            data.account_bank = bankcode;
            data.account_number = Profile.AccountNumber;
            data.amount = Convert.ToInt32(Amount);
            data.narration = "Ahioma Transfer";
            data.currency = "NGN";
            data.reference = "";
            data.callback_url = "";
            data.debit_currency = "NGN";

            TempData["paydata"] = JsonSerializer.Serialize(data);
            return RedirectToPage("./ProcessPay");

            //var response = await _flutterTransactionAppService.Transfer(account_bank, account_number, amount, narration,
            //    currency, reference, callback_url, debit_currency);


            //try
            //{
            //    var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            //    var mainurllink = urllink.AbsoluteUri;
            //    var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            //    var lognew = await _log.LogData(user.UserName, "", mainurllink);
            //    Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
            //    _context.Attach(Userlog).State = EntityState.Modified;

            //}
            //catch (Exception s)
            //{

            //}
            //if (response != null)
            //{

            //    if (response.status == "success")
            //    {


            //        AhiaPayTransfer transfer = new AhiaPayTransfer();
            //        transfer.Amount = Amount;
            //        transfer.DateOfTransfer = DateTime.UtcNow.AddHours(1);
            //        transfer.Description = "AhiaPay Transfer";
            //        transfer.Sender = "Ahioma";
            //        transfer.Status = Enums.TransferEnum.Success;
            //        transfer.TransferReference = response.data.reference;
            //        transfer.UserId = user.Id;
            //        _context.AhiaPayTransfers.Add(transfer);
            //        await _context.SaveChangesAsync();


            //        Wallet.WithdrawBalance -= Amount;
            //        Wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
            //        _context.Entry(Wallet).State = EntityState.Modified;
            //        //wallet histiory
            //        WalletHistory history = new WalletHistory();
            //        history.Amount = Amount;
            //        history.CreationTime = DateTime.UtcNow.AddHours(1);
            //        history.LedgerBalance = Wallet.Balance;
            //        history.AvailableBalance = Wallet.WithdrawBalance;
            //        history.WalletId = Wallet.Id;
            //        history.UserId = Wallet.UserId;
            //        history.UserProfileId = Profile.Id;
            //        history.TransactionType = "Dr";
            //        history.Source = "AhiaPay Transfer to bank";
            //        _context.WalletHistories.Add(history);
            //        await _context.SaveChangesAsync();

            //        var newtransaction = await _transactionAppService.CreateTransaction(new Transaction
            //        {
            //            Amount = Amount,
            //            DateOfTransaction = DateTime.UtcNow.AddHours(1),
            //            Status = EntityStatus.Success,
            //            TransactionType = TransactionTypeEnum.Debit,
            //            Note = "Online Order",
            //            UserId = user.Id,
            //            TrackCode = response.data.reference,
            //            WalletId = Wallet.Id,
            //            TransactionShowEnum = TransactionShowEnum.Off,
            //            TransactionSection = TransactionSection.Order,
            //            Description = "Transfer to bank"
            //        });

            //        TempData["success"] = "Transfer Queued Successfully";

            //        string email = user.Email;
            //        string phone = user.PhoneNumber;
            //        string Title = "Hi " + Profile.Surname;
            //        string SMS = "Your Ahia Pay Transfer.";
            //        string Subject = "Ahia Pay Bank Transfer";
            //        string Message = string.Format("<h4>Transfer to Your Bank from Ahia Pay was Successful. Account credited Immediately with {0}", Amount);

            //        await _emailSender.SendToOne(email, Subject, Title, Message);
            //        await _emailSender.SMSToOne(phone, SMS);



            //        return RedirectToPage("./Index");

            //    }
            //    else
            //    {
            //        AhiaPayTransfer transfer = new AhiaPayTransfer();
            //        transfer.Amount = Amount;
            //        transfer.DateOfTransfer = DateTime.UtcNow.AddHours(1);
            //        transfer.Description = "AhiaPay Transfer";
            //        transfer.Sender = "Ahioma";
            //        transfer.Status = Enums.TransferEnum.Fail;
            //        transfer.TransferReference = "";
            //        transfer.UserId = user.Id;
            //        _context.AhiaPayTransfers.Add(transfer);
            //        await _context.SaveChangesAsync();
            //        TempData["error"] = "Transfer Not Successful, Kindly Update Account and try again";
            //        return RedirectToPage("./Index");
            //    }
            //}
            //else
            //{
            //    AhiaPayTransfer transfer = new AhiaPayTransfer();
            //    transfer.Amount = Amount;
            //    transfer.DateOfTransfer = DateTime.UtcNow.AddHours(1);
            //    transfer.Description = "AhiaPay Transfer";
            //    transfer.Sender = "Ahioma";
            //    transfer.Status = Enums.TransferEnum.Fail;
            //    transfer.TransferReference = "";
            //    transfer.UserId = user.Id;
            //    _context.AhiaPayTransfers.Add(transfer);
            //    await _context.SaveChangesAsync();
            //    TempData["error"] = "Not Successful. Try again";
            //    return RedirectToPage("./Index");
            //}
        }
    }
}
