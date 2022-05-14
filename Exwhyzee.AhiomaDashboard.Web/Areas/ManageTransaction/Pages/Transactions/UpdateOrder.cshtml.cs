using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin")]

    public class UpdateOrderModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IUserLogging _log;

        private readonly IFlutterTransactionService _flutterTransactionAppService;


        public UpdateOrderModel(
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
        [BindProperty]
        public long OrderId { get; set; }
        [BindProperty]
        public string UserID { get; set; }
        [BindProperty]
        public Order Order { get; set; }
        public IList<Transaction> Transactions { get; set; }
        public string Note { get; set; }
        [BindProperty]
        public string FundLocation { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            //Order = await _context.Orders.Include(x => x.Product).Include(x => x.UserProfile).FirstOrDefaultAsync(x => x.Id == id);
            //Transactions = await _context.Transactions.Where(x => x.TrackCode == Order.GroupOrderId).OrderByDescending(x => x.DateOfTransaction).ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updateorder = await _context.Orders.Include(x => x.UserProfile).FirstOrDefaultAsync(x => x.Id == OrderId);
            string AccountSuccess = "";
            if (Order.Status == Enums.OrderStatus.Completed)
            {

                if (FundLocation == "Bank")
                {
                    var transaction = await _context.Transactions.Where(x => x.TrackCode == updateorder.ReferenceId).ToListAsync();
                    foreach (var acc in transaction)
                    {
                        var Profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == acc.UserId);
                        if (!String.IsNullOrEmpty(Profile.AccountNumber))
                        {
                            var Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == Profile.UserId);
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


                            string account_bank = bankcode;
                            string account_number = Profile.AccountNumber;
                            int amount = Convert.ToInt32(acc.Amount);
                            string narration = "Ahioma Transfer";
                            string currency = "NGN";
                            string reference = "";
                            string callback_url = "";
                            string debit_currency = "NGN";

                            var response = await _flutterTransactionAppService.Transfer(account_bank, account_number, amount, narration,
                                currency, reference, callback_url, debit_currency, acc.UserId, "web");

                            try
                            {
                                var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                                var mainurllink = urllink.AbsoluteUri;
                                var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == Profile.UserId);
                                var lognew = await _log.LogData(Profile.User.UserName, "", mainurllink);
                                Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                                _context.Attach(Userlog).State = EntityState.Modified;

                            }
                            catch (Exception s)
                            {

                            }
                           

                                    //acc.PayoutStatus = Enums.PayoutStatus.Bank;
                                    //_context.Entry(acc).State = EntityState.Modified;
                                    //await _context.SaveChangesAsync();

                                    AccountSuccess = AccountSuccess + response;

                                    string email = Profile.User.Email;
                                    string phone = Profile.User.PhoneNumber;
                                    string Title = "Hi " + Profile.Surname;
                                    string SMS = "Your Ahia Pay Transfer.";
                                    string Subject = "Ahia Pay Bank Transfer";
                                    string Message = string.Format("<h4>Transfer to Your Bank from Ahia Pay was Successful. Account credited Immediately with {0}", acc.Amount);

                                    await _emailSender.SendToOne(email, Subject, Title, Message);
                                    await _emailSender.SMSToOne(phone, SMS);

                        }
                        else
                        {
                            AccountSuccess = AccountSuccess + "Transfer to " + Profile.Fullname + " with id (" + Profile.IdNumber + "). failed. account number not registeed";
                        }

                    }
                }
                else if (FundLocation == "Wallet")
                {
                    var transaction = await _context.Transactions.Where(x => x.TrackCode == updateorder.ReferenceId).ToListAsync();
                    foreach (var acc in transaction)
                    {
                        var Profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == acc.UserId);

                        var Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == Profile.UserId);

                        try
                        {
                            var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                            var mainurllink = urllink.AbsoluteUri;
                            var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == Profile.UserId);
                            var lognew = await _log.LogData(Profile.User.UserName, "", mainurllink);
                            Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                            _context.Attach(Userlog).State = EntityState.Modified;

                        }
                        catch (Exception s)
                        {

                        }


                        AhiaPayTransfer transfer = new AhiaPayTransfer();
                        transfer.Amount = acc.Amount;
                        transfer.DateOfTransfer = DateTime.UtcNow.AddHours(1);
                        transfer.Description = "AhiaPay Transfer";
                        transfer.Sender = "Ahioma";
                        transfer.Status = Enums.TransferEnum.Success;
                        transfer.TransferReference = "Wallet";
                        transfer.UserId = Profile.UserId;
                        _context.AhiaPayTransfers.Add(transfer);


                        Wallet.Balance -= acc.Amount;
                        Wallet.WithdrawBalance += acc.Amount;
                        Wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                        _context.Entry(Wallet).State = EntityState.Modified;

                        WalletHistory history1 = new WalletHistory();
                        history1.Amount = acc.Amount;
                        history1.CreationTime = DateTime.UtcNow.AddHours(1);
                        history1.LedgerBalance = Wallet.Balance;
                        history1.AvailableBalance = Wallet.WithdrawBalance;
                        history1.WalletId = Wallet.Id;
                        history1.UserId = Wallet.UserId;
                        history1.UserProfileId = Profile.Id;
                        history1.TransactionType = "Cr";
                        history1.Source = "commission";
                        _context.WalletHistories.Add(history1);

                        //acc.PayoutStatus = Enums.PayoutStatus.Wallet;
                        //_context.Entry(acc).State = EntityState.Modified;
                        //await _context.SaveChangesAsync();

                        AccountSuccess = AccountSuccess + "Transfer to wallet Successfully";

                        string email = Profile.User.Email;
                        string phone = Profile.User.PhoneNumber;
                        string Title = "Hi " + Profile.Surname;
                        string SMS = "Transfer of N" + acc.Amount + " was made to your Ahia Pay Wallet.";
                        string Subject = "Ahia Pay Wallet Transfer";
                        string Message = string.Format("<h4>Transfer to Your Wallet from Ahia Pay was Successful. Account credited Immediately with {0}", acc.Amount);

                        await _emailSender.SendToOne(email, Subject, Title, Message);
                        await _emailSender.SMSToOne(phone, SMS);
                        updateorder.Status = Order.Status;
                        _context.Attach(updateorder).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                    }
                }
            }
            else
            {
                updateorder.Status = Order.Status;
                _context.Attach(updateorder).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            TempData["success"] = "success";
            TempData["msg"] = AccountSuccess;
            // Profile = await _account.GetByUserId(profile.UserId);
            return RedirectToPage("./Orders");
        }

    }


}
