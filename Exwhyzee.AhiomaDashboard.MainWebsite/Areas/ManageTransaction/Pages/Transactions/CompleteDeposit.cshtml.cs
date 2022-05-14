using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;

using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.ManageTransaction.Pages.Transactions
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare")]
    public class CompleteDepositModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; 
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IOrderRepository _order;
        private readonly IMessageRepository _message;

        //private readonly IEmailSender _emailSender;
        private readonly IUserProfileRepository _account;

        public CompleteDepositModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IUserProfileRepository account
, AhiomaDbContext context
, IOrderRepository order
, IMessageRepository message
/*IEmailSender emailSender*/)
        {
            _userManager = userManager;
            //_emailSender = emailSender;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _order = order;
            _message = message;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
          
            public string Role { get; set; }

        }
        [BindProperty]
        public Transaction Transaction { get; set; }

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

        public async Task OnGetAsync(long id)
        {
            Transaction = await _context.Transactions.FindAsync(id);
        }
        [BindProperty]
        public string source { get; set; }
        [BindProperty]
        public string orderid { get; set; }
        [BindProperty]
        public string ahiapaystatus { get; set; }
        [BindProperty]
        public string transactiontype { get; set; }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
           
            if (ModelState.IsValid)
            {
                if (transactiontype == "deposit")
                {
                    //tracking code
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
                    string TransactionCode = xNumber.Substring(1, 8).ToUpper();

                    var transaction = await _context.Transactions.FindAsync(Transaction.Id);
                    var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
                    var profile = await _context.UserProfiles.Include(x=>x.User).FirstOrDefaultAsync(x => x.UserId == transaction.UserId);

                    transaction.TransactionReference = "update by admin";
                    transaction.WalletId = wallet.Id;
                    transaction.TrackCode = TransactionCode;
                    transaction.Note = "Online Deposit";

                    transaction.Status = Enums.EntityStatus.Success;
                    _context.Entry(transaction).State = EntityState.Modified;
                    //update wallet
                    wallet.WithdrawBalance += transaction.Amount;
                    wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                    _context.Entry(wallet).State = EntityState.Modified;
                    //update history
                    WalletHistory a = new WalletHistory();
                    a.Amount = transaction.Amount;
                    a.CreationTime = DateTime.UtcNow.AddHours(1);
                    a.LedgerBalance = wallet.Balance;
                    a.AvailableBalance = wallet.WithdrawBalance + transaction.Amount;
                    a.WalletId = wallet.Id;
                    a.UserId = wallet.UserId;
                    a.UserProfileId = profile.Id;
                    a.TransactionType = "Cr";
                    a.Source = "Online Deposit";
                    _context.WalletHistories.Add(a);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Online Deposit Successful";
                    try
                    {
                        if (transaction.Status == Enums.EntityStatus.Success)
                        {
                            string mSmsContent = "";
                            try
                            {
                                mSmsContent = await _message.GetMessage(Enums.ContentType.OnlineDepositSMS);
                            }
                            catch (Exception c) { }

                            //update content Data
                            mSmsContent = mSmsContent.Replace("|Amount|", transaction.Amount.ToString());
                            mSmsContent = mSmsContent.Replace("|Desc|", transaction.TransactionReference);
                            mSmsContent = mSmsContent.Replace("|Date|", transaction.DateOfTransaction.ToString("dd/MM/yyyy hh:mm tt"));
                            mSmsContent = mSmsContent.Replace("|Balance|", wallet.WithdrawBalance.ToString());
                            mSmsContent = mSmsContent.Replace("|||", "\r\n");
                            //sms
                            AddMessageDto sms = new AddMessageDto();
                            sms.Content = mSmsContent;
                            sms.Recipient = profile.User.PhoneNumber.Replace(" ", "");
                            sms.NotificationType = Enums.NotificationType.SMS;
                            sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                            sms.Retries = 0;
                            sms.Title = "Online Deposit";
                            //
                            var stss = await _message.AddMessage(sms);

                            ////email
                            string mEmailContent = "";
                            try
                            {
                                mEmailContent = await _message.GetMessage(Enums.ContentType.OnlineDepositEmail);
                            }
                            catch (Exception c) { }


                            ////update content Data
                            mEmailContent = mEmailContent.Replace("|Amount|", transaction.Amount.ToString());
                            mEmailContent = mEmailContent.Replace("|Ref|", transaction.TransactionReference);
                            mEmailContent = mEmailContent.Replace("|Date|", transaction.DateOfTransaction.ToString("dd/MM/yyyy hh:mm tt"));
                            mEmailContent = mEmailContent.Replace("|Balance|", wallet.WithdrawBalance.ToString());
                            mEmailContent = mEmailContent.Replace("|Surname|", profile.Surname);
                            mEmailContent = mEmailContent.Replace("|Description|", "Online Deposit");


                            AddMessageDto email = new AddMessageDto();
                            email.Content = mEmailContent;
                            email.Recipient = profile.User.Email.Replace(" ", "");
                            email.NotificationType = Enums.NotificationType.Email;
                            email.NotificationStatus = Enums.NotificationStatus.NotSent;
                            email.Retries = 0;
                            email.Title = "Online Deposit";

                            var sts = await _message.AddMessage(email);
                            
                        }
                        else
                        {
                            
                        }
                    }
                    catch (Exception c)
                    {
                        
                    }
                    return RedirectToPage("/Transactions/Index2", new { area="ManageTransaction"});
                }




                        var result = await _order.Insert(source, null, Transaction.Id.ToString(), null, "", orderid, ahiapaystatus, "ahiaid", transactiontype, "web", "");

                if (result.Contains("Ok Deposit"))
                {
                    TempData["success"] = "Online Deposit Successful";
                    return RedirectToPage("/Transactions/Index2", new { area = "ManageTransaction" });
                }
                else if (result.Contains("Fail Deposit"))
                {
                    TempData["error"] = "Online Deposit Failed. Try Again";
                    return RedirectToPage("./Index");
                }
                else if (result.Contains("Success: checkout successful"))
                {
                    TempData["success"] = "Order Placed";
                    return RedirectToPage("./MyOrders");
                }
                return Page();
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
