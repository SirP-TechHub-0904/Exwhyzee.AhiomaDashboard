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

    public class ConfirmAhiaPayTransferModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWalletRepository _walletAppService;
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;

        private readonly IHostingEnvironment _hostingEnv;

        public ConfirmAhiaPayTransferModel(IHostingEnvironment env,
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
            public string Fullname { get; set; }
        }

        public string LoggedInUser { get; set; }

        public string ReturnUrl { get; set; }
        public decimal Balance { get; set; }
        [BindProperty]
        public decimal Amount { get; set; }
        [BindProperty]
        public string PhoneNumber { get; set; }
        [BindProperty]
        public string Note { get; set; }
        [BindProperty]
        public string Fullname { get; set; }

        public UserProfile UserProfile { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var phone = TempData["phone"] as string;

            if (phone == null)
            {
                return RedirectToPage("./Invalid");
            }
            var note = TempData["note"] as string;

            if (note == null)
            {
                return RedirectToPage("./Invalid");
            }
            var amt = TempData["amt"] as string;

            if (amt == null)
            {
                return RedirectToPage("./Invalid");
            }
            try
            {
                var iamount = JsonSerializer.Deserialize<decimal>(amt);
                var iphone = JsonSerializer.Deserialize<string>(phone);
                var inote = JsonSerializer.Deserialize<string>(note);
                var Ruser = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == iphone);
                var infouser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == Ruser.Id);

                Amount = iamount;
                PhoneNumber = iphone;
                Note = inote;
                Fullname = infouser.Fullname;

                LoggedInUser = _userManager.GetUserId(HttpContext.User);
                var userbalance = await _walletAppService.GetWallet(LoggedInUser);
                Balance = userbalance.WithdrawBalance;
                UserProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == LoggedInUser);
                //var users = _userManager.Users.Where(x => x.UserName != "mJinmcever").ToList();
                //Users = users;
                return Page();
            }catch(Exception d)
            {

            }
            return RedirectToPage("./Invalid");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var check = "";
                    Thread.Sleep(1000);
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
                        //if (transferMoney.Amount > 10000)
                        //{
                        //    var userbalance = await _walletAppService.GetWallet(transferMoney.SenderId);
                        //    Balance = userbalance.WithdrawBalance;
                        //    LoggedInUser = _userManager.GetUserId(HttpContext.User);
                        //    var users = _userManager.Users.Where(x => x.UserName != "mJinmcever").ToList();
                        //    Users = users;
                        //    StatusMessage = "Error! cannot wimpay above #10000.";
                        //    return Page();
                        //}
                    }
                    var Ruser = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == transferMoney.PhoneNumber);

                    var receiverVerify = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == Ruser.Id);
                    var receiverwallet = await _walletAppService.GetWallet(receiverVerify.UserId);

                    //if (receiverVerify.User.PhoneNumber != transferMoney.PhoneNumber)
                    //{

                    //    Balance = receiverwallet.WithdrawBalance;
                    //    LoggedInUser = _userManager.GetUserId(HttpContext.User);
                    //    //var users = _userManager.Users.Where(x => x.UserName != "mJinmcever").ToList();
                    //    //Users = users;
                    //    UserProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == LoggedInUser);
                    //    StatusMessage = "Error! Unable to verify User. Confirm phone number";
                    //    return Page();
                    //}
                    string date1 = DateTime.UtcNow.AddHours(1).ToString("ssfff");

                    // The random number sequence
                    Random num = new Random();

                    // Create new string from the reordered char array
                    string rand = new string(date1.ToCharArray().
                                    OrderBy(s => (num.Next(2) % 2) == 0).ToArray());

                    var code = Token(5);
                    //
                    var xxx = date1 + code;
                    string TokenTracker = xxx;
                    string xNumber = new string(TokenTracker.ToCharArray().
                                    OrderBy(s => (num.Next(2) % 2) == 0).ToArray());
                    var TransactionCode = xNumber.Substring(1, 8).ToUpper();
                   
                    var ahiapay = new AhiapayDto
                    {
                        Sender = transferMoney.SenderId,
                        ReceiverId = receiverVerify.UserId,
                        Amount = transferMoney.Amount,
                        Note = transferMoney.Note,
                        TransactionCode = TransactionCode,
                        ReceiverPhone = transferMoney.PhoneNumber,
                        Senderwalletid = senderwallet.Id,
                        Receiverwalletid = receiverwallet.Id,
                        From = "web"
                    };


                    var result = await _walletAppService.CreateAhiaPay(ahiapay);

                    var senderProfile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == senderwallet.UserId);
                    var receiverProfile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == receiverwallet.UserId);
                    var sendwallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == senderwallet.UserId);
                    var receivewallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == receiverwallet.UserId);
                    var transactionResult = await _context.Transactions.FirstOrDefaultAsync(x => x.TrackCode == ahiapay.TransactionCode);
                    var sEmail = "";
                    var sTitle = "";
                    var sMessageSubject = "";
                    var sSMS = "";
                    var sPhone = "";
                    var sMessage = "";
                    //

                    var REmail = "";
                    var RTitle = "";
                    var RMessageSubject = "";
                    var RPhone = "";
                    var RSMS = "";
                    var RMessage = "";
                    if (result == 1)
                    {
                        sEmail = senderProfile.User.Email;
                        sTitle = "Hi, " + senderProfile.Fullname;
                        sMessageSubject = "AhiaPay Debit Alert";

                        sPhone = senderProfile.User.PhoneNumber;

                        sMessage = "<table border=\"1\" width=\"40%\"><tr><td colspan=\"2\"><strong>AhiaPay Dr</strong></td></tr><tr><td>From</td><td>" + senderProfile.User.PhoneNumber + "</td></tr><tr><td>To</td><td>" + ahiapay.ReceiverPhone + "</td></tr><tr><td>Amount</td><td>" + ahiapay.Amount + "</td></tr><tr><td>Reference</td><td>" + ahiapay.TransactionCode + "</td></tr><tr><td>Description</td><td>" + ahiapay.Note + "</td></tr><tr><td>Date</td><td>" + transactionResult.DateOfTransaction + "</td></tr><tr><td>Balance</td><td>" + sendwallet.WithdrawBalance + "</td></tr></table>";

                        sSMS = "AhiaPay Dr.\r\n From: " + senderProfile.User.PhoneNumber + "\r\n To: " + ahiapay.ReceiverPhone + "\r\n Amt: " + ahiapay.Amount + "\r\n Ref: " + ahiapay.TransactionCode + "\r\n Desc: " + ahiapay.Note + " \r\n Date: " + transactionResult.DateOfTransaction.ToShortDateString() + " \r\n Bal: " + sendwallet.WithdrawBalance;



                        await _emailSender.SendToOne(sEmail, sMessageSubject, sTitle, sMessage);
                        if (transferMoney.OneNairaOneView == false)
                        {
                            await _emailSender.SMSToOne(sPhone, sSMS);
                        }
                        //

                        REmail = receiverProfile.User.Email;
                        RTitle = "Hi, " + receiverProfile.Fullname;
                        RMessageSubject = "AhiaPay Credit Alert";
                        RPhone = receiverProfile.User.PhoneNumber;
                        RMessage = "<table border=\"1\" width=\"40%\"><tr><td colspan=\"2\"><strong>AhiaPay Cr</strong></td></tr><tr><td>From</td><td>" + senderProfile.User.PhoneNumber + "</td></tr><tr><td>To</td><td>" + ahiapay.ReceiverPhone + "</td></tr><tr><td>Amount</td><td>" + ahiapay.Amount + "</td></tr><tr><td>Reference</td><td>" + ahiapay.TransactionCode + "</td></tr><tr><td>Description</td><td>" + ahiapay.Note + "</td></tr><tr><td>Date</td><td>" + transactionResult.DateOfTransaction + "</td></tr><tr><td>Balance</td><td>" + receiverwallet.WithdrawBalance + "</td></tr></table>";

                        RSMS = "AhiaPay Cr.\r\n From" + senderProfile.User.PhoneNumber + "\r\n To: " + ahiapay.ReceiverPhone + "\r\n Amt: " + ahiapay.Amount + "\r\n Ref: " + ahiapay.TransactionCode + "\r\n Desc: " + ahiapay.Note + " \r\n Date: " + transactionResult.DateOfTransaction.ToShortDateString() + " \r\n Bal: " + receivewallet.WithdrawBalance;

                        await _emailSender.SendToOne(REmail, RMessageSubject, RTitle, RMessage);
                        if (transferMoney.OneNairaOneView == false)
                        {
                            await _emailSender.SMSToOne(RPhone, RSMS);
                        }
                        TempData["success"] = "Transfer Succesful";
                    }
                    else
                    {
                        TempData["error"] = "Transfer Fail";
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
