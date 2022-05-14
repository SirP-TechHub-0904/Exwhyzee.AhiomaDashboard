using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.ManageTransaction.Pages.Transactions
{
    public class CompleteOrderModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserLogging _log;

        public CompleteOrderModel(
                UserManager<IdentityUser> userManager,
                IUserLogging log, AhiomaDbContext context,
                IEmailSendService emailSender)
        {
            _userManager = userManager;
            _context = context;
            _log = log;

            _emailSender = emailSender;

        }
        [BindProperty]
        public long OId { get; set; }
        public OrderItem OrderItem { get; set; }
        public IQueryable<Transaction> Transaction { get; set; }
        public string AccountSuccess { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            OId = id;
            OrderItem = await _context.OrderItems.Include(x => x.Product.Tenant).Include(x => x.Product).Include(x => x.Order).FirstOrDefaultAsync(x => x.Id == id);
            IQueryable<Transaction> itransaction = from s in _context.Transactions
             .OrderBy(x => x.DateOfTransaction)
             .Where(x => x.OrderItemId == id)
                                                   select s;
            Transaction = itransaction.AsQueryable();
            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            IQueryable<Transaction> itransaction = from s in _context.Transactions
              .OrderBy(x => x.DateOfTransaction)
              .Where(x => x.OrderItemId == OId)
                                                   select s;
            //
            try
            {
                var OrderItems = await _context.OrderItems.Include(x => x.Product.Tenant).Include(x => x.Product).Include(x => x.Order).FirstOrDefaultAsync(x => x.Id == OId);

                OrderItems.Status = Enums.OrderStatus.Completed;
                _context.Entry(OrderItems).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception f)
            {

            }


            var iList = await itransaction.ToListAsync();
            foreach (var itemT in iList)
            {
                var item = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == itemT.Id);
                if (item.PayoutStatus == Enums.PayoutStatus.Available)
                {
                    AccountSuccess = AccountSuccess + "Unable to move Money status is already in Available " + item.Description;
                }
                else
                {
                    var Profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == item.UserId);
                    //  var Product = await _context.Products.Include(x => x.Tenant).FirstOrDefaultAsync(x => x.id == item.UserId);

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
                    transfer.Amount = item.Amount;
                    transfer.DateOfTransfer = DateTime.UtcNow.AddHours(1);
                    if (Profile.Roles == null)
                    {
                        transfer.Description = "Commission Money moved to AhiaPay available balance";
                    }
                    else
                    {


                        if (Profile.Roles.ToLower().Contains("store"))
                        {
                            transfer.Description = "Sales Money moved to AhiaPay available balance";
                        }
                        else
                        {
                            transfer.Description = "Commission Money moved to AhiaPay available balance";
                        }
                    }
                    transfer.Sender = "Ahioma";
                    transfer.Status = Enums.TransferEnum.Success;
                    transfer.TransferReference = "Wallet";
                    transfer.UserId = Profile.UserId;
                    _context.AhiaPayTransfers.Add(transfer);


                    Wallet.Balance -= item.Amount;
                    Wallet.WithdrawBalance += item.Amount;
                    Wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                    _context.Entry(Wallet).State = EntityState.Modified;

                    WalletHistory history = new WalletHistory();
                    history.Amount = item.Amount;
                    history.CreationTime = DateTime.UtcNow.AddHours(1);
                    history.LedgerBalance = Wallet.Balance;
                    history.AvailableBalance = Wallet.WithdrawBalance;
                    history.WalletId = Wallet.Id;
                    history.UserId = Profile.UserId;
                    history.UserProfileId = Profile.Id;
                    history.TransactionType = "Cr";

                    history.Source = "commision";
                     _context.WalletHistories.Add(history);

                    item.PayoutStatus = Enums.PayoutStatus.Available;
                    _context.Entry(item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    AccountSuccess = AccountSuccess + "Transfer to wallet Successfully for " + item.Description;

                    string email = Profile.User.Email;
                    string phone = Profile.User.PhoneNumber;
                    string Title = "Hi " + Profile.Surname;
                    string SMS = "Transfer of N" + item.Amount + " was made to your Ahia Pay Wallet.";
                    string Subject = "Ahia Pay Wallet Transfer";
                    string Message = string.Format("<h4>Transfer to Your Wallet from Ahia Pay was Successful. Account credited Immediately with {0}", item.Amount);

                    await _emailSender.SendToOne(email, Subject, Title, Message);
                    await _emailSender.SMSToOne(phone, SMS);
                }
            }
            TempData["AccountSuccess"] = AccountSuccess;
            return RedirectToPage("./ItemOrderList", new { id = OId });

        }
    }
}