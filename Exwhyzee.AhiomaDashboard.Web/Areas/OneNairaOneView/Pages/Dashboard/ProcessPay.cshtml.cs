using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Exwhyzee.AhiomaDashboard.EntityFramework.Dtos;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using Exwhyzee.Enums;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Microsoft.AspNetCore.Hosting;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.OneNairaOneView.Pages.Dashboard
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare")]
    public class ProcessPayModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IWalletRepository _walletAppService;

        private readonly IHostingEnvironment _hostingEnv;


        public ProcessPayModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,

            IUserProfileRepository account, AhiomaDbContext context,
            IEmailSendService emailSender, IWalletRepository walletAppService, IHostingEnvironment hostingEnv)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _walletAppService = walletAppService;
            _hostingEnv = hostingEnv;
        }

        public PaginatedList<Transaction> Transaction { get; set; }
        public int CountString(string searchString)
        {
            int result = 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();

                if (searchString == "")
                    return 0;

                while (searchString.Contains("  "))
                    searchString = searchString.Replace("  ", " ");

                foreach (string y in searchString.Split(' '))

                    result++;

            }
            return result;
        }

        public int Count { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int? CurrentPage { get; set; }
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;
        public string LoggedInUser { get; set; }
        public decimal Balance { get; set; }


        [BindProperty]
        public UserProfile UserProfile { get; set; }
        public async Task<IActionResult> OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex, long id)
        {
            CurrentFilter = searchString;
            CurrentSort = sortOrder;
           
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            UserProfile = await _context.UserProfiles.Include(x=>x.User).FirstOrDefaultAsync(x=>x.Id == id);
            LoggedInUser = UserProfile.UserId;
            var iuser = _userManager.GetUserId(HttpContext.User);
            var userbalance = await _walletAppService.GetWallet(iuser);
            Balance = userbalance.WithdrawBalance;
            IQueryable<Transaction> itransact = from s in _context.Transactions
                                    .Where(x => x.UserId == UserProfile.UserId).Where(x => x.Description.Contains("1 View 1 Naira Comm."))
                                                   .OrderByDescending(x => x.DateOfTransaction)
                                                select s;

             
          
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    IQueryable<Transaction> listProduct = Enumerable.Empty<Transaction>().AsQueryable();
                    List<Transaction> alist = new List<Transaction>();
                    foreach (var item in searchStringCollection)
                    {
                        itransact = itransact.Where(s => (s.Amount != null) && s.Amount.ToString().ToUpper().Contains(item.ToUpper())
                        || (s.TrackCode != null) && s.TrackCode.ToUpper().Contains(item.ToUpper())
                        || (s.DateOfTransaction != null) && s.DateOfTransaction.ToString().ToUpper().Contains(item.ToUpper())
                        

                        );

                        var li = itransact.ToList();
                        alist.AddRange(li);
                    }

                }
                else
                {
                    itransact = itransact.Where(s => (s.Amount != null) && s.Amount.ToString().ToUpper().Contains(searchString.ToUpper())
                        || (s.TrackCode != null) && s.TrackCode.ToUpper().Contains(searchString.ToUpper())
                        || (s.DateOfTransaction != null) && s.DateOfTransaction.ToString().ToUpper().Contains(searchString.ToUpper())

                      );



                }

            }
            Count = itransact.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;

            Transaction = await PaginatedList<Transaction>.CreateAsync(
                itransact.AsNoTracking(), pageIndex ?? 1, pageSize);
            CurrentPage = pageIndex ?? 1;

            return Page();
        }
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
          
            public string Note { get; set; }
            public bool OneNairaOneView { get; set; }
        
            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var check = "";
                    
                    string uid = _userManager.GetUserId(HttpContext.User);
                    var profile = await _userManager.FindByIdAsync(uid);
                    var mprofile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == profile.Id);
                    var senderwallet = await _walletAppService.GetWallet(mprofile.UserId);
                    if (mprofile.DisableAdsCrediting == true)
                    {


                        StatusMessage = "Error! this Account has been Disable.";
                        return RedirectToPage("./AdUsers");
                    }
                    if (mprofile.DisableAhiaPayTransfer == true)
                    {
                        

                        StatusMessage = "Error! AhiaPay Transfer Disabled";
                        return RedirectToPage("./AdUsers");
                    }

                    if (transferMoney.PhoneNumber == profile.PhoneNumber)
                    {
                       
                        StatusMessage = "Error! You can't transfer to yourself.";
                        return RedirectToPage("./AdUsers");
                    }


                    if (senderwallet == null)
                    {
                        //   var userbalance = await _walletAppService.GetWallet(transferMoney.SenderId);
                      
                        StatusMessage = "Error! Your Wallet not found.";
                        return RedirectToPage("./AdUsers");
                    }
                    else
                    {
                        if (senderwallet.WithdrawBalance < transferMoney.Amount)
                        {
                            // var userbalance = await _walletAppService.GetWallet(transferMoney.SenderId);
                          
                            StatusMessage = "Error! Insufficient balance. Credit your account or send amount below your balance.";
                            return RedirectToPage("./AdUsers");
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
                    string iNote = "";
                   
                    var ahiapay = new AhiapayDto
                    {
                        Sender = senderwallet.UserId,
                        ReceiverId = receiverVerify.UserId,
                        Amount = transferMoney.Amount,
                        Note = "1 View 1 Naira Comm.",
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
                       


                        REmail = receiverProfile.User.Email;
                        RTitle = "Hi, " + receiverProfile.Fullname;
                        RMessageSubject = "AhiaPay Credit Alert";
                        RPhone = receiverProfile.User.PhoneNumber;
                        RMessage = "<table border=\"1\" width=\"40%\"><tr><td colspan=\"2\"><strong>AhiaPay Cr</strong></td></tr><tr><td>From</td><td>" + senderProfile.User.PhoneNumber + "</td></tr><tr><td>To</td><td>" + ahiapay.ReceiverPhone + "</td></tr><tr><td>Amount</td><td>" + ahiapay.Amount + "</td></tr><tr><td>Reference</td><td>" + ahiapay.TransactionCode + "</td></tr><tr><td>Description</td><td>" + ahiapay.Note + "</td></tr><tr><td>Date</td><td>" + transactionResult.DateOfTransaction + "</td></tr><tr><td>Balance</td><td>" + receiverwallet.WithdrawBalance + "</td></tr></table>";

                        RSMS = "AhiaPay Cr.\r\n From" + senderProfile.User.PhoneNumber + "\r\n To: " + ahiapay.ReceiverPhone + "\r\n Amt: " + ahiapay.Amount + "\r\n Ref: " + ahiapay.TransactionCode + "\r\n Desc: " + ahiapay.Note + " \r\n Date: " + transactionResult.DateOfTransaction.ToShortDateString() + " \r\n Bal: " + receivewallet.WithdrawBalance;

                        await _emailSender.SendToOne(REmail, RMessageSubject, RTitle, RMessage);

                        TempData["success"] = "Transfer Succesful";
                    }
                    else
                    {
                        TempData["error"] = "Transfer Fail";
                    }



                    return RedirectToPage("ProcessPay", new { id = receiverVerify.Id });
                }

                return Page();
            }
            catch (Exception ex)
            {
                throw ex;
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
