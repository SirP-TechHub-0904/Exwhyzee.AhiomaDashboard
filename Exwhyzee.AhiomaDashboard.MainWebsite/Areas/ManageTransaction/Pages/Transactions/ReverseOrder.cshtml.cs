using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.ManageTransaction.Pages.Transactions
{
    public class ReverseOrderModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserLogging _log;
        private readonly ITransactionRepository _transactionAppService;
        private readonly IHostingEnvironment _hostingEnv;
        public ReverseOrderModel(
                UserManager<IdentityUser> userManager,
                IUserLogging log, AhiomaDbContext context,
                ITransactionRepository transactionAppService, IHostingEnvironment hostingEnv,
                IEmailSendService emailSender)
        {
            _userManager = userManager;
            _context = context;
            _log = log;
            _hostingEnv = hostingEnv;
            _transactionAppService = transactionAppService;
            _emailSender = emailSender;

        }
        [BindProperty]
        public long OId { get; set; }

        [BindProperty]
        public string Note { get; set; }
        [BindProperty]
        public OrderItem OrderItem { get; set; }
        public IQueryable<Transaction> Transaction { get; set; }
        public string AccountSuccess { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {
            OId = id;
            OrderItem = await _context.OrderItems.Include(x => x.Product.Tenant).Include(x => x.Product.Tenant.User).Include(x => x.Product).Include(x => x.Order).Include(x => x.Order.UserProfile).Include(x => x.Order.UserProfile.User).FirstOrDefaultAsync(x => x.Id == id);
            IQueryable<Transaction> itransaction = from s in _context.Transactions
             .OrderBy(x => x.DateOfTransaction)
             .Where(x => x.OrderItemId == id)
                                                   select s;
            Transaction = itransaction.AsQueryable();

            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            var ahiapay = Url.Page(
                  "/Account/Index",
                  pageHandler: null,
                  values: new { area = "User" },
                  protocol: Request.Scheme);

            var transactionhistory = Url.Page(
                   "/Account/TransactionHistory",
                   pageHandler: null,
                   values: new { area = "User" },
                   protocol: Request.Scheme);


            IQueryable<Transaction> itransaction = from s in _context.Transactions
                                                      .Include(x => x.OrderItem.Product.Tenant)
              .Include(x => x.OrderItem.Product.Tenant.User).Include(x => x.OrderItem.Product).Include(x => x.OrderItem)
              .OrderBy(x => x.DateOfTransaction)
             
              
           
              .Where(x => x.OrderItemId == OId)
                                                   select s;
            var iList = await itransaction.ToListAsync();
            foreach (var itemT in iList)
            {
                var item = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == itemT.Id);

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
                    transfer.Description = "Commission reversed";
                }
                else
                {


                    if (Profile.Roles.ToLower().Contains("store"))
                    {
                        transfer.Description = "Sales reversed";
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
                history.TransactionType = "Dr";

                history.Source = "reverse";
                _context.WalletHistories.Add(history);

                item.PayoutStatus = Enums.PayoutStatus.Reversed;
                await _context.SaveChangesAsync();

                //create soa transaction
                var ReverseTransaction = await _transactionAppService.CreateTransaction(new Transaction
                {
                    Amount = item.Amount,
                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
                    Status = EntityStatus.Success,
                    TransactionType = TransactionTypeEnum.Credit,
                    Note = "Reversal of "+item.OrderItem.Product.Name,
                    UserId = Profile.UserId,
                    WalletId = Wallet.Id,
                    TrackCode = item.TrackCode,
                    Color = item.Color,
                    TransactionSection = TransactionSection.Reverse,
                    PayoutStatus = PayoutStatus.Ledger,
                    Description = "Reversal of " + item.OrderItem.Product.Name
                });

                AccountSuccess = AccountSuccess + "Reverse of N" + item.Amount + " from Ledger wallet was Successfully for " + item.Description;

                string email = Profile.User.Email;
                string phone = Profile.User.PhoneNumber;
                string Title = "Hi " + Profile.Surname;
                string SMS = "Reverse of N" + item.Amount + " was made from your Ahia Pay Ledger Wallet.";
                string Subject = "Ahia Pay Fund Reverse";
                string Message = string.Format("<h4>Reverse of N" + item.Amount + " from Ledger wallet was Successfully for");

                //user revesed
                MailMessage usermail = new MailMessage();
                try
                {
                    StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "Debit.html"));
                    //create the mail message 

                    string mailmsg = sr.ReadToEnd();
                    mailmsg = mailmsg.Replace("{Amount}", item.Amount.ToString());
                    mailmsg = mailmsg.Replace("{Description}", "Reversal of order (" + OrderItem.Product.Name + ")");
                    mailmsg = mailmsg.Replace("{Date}", DateTime.UtcNow.AddHours(1).ToString());
                    mailmsg = mailmsg.Replace("{LedgerBalance}", Wallet.Balance.ToString());
                    mailmsg = mailmsg.Replace("{AvailableBalance}", Wallet.WithdrawBalance.ToString());
                    mailmsg = mailmsg.Replace("{ref}", item.TrackCode.ToString());

                    mailmsg = mailmsg.Replace("{ahiapay}", ahiapay);
                    mailmsg = mailmsg.Replace("{history}", transactionhistory);

                    usermail.Body = mailmsg;
                    sr.Close();
                }
                catch (Exception c) { }

                await _emailSender.NewSendToOne(OrderItem.Order.UserProfile.User.Email, "Reversal of order (" + OrderItem.Product.Name + ")", usermail);



                MailMessage mail = new MailMessage();
                try
                {
                    StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "ReversedOrder.html"));
                    //create the mail message 
                    
                    string mailmsg = sr.ReadToEnd();

                    mailmsg = mailmsg.Replace("{ProductName}", item.OrderItem.Product.Name);
                    mailmsg = mailmsg.Replace("{Quantity}", item.OrderItem.Quantity.ToString());
                    mailmsg = mailmsg.Replace("{Amount}", item.OrderItem.Product.Price.ToString());
                    mailmsg = mailmsg.Replace("{Shop name}", item.OrderItem.Product.Tenant.BusinessName);
                    mailmsg = mailmsg.Replace("{Total}", (item.OrderItem.Quantity * item.OrderItem.Product.Price).ToString());


                    mail.Body = mailmsg;
                    sr.Close();
                }
                catch (Exception c) { }
                //
                //so mail
                var sovieworder = Url.Page(
                     "/Orders/Orders",
                     pageHandler: null,
                     values: new { area = "Store" },
                     protocol: Request.Scheme);
                var sodashboard = Url.Page(
                     "/Dashboard/Index",
                     pageHandler: null,
                     values: new { area = "Store" },
                     protocol: Request.Scheme);
                var shoppage = Url.Page(
                     "/",
                     pageHandler: null,
                     values: new { },
                     protocol: Request.Scheme);
                MailMessage somail = new MailMessage();
                try
                {
                    StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "ReversedOrderSO.html"));
                    //create the mail message 

                    string mailmsg = sr.ReadToEnd();
                    mailmsg = mailmsg.Replace("{ItemName}", item.OrderItem.Product.Name);
                    mailmsg = mailmsg.Replace("{Qty}", item.OrderItem.Quantity.ToString());
                    mailmsg = mailmsg.Replace("{Colour}", item.OrderItem.Itemcolor.ToString());
                    mailmsg = mailmsg.Replace("{Item SIze}", item.OrderItem.ItemSize.ToString());
                    mailmsg = mailmsg.Replace("{Amount}", item.OrderItem.Product.Price.ToString());

                    mailmsg = mailmsg.Replace("{vieworder}", sovieworder);
                    mailmsg = mailmsg.Replace("{sodashboard}", sodashboard);
                    mailmsg = mailmsg.Replace("{shoppage}", shoppage+"/"+item.OrderItem.Product.Tenant.TenentHandle);

                    somail.Body = mailmsg;
                    sr.Close();
                }
                catch (Exception c) { }

                await _emailSender.NewSendToOne(Profile.User.Email, "Reversal of " + item.OrderItem.Product.Name, mail);
                await _emailSender.NewSendToOne(item.OrderItem.Product.Tenant.User.Email, "Reversal of " + item.OrderItem.Product.Name, somail);
                await _emailSender.SendToOne(email, Subject, Title, Message);
                await _emailSender.SMSToOne(phone, SMS);

            }

            var sum = itransaction.Sum(x => x.Amount);
            var itemOrder = await _context.OrderItems.FirstOrDefaultAsync(x => x.Id == OId);
            var order = await _context.Orders.Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.Id == itemOrder.OrderId);
            var customerWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == order.UserProfile.UserId);
            customerWallet.WithdrawBalance += sum;
            customerWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
            _context.Entry(customerWallet).State = EntityState.Modified;

            //
            WalletHistory history1 = new WalletHistory();
            history1.Amount = sum;
            history1.CreationTime = DateTime.UtcNow.AddHours(1);
            history1.LedgerBalance = customerWallet.Balance;
            history1.AvailableBalance = customerWallet.WithdrawBalance;
            history1.WalletId = customerWallet.Id;
            history1.UserId = customerWallet.UserId;
            history1.UserProfileId = order.UserProfile.Id;
            history1.TransactionType = "Cr";

            history1.Source = "reserved";
            _context.WalletHistories.Add(history1);
            //
            var orderitemreverse = await _context.OrderItems.FirstOrDefaultAsync(x => x.Id == OId);
            orderitemreverse.Status = Enums.OrderStatus.Reversed;
            _context.Entry(orderitemreverse).State = EntityState.Modified;

            TrackOrder itrack = new TrackOrder();
            itrack.OrderItemId = orderitemreverse.Id;
            itrack.Status = "REVERSED ORDER";
            itrack.Note = Note;
            itrack.Date = DateTime.UtcNow.AddHours(1);
            _context.TrackOrders.Add(itrack);



            await _context.SaveChangesAsync();
            //create soa transaction
            var CustomerTransaction = await _transactionAppService.CreateTransaction(new Transaction
            {
                Amount = sum,
                DateOfTransaction = DateTime.UtcNow.AddHours(1),
                Status = EntityStatus.Success,
                TransactionType = TransactionTypeEnum.Credit,
                Note = "Reversal of order (" + OrderItem.Product.Name + ")",
                UserId = OrderItem.Order.UserProfile.UserId,
                WalletId = customerWallet.Id,
                TrackCode = OrderItem.ReferenceId,
                Color = "green",
                TransactionSection = TransactionSection.Reverse,
                PayoutStatus = PayoutStatus.Available,
                Description = "Reversal of order (" + OrderItem.Product.Name + ")"
            });

           
            MailMessage cusmail = new MailMessage();
            try
            {
                StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "Credit.html"));
                //create the mail message 

                string mailmsg = sr.ReadToEnd();
                mailmsg = mailmsg.Replace("{Amount}", sum.ToString());
                mailmsg = mailmsg.Replace("{Description}", "Reversal of order ("+OrderItem.Product.Name+")");
                mailmsg = mailmsg.Replace("{Date}", DateTime.UtcNow.AddHours(1).ToString());
                mailmsg = mailmsg.Replace("{LedgerBalance}", customerWallet.Balance.ToString());
                mailmsg = mailmsg.Replace("{AvailableBalance}", customerWallet.WithdrawBalance.ToString());
                mailmsg = mailmsg.Replace("{ref}", OrderItem.ReferenceId.ToString());

                mailmsg = mailmsg.Replace("{ahiapay}", ahiapay);
                mailmsg = mailmsg.Replace("{history}", transactionhistory);
               
                cusmail.Body = mailmsg;
                sr.Close();
            }
            catch (Exception c) { }

            await _emailSender.NewSendToOne(OrderItem.Order.UserProfile.User.Email, "Reversal of order (" + OrderItem.Product.Name + ")", cusmail);

            AccountSuccess = AccountSuccess + "credited customer " + order.UserProfile.Fullname + "with sum N" + sum;
            try
            {
                var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                var mainurllink = urllink.AbsoluteUri;
                var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == order.UserProfile.UserId);
                var lognew = await _log.LogData(order.UserProfile.User.UserName, "", mainurllink);
                Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                _context.Attach(Userlog).State = EntityState.Modified;

            }
            catch (Exception s)
            {

            }

            return RedirectToPage("./ItemOrderList", new { id = OId });

        }
    }
}