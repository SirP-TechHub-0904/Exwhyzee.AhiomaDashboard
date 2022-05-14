using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Reconsile.Pages.Transfers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin,CustomerCare,Logistic")]

    public class AdminAhiaPayTransferModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWalletRepository _walletAppService;
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;

        private readonly IHostingEnvironment _hostingEnv;

        public AdminAhiaPayTransferModel(IHostingEnvironment env,
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

            public decimal Amount { get; set; }

            public DateTime DateOfTransaction { get; set; }

            public TransactionTypeEnum Status { get; set; }

            public string Note { get; set; }
            public string PhoneNumber { get; set; }
            public string AhiaPayType { get; set; }
        }

        public string LoggedInUser { get; set; }

        public string ReturnUrl { get; set; }
        public decimal Balance { get; set; }


        public async Task OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            LoggedInUser = _userManager.GetUserId(HttpContext.User);
            var userbalance = await _walletAppService.GetWallet(LoggedInUser);
            Balance = userbalance.WithdrawBalance;

            //var users = _userManager.Users.Where(x => x.UserName != "mJinmcever").ToList();
            //Users = users;


        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    var Ruser = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == transferMoney.PhoneNumber);

                    var receiverVerify = await _context.UserProfiles.Include(x=>x.User).FirstOrDefaultAsync(x=>x.UserId == Ruser.Id);
                    var receiverwallet = await _walletAppService.GetWallet(receiverVerify.UserId);

                    if (receiverVerify.User.PhoneNumber != transferMoney.PhoneNumber)
                    {

                        Balance = receiverwallet.WithdrawBalance;
                        LoggedInUser = _userManager.GetUserId(HttpContext.User);
                        //var users = _userManager.Users.Where(x => x.UserName != "mJinmcever").ToList();
                        //Users = users;
                        StatusMessage = "Error! Unable to verify User. Confirm phone number";
                        return Page();
                    }
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

                    var ahiapay = new AhiapayAdminDto
                    {
                        ReceiverId = receiverVerify.UserId,
                        Amount = transferMoney.Amount,
                        Note = transferMoney.Note,
                        TransactionCode = TransactionCode,
                        ReceiverPhone = transferMoney.PhoneNumber,
                       
                        Receiverwalletid = receiverwallet.Id,
                        AhiaPayType = transferMoney.AhiaPayType
                    };


                    var result = await _walletAppService.CreateAhiaPayAdmin(ahiapay);

                    var receiverProfile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == receiverwallet.UserId);
                    var receivewallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == receiverwallet.UserId);
                    var transactionResult = await _context.Transactions.FirstOrDefaultAsync(x => x.TrackCode == ahiapay.TransactionCode);
                  

                    var REmail = "";
                    var RTitle = "";
                    var RMessageSubject = "";
                    var RPhone = "";
                    var RSMS = "";
                    var RMessage = "";
                    if (result == 1)
                    {


                        string stdr = "";
                        //
                        if(ahiapay.AhiaPayType == "Cr")
                        {
                            stdr = "Cr";
                        }else if (ahiapay.AhiaPayType == "Dr")
                        {
                            stdr = "Dr";
                        }

                        REmail = receiverProfile.User.Email;
                         RTitle = "Hi, " + receiverProfile.Fullname;
                         RMessageSubject = "AhiaPay Credit Alert";
                         RPhone = receiverProfile.User.PhoneNumber;
                        RMessage = "<table border=\"1\" width=\"40%\"><tr><td colspan=\"2\"><strong>AhiaPay "+stdr+"</strong></td></tr><tr><td>From</td><td>" + "Ahioma" + "</td></tr><tr><td>To</td><td>" + ahiapay.ReceiverPhone + "</td></tr><tr><td>Amount</td><td>" + ahiapay.Amount + "</td></tr><tr><td>Reference</td><td>" + ahiapay.TransactionCode + "</td></tr><tr><td>Description</td><td>" + ahiapay.Note + "</td></tr><tr><td>Date</td><td>" + transactionResult.DateOfTransaction + "</td></tr><tr><td>Balance</td><td>" + receiverwallet.WithdrawBalance + "</td></tr></table>";

                        RSMS = "AhiaPay "+stdr+".\r\n From" + " Ahioma Reconsiliation" + "\r\n To: " + ahiapay.ReceiverPhone + "\r\n Amt: " + ahiapay.Amount + "\r\n Ref: " + ahiapay.TransactionCode + "\r\n Desc: " + ahiapay.Note + " \r\n Date: " + transactionResult.DateOfTransaction.ToShortDateString() + " \r\n Bal: " + receivewallet.WithdrawBalance;


                        await _emailSender.SendToOne(REmail, RMessageSubject, RTitle, RMessage);
                        await _emailSender.SMSToOne(RPhone, RSMS);
                        TempData["success"] = "Transfer Succesful";
                    }
                    else
                    {
                        TempData["error"] = "Transfer Fail";
                    }
                    return RedirectToPage();
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
            }catch(Exception k)
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
