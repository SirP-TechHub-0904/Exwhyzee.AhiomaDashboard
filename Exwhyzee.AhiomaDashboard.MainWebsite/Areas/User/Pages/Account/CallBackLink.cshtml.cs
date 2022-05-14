using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.PayStack;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    public class CallBackLinkModel : PageModel
    {

        private readonly IWalletRepository _walletAppService;
        private readonly IUserProfileRepository _profile;
        private readonly AhiomaDbContext _context;
        private readonly ITransactionRepository _transactionAppService;
        private readonly IFlutterTransactionService _flutterTransactionAppService;
        private readonly IUserLogging _log;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IConfiguration _config;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSendService _emailSender;

        public CallBackLinkModel(ITransactionRepository transactionAppService,
          IFlutterTransactionService flutterTransactionAppService,
          IEmailSendService emailSender,
          AhiomaDbContext context, IHostingEnvironment hostingEnv,
          IUserLogging log,
          IUserProfileRepository profile, SignInManager<IdentityUser> signInManager,
          UserManager<IdentityUser> userManager, IWalletRepository walletAppService,
          IConfiguration configuration)
        {
            _transactionAppService = transactionAppService;
            _flutterTransactionAppService = flutterTransactionAppService;
            _userManager = userManager;
            _profile = profile;
            _walletAppService = walletAppService;
            _signInManager = signInManager;
            _config = configuration;
            _context = context;
            _log = log;
            _emailSender = emailSender;
            _hostingEnv = hostingEnv;
        }



        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public string StatusMessageSend { get; set; }

        public decimal Amount { get; set; }

        public int Check { get; set; }

        public decimal WalletTotal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync()
        {
            // tx_ref = ref &transaction_id = 30490 & status = successful

            // = checkout
            var source = HttpContext.Request.Query["source"].ToString();
            var tranxRef = HttpContext.Request.Query["tx_ref"].ToString();
            var transaction_id = HttpContext.Request.Query["transaction_id"].ToString();
            var status = HttpContext.Request.Query["status"].ToString();
            var customerRef = HttpContext.Request.Query["customerRef"].ToString();
            var orderid = HttpContext.Request.Query["oid"].ToString();
            var ahiapaystatus = HttpContext.Request.Query["ahiapaystatus"].ToString();
            var Ahia_transac_Id = HttpContext.Request.Query["transac_id"].ToString();

            


            //declear notification variable
            var shopEmail = "";
            var shopTitle = "";
            var shopSMS = "";
            var shopPhone = "";
            decimal shopMessageSubject = 0;
            var shopMessage = "";
            //
            var soaEmail = "";
            var soaTitle = "";
            decimal soaMessageSubject = 0;
            var soaSMS = "";
            var soaPhone = "";
            var soaMessage = "";
            //

            var RefsoaEmail = "";
            var RefsoaTitle = "";
            decimal RefsoaMessageSubject = 0;
            var RefsoaPhone = "";
            var RefsoaSMS = "";
            var RefsoaMessage = "";

            //

            var RefCustomerEmail = "";
            var RefCustomerTitle = "";
            decimal RefCustomerMessageSubject = 0;
            var RefCustomerPhone = "";
            var RefCustomerSMS = "";
            var RefCustomerMessage = "";
            //
            var AdminEmail = "";
            var AdminTitle = "";
            var AdminSubject = "";
            var AdminMessage = "";

            //
            var ARefEmail = "";
            var ARefTitle = "";
            var ARefSubject = "";
            var ARefMessage = "";

            //
            var LogEmail = "";
            var LogTitle = "";
            var LogSubject = "";
            var LogMessage = "";
            //

            var OrderEmail = "";
            var OrderTitle = "";
            var OrderSMS = "";
            var OrderPhone = "";
            var OrderSubject = "";
            var OrderMessage = "";
            var AdminOrderSMS = "";
            var AdminOrderEmail = "";
            //
            string itemlist = "";
            //
            string TransactionCode = "";
            Transaction transaction = new Transaction();
            UserProfile user1 = new UserProfile();
            Wallet wallet = new Wallet();
            long orderId = Convert.ToInt64(orderid);
            var orderitems = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == orderId);

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
            TransactionCode = xNumber.Substring(1, 8).ToUpper();

            if (!String.IsNullOrEmpty(tranxRef))
                {
                    var response = await _flutterTransactionAppService.VerifyTransaction(transaction_id);
                    //var response = await _flutterTransactionAppService.VerifyTransaction(tranxRef);
                    long t_id = Convert.ToInt64(tranxRef);


                     transaction = await _transactionAppService.GetById(t_id);
                    transaction.TransactionReference = transaction_id;
                    var updatemain = await _transactionAppService.Update(transaction);

                     user1 = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
                    //var user = await _userManager.GetUserAsync(User);
                    try
                    {
                        var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
                        var lognew = await _log.LogData(transaction.UserId, "", "");
                        Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                        _context.Attach(Userlog).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception s)
                    {

                    }
                     wallet = await _walletAppService.GetWallet(transaction.UserId);
                    if (response == null)
                    {
                        transaction.WalletId = wallet.Id;
                        transaction.Status = Enums.EntityStatus.Failed;
                    transaction.TrackCode = TransactionCode;
                        transaction.TransactionReference = tranxRef;
                        transaction.Note = "Online Deposit";
                        var update = await _transactionAppService.Update(transaction);
                        StatusMessage = $"Error! Transaction with Reference {transaction_id} failed.";
                        StatusMessageSend = $"Transaction with Reference {transaction_id} failed. We help you sell more. Ahioma";
                        await _signInManager.SignInAsync(user1.User, isPersistent: false);
                    //failed order
                   // long orderId = Convert.ToInt64(orderid);
                    var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
                    var ProductCarts = await _context.OrderItems.Include(x => x.Product).Include(x => x.Product.Tenant).Where(x => x.OrderId == orderId).ToListAsync();

                    var ProductCartlist = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.Tenant).Where(x => x.UserProfileId == profile.Id && x.CartStatus == CartStatus.Active).ToListAsync();

                    foreach (var p in ProductCartlist)
                    {

                        p.CartStatus = Enums.CartStatus.CheckOut;
                        _context.Attach(p).State = EntityState.Modified;

                    }
                    try
                    {
                        var iorderitems = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == orderId);
                            iorderitems.Note = "Payment Not Successful";
                            iorderitems.Status = OrderStatus.Pending;
                            iorderitems.AmountPaid = 0;
                            iorderitems.OrderId = orderitems.Id.ToString("0000000");
                            _context.Attach(iorderitems).State = EntityState.Modified;
                        foreach (var p in ProductCarts)
                        {
                            

                            p.Note = "Payment Not Successful";
                            p.Status = OrderStatus.Pending;
                            _context.Attach(p).State = EntityState.Modified;

                            TrackOrder itrack = new TrackOrder();
                            itrack.OrderItemId = p.Id;
                            itrack.Status = "ORDER PLACED";
                            itrack.Date = DateTime.UtcNow.AddHours(1);
                            _context.TrackOrders.Add(itrack);

                            TrackOrder itrack2 = new TrackOrder();
                            itrack2.OrderItemId = p.Id;
                            itrack2.Status = "PENDING CONFIRMATION";
                            itrack2.Date = DateTime.UtcNow.AddHours(1);
                            _context.TrackOrders.Add(itrack2);

                            TrackOrder itrack3 = new TrackOrder();
                            itrack3.OrderItemId = p.Id;
                            itrack3.Status = "CANCELLED - PAYMENT UNSUCCESSFUL";
                            itrack3.Date = DateTime.UtcNow.AddHours(1);
                            _context.TrackOrders.Add(itrack3);
                        }
                        await _context.SaveChangesAsync();
                    }catch(Exception h) { }

                    await _emailSender.SendToOne(user1.User.Email, "AHIA PAY", "Hi, " + user1.Fullname, StatusMessageSend);


                        await _emailSender.SMSToOne(user1.User.PhoneNumber, StatusMessageSend);


                        return Page();
                    }
                    if (response.data.status == "successful" || status == "completed")
                    {

                    }
                }
            

                else if (Ahia_transac_Id != null && ahiapaystatus == "success")
            {
               
                long t_id = Convert.ToInt64(Ahia_transac_Id);


                transaction = await _transactionAppService.GetById(t_id);
               
                user1 = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
                //var user = await _userManager.GetUserAsync(User);
                try
                {
                    var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
                    var lognew = await _log.LogData(transaction.UserId, "", "");
                    Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                    _context.Attach(Userlog).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                }
                catch (Exception s)
                {

                }
                wallet = await _walletAppService.GetWallet(transaction.UserId);
                if (transaction.Status != EntityStatus.Success)
                {
                    transaction.WalletId = wallet.Id;
                    transaction.Status = Enums.EntityStatus.Failed;
                    transaction.TrackCode = TransactionCode;
                    transaction.TransactionReference = tranxRef;
                    transaction.Note = "Buy from Ahia Pay";
                    var update = await _transactionAppService.Update(transaction);
                    StatusMessage = $"Error! Transaction with Reference {transaction_id} failed.";
                    StatusMessageSend = $"Transaction with Reference {transaction_id} failed. We help you sell more. Ahioma";
                    await _signInManager.SignInAsync(user1.User, isPersistent: false);
                    await _emailSender.SendToOne(user1.User.Email, "AHIA PAY", "Hi, " + user1.Fullname, StatusMessageSend);
                    await _emailSender.SMSToOne(user1.User.PhoneNumber, StatusMessageSend);
                    return Page();
                }
                if (transaction.Status == EntityStatus.Success)
                {

                }
            }
            else
            {
                transaction.WalletId = wallet.Id;
                transaction.Status = Enums.EntityStatus.Failed;
                transaction.TransactionReference = tranxRef;
                var update = await _transactionAppService.Update(transaction);
                StatusMessage = $"Error! Transaction with Reference {transaction_id} failed.";
                return Page();

            }

            Amount = transaction.Amount;

            if (transaction == null)
            {
                TempData["StatusMessage"] = $"Transaction with Reference {transaction_id} was successful. But Wallet was not updated. Please contact Help Desk.";
                StatusMessage = $"Transaction with Reference {transaction_id} was successful. But Wallet was not updated. Please contact Help Desk.";
                await _signInManager.SignInAsync(user1.User, isPersistent: false);
                await _emailSender.SendToOne(user1.User.Email, "AHIA PAY", "Hi, " + user1.Fullname, StatusMessage);


                await _emailSender.SMSToOne(user1.User.PhoneNumber, StatusMessage);

                return Page();
            }
            else if (!string.IsNullOrEmpty(transaction.TransactionReference))
            {
                TempData["StatusMessage"] = $"Transaction with Reference {transaction_id} was successful.";
                StatusMessage = $"Transaction with Reference {transaction_id} was successful.";


                WalletTotal = wallet.WithdrawBalance;
                transaction.WalletId = wallet.Id;
                transaction.TrackCode = TransactionCode;
                transaction.Status = Enums.EntityStatus.Success;
                transaction.TransactionReference = transaction_id;
                transaction.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                var update = await _transactionAppService.Update(transaction);

                wallet.WithdrawBalance += transaction.Amount;
                wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                await _walletAppService.Update(wallet);

                //wallet histiory
                WalletHistory credithistory = new WalletHistory();
                credithistory.Amount = transaction.Amount;
                credithistory.CreationTime = DateTime.UtcNow.AddHours(1);
                credithistory.LedgerBalance = wallet.Balance;
                credithistory.AvailableBalance = wallet.WithdrawBalance;
                credithistory.WalletId = wallet.Id;
                credithistory.UserId = wallet.UserId;
                credithistory.UserProfileId = user1.Id;
                credithistory.TransactionType = "Cr";
                credithistory.Source = "Online Order for Order Id (" + orderid + ")";
                _context.WalletHistories.Add(credithistory);
                await _context.SaveChangesAsync();
                var walletcurrent = await _walletAppService.GetWallet(transaction.UserId);
                var user = await _userManager.FindByIdAsync(transaction.UserId);
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);

                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["StatusMessage"] = $"Transaction with Reference {transaction_id} was successful.";
                StatusMessage = $"Transaction with Reference {transaction_id} was successful.";
                await _emailSender.SendToOne(user1.User.Email, "AHIA PAY", "Hi, " + user1.Fullname, StatusMessage);


                await _emailSender.SMSToOne(user1.User.PhoneNumber, StatusMessage);
                if (source == "checkout")
                {
                    var newtransaction = await _transactionAppService.CreateTransaction(new Transaction
                    {
                        Amount = transaction.Amount,
                        DateOfTransaction = DateTime.UtcNow.AddHours(1),
                        Status = EntityStatus.Success,
                        TransactionType = TransactionTypeEnum.Debit,
                        Note = "Online Order",
                        UserId = user.Id,
                        TrackCode = TransactionCode,
                        WalletId = wallet.Id,
                        TransactionShowEnum = TransactionShowEnum.Off,
                        TransactionSection = TransactionSection.Order,
                        Description = "Online Order for Order Id (" + orderid + ")"
                    });

                    wallet.WithdrawBalance -= transaction.Amount;
                    wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                    await _walletAppService.Update(wallet);

                    //wallet histiory
                    WalletHistory debithistory = new WalletHistory();
                    debithistory.Amount = transaction.Amount;
                    debithistory.CreationTime = DateTime.UtcNow.AddHours(1);
                    debithistory.LedgerBalance = wallet.Balance;
                    debithistory.AvailableBalance = wallet.WithdrawBalance;
                    debithistory.WalletId = wallet.Id;
                    debithistory.UserId = wallet.UserId;
                    debithistory.UserProfileId = userProfile.Id;
                    debithistory.TransactionType = "Dr";
                    debithistory.Source = "Online Order for Order Id (" + orderid + ")";
                    _context.WalletHistories.Add(debithistory);
                    await _context.SaveChangesAsync();

                    //update store wallets

                    var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    var ProductCarts = await _context.OrderItems.Include(x => x.Product).Include(x => x.Product.Tenant).Where(x => x.OrderId == orderId).ToListAsync();
                    try
                    {
                        var tcodecolor = "";
                        foreach (var p in ProductCarts)
                        {
                            if (p.Product.Name.Length > 15)
                            {
                                p.Product.Name = p.Product.Name.Substring(0, 14);
                            }


                            //generate token for tracking


                            tcodecolor = TransactionCode;
                            //get product
                            var shop = await _context.Tenants.Include(x => x.User).Include(x => x.UserProfile).FirstOrDefaultAsync(x => x.Id == p.Product.TenantId);
                            //get store wallet
                            var storeWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == shop.UserId);
                            //get commision
                            decimal pDivision = Convert.ToDecimal(p.Product.Commision);
                            if (pDivision == 0)
                            {
                                pDivision = pDivision + 5;
                            }
                            decimal productAmountAfterCommisionAdd = pDivision / Convert.ToDecimal(100);
                            decimal productAmountAfterCommision = p.Product.Price * productAmountAfterCommisionAdd * p.Quantity;
                            //update store wallet

                            var storewallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == p.Product.Tenant.UserId);
                            var amountCommision = Convert.ToDecimal(p.Product.Price) * p.Quantity * (Convert.ToDecimal(pDivision) / Convert.ToDecimal(100));
                            var shopProductBalance = (Convert.ToDecimal(p.Product.Price) * p.Quantity) - amountCommision;
                            storewallet.Balance += shopProductBalance;
                            storewallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                            await _walletAppService.Update(storewallet);

                            //wallet histiory
                            WalletHistory storehistory = new WalletHistory();
                            storehistory.Amount = shopProductBalance;
                            storehistory.CreationTime = DateTime.UtcNow.AddHours(1);
                            storehistory.LedgerBalance = storewallet.Balance;
                            storehistory.AvailableBalance = storewallet.WithdrawBalance;
                            storehistory.WalletId = storewallet.Id;
                            storehistory.UserId = storewallet.UserId;
                            storehistory.UserProfileId = shop.UserProfileId ?? 0;
                            storehistory.TransactionType = "Cr";
                            storehistory.Source = "Commission " + p.Product.Name;
                            _context.WalletHistories.Add(storehistory);
                            await _context.SaveChangesAsync();

                            //create transaction for store
                            var storeNewewtransactionStore = await _transactionAppService.CreateTransaction(new Transaction
                            {
                                Amount = shopProductBalance,
                                DateOfTransaction = DateTime.UtcNow.AddHours(1),
                                Status = EntityStatus.Success,
                                TransactionType = TransactionTypeEnum.Credit,
                                Note = "Sales",
                                UserId = shop.UserId,
                                WalletId = storewallet.Id,
                                TrackCode = TransactionCode,
                                OrderItemId = p.Id,
                                TransactionSection = TransactionSection.Sales,
                                PayoutStatus = PayoutStatus.Ledger,
                                Description = "SO (" + p.Product.Name + ")"
                            });
                            //send sms and email. store
                            //
                            shopEmail = shop.User.Email;
                            shopPhone = shop.User.PhoneNumber;
                            shopTitle = "Hi " + shop.BusinessName;
                          //  shopSMS = "Deposit of NGN" + shopProductBalance.ToString("#.##") + " received for " + p.Product.Name;
                            shopMessageSubject += shopProductBalance;
                            shopMessage += "You have received a deposit of NGN" + shopProductBalance.ToString("#.##") + " for " + p.Product.Name +"<br>";
                            //

                            //get SOA
                            var soaUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == shop.CreationUserId);
                            //get soa wallet
                            var soaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == soaUser.UserId);
                            //soa commision of 25% of shop commision
                            decimal soaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(25) / Convert.ToDecimal(100));
                            soaWallet.Balance += soaCommision;
                            soaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                            await _walletAppService.Update(soaWallet);

                            //wallet histiory
                            WalletHistory soahistory = new WalletHistory();
                            soahistory.Amount = soaCommision;
                            soahistory.CreationTime = DateTime.UtcNow.AddHours(1);
                            soahistory.LedgerBalance = soaWallet.Balance;
                            soahistory.AvailableBalance = soaWallet.WithdrawBalance;
                            soahistory.WalletId = soaWallet.Id;
                            soahistory.UserId = soaWallet.UserId;
                            soahistory.UserProfileId = soaUser.Id;
                            soahistory.TransactionType = "Cr";
                            soahistory.Source = "Commission " + p.Product.Name;
                            _context.WalletHistories.Add(soahistory);
                            await _context.SaveChangesAsync();

                            //create soa transaction
                            var SoaNewtransactionStore = await _transactionAppService.CreateTransaction(new Transaction
                            {
                                Amount = soaCommision,
                                DateOfTransaction = DateTime.UtcNow.AddHours(1),
                                Status = EntityStatus.Success,
                                Note = "Commision",
                                TransactionType = TransactionTypeEnum.Credit,
                                UserId = soaUser.UserId,
                                WalletId = soaWallet.Id,
                                TrackCode = TransactionCode,
                                PayoutStatus = PayoutStatus.Ledger,
                                OrderItemId = p.Id,
                                TransactionSection = TransactionSection.ShopCommission,
                                Description = "SOA (" + p.Product.Name + ")"
                            });
                            //send sms and email.
                            //
                            //
                            soaEmail = soaUser.User.Email;
                            soaPhone = soaUser.User.PhoneNumber;
                            soaTitle = "Hi " + soaUser.Surname;
                            soaMessageSubject += soaCommision; 
                           // soaSMS = "Deposit of NGN" + soaCommision.ToString("#.##") + " received for " + p.Product.Name + "for SO (" + shop.BusinessName + ").";
                            soaMessage += "You have received a deposit of NGN" + soaCommision.ToString("#.##") + " for " + p.Product.Name +"(Qty: "+p.Quantity+")"+"<br>";
                            //
                            //

                            //get Referral
                            if (!String.IsNullOrEmpty(soaUser.ReferralLink))
                            {


                                var ReferralUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.IdNumber == soaUser.ReferralLink);
                                //get soa wallet
                                var ReferralSoaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == ReferralUser.UserId);
                                //soa commision of 5% of referral commision
                                decimal ReferralSoaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                                ReferralSoaWallet.Balance += ReferralSoaCommision;
                                ReferralSoaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                                await _walletAppService.Update(ReferralSoaWallet);

                                //wallet histiory
                                WalletHistory refsoahistory = new WalletHistory();
                                refsoahistory.Amount = ReferralSoaCommision;
                                refsoahistory.CreationTime = DateTime.UtcNow.AddHours(1);
                                refsoahistory.LedgerBalance = ReferralSoaWallet.Balance;
                                refsoahistory.AvailableBalance = ReferralSoaWallet.WithdrawBalance;
                                refsoahistory.WalletId = ReferralSoaWallet.Id;
                                refsoahistory.UserId = ReferralSoaWallet.UserId;
                                refsoahistory.UserProfileId = ReferralUser.Id;
                                refsoahistory.TransactionType = "Cr";
                                refsoahistory.Source = "Commission " + p.Product.Name;
                                _context.WalletHistories.Add(refsoahistory);
                                await _context.SaveChangesAsync();

                                //create soa transaction
                                var ReferralSoaNewtransactionStore = await _transactionAppService.CreateTransaction(new Transaction
                                {
                                    Amount = ReferralSoaCommision,
                                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
                                    Status = EntityStatus.Success,
                                    TransactionType = TransactionTypeEnum.Credit,
                                    Note = "Referral Commision",
                                    UserId = ReferralUser.UserId,
                                    WalletId = ReferralSoaWallet.Id,
                                    TrackCode = TransactionCode,
                                    PayoutStatus = PayoutStatus.Ledger,
                                    OrderItemId = p.Id,
                                    TransactionSection = TransactionSection.ReferralCommission,
                                    Description = "SOA Upline (" + p.Product.Name + ")"
                                });
                                //send sms and email.
                                //
                                //
                                RefsoaEmail = ReferralUser.User.Email;
                                RefsoaPhone = ReferralUser.User.PhoneNumber;
                                RefsoaTitle = "Hi " + ReferralUser.Surname;
                                RefsoaMessageSubject += ReferralSoaCommision;
                               // RefsoaSMS += "Deposit of NGN" + ReferralSoaCommision.ToString("#.##") + " received for " + p.Product.Name +"<br>";
                                RefsoaMessage += "You have received a deposit of NGN" + ReferralSoaCommision.ToString("#.##") + " for " + p.Product.Name + "(Qty: "+p.Quantity+")"+".<br>";
                                //
                                //

                            }
                            else
                            {
                                var AdminReferralUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == "referral@ahioma.com");
                                //get soa wallet
                                var AdminReferralSoaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == AdminReferralUser.UserId);
                                //soa commision of 5% of referral commision
                                decimal AdminReferralSoaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                                AdminReferralSoaWallet.Balance += AdminReferralSoaCommision;
                                AdminReferralSoaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                                await _walletAppService.Update(AdminReferralSoaWallet);

                                //wallet histiory
                                WalletHistory adrefsoahistory = new WalletHistory();
                                adrefsoahistory.Amount = AdminReferralSoaCommision;
                                adrefsoahistory.CreationTime = DateTime.UtcNow.AddHours(1);
                                adrefsoahistory.LedgerBalance = AdminReferralSoaWallet.Balance;
                                adrefsoahistory.AvailableBalance = AdminReferralSoaWallet.WithdrawBalance;
                                adrefsoahistory.WalletId = AdminReferralSoaWallet.Id;
                                adrefsoahistory.UserId = AdminReferralSoaWallet.UserId;
                                adrefsoahistory.UserProfileId = AdminReferralUser.Id;
                                adrefsoahistory.TransactionType = "Cr";
                                adrefsoahistory.Source = "Commission " + p.Product.Name;
                                _context.WalletHistories.Add(adrefsoahistory);
                                await _context.SaveChangesAsync();
                                //create soa transaction
                                var AdminReferralSoaNewtransactionStore = await _transactionAppService.CreateTransaction(new Transaction
                                {
                                    Amount = AdminReferralSoaCommision,
                                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
                                    Status = EntityStatus.Success,
                                    TransactionType = TransactionTypeEnum.Credit,
                                    Note = "Referral Commision",
                                    UserId = AdminReferralUser.UserId,
                                    WalletId = AdminReferralSoaWallet.Id,
                                    TrackCode = TransactionCode,
                                    PayoutStatus = PayoutStatus.Ledger,
                                    OrderItemId = p.Id,
                                    TransactionSection = TransactionSection.ReferralCommission,
                                    Description = "SOA Upline (" + p.Product.Name + ")"
                                });
                                //send sms and email.
                                //
                                //
                                ARefEmail = AdminReferralUser.User.Email;
                                
                                ARefTitle = "Hi " + AdminReferralUser.Surname;
                                ARefSubject += AdminReferralSoaCommision;
                                // RefsoaSMS += "Deposit of NGN" + ReferralSoaCommision.ToString("#.##") + " received for " + p.Product.Name +"<br>";
                                ARefMessage += "You have received a deposit of NGN" + AdminReferralSoaCommision.ToString("#.##") + " for " + p.Product.Name + "(Qty: " + p.Quantity + ")" + ".<br>";
                                //
                                //
                            }
                            //get customer Referral
                            if (!String.IsNullOrEmpty(customerRef))
                            {


                                var ReferralCustomer = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.IdNumber == customerRef);
                                //get soa wallet
                                var ReferralCustomerWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == ReferralCustomer.UserId);
                                //soa commision of 5% of referral commision
                                decimal ReferralCustomerCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                                ReferralCustomerWallet.Balance += ReferralCustomerCommision;
                                ReferralCustomerWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                                await _walletAppService.Update(ReferralCustomerWallet);
                                //wallet histiory
                                WalletHistory cushistory = new WalletHistory();
                                cushistory.Amount = ReferralCustomerCommision;
                                cushistory.CreationTime = DateTime.UtcNow.AddHours(1);
                                cushistory.LedgerBalance = ReferralCustomerWallet.Balance;
                                cushistory.AvailableBalance = ReferralCustomerWallet.WithdrawBalance;
                                cushistory.WalletId = ReferralCustomerWallet.Id;
                                cushistory.UserId = ReferralCustomerWallet.UserId;
                                cushistory.UserProfileId = ReferralCustomer.Id;
                                cushistory.TransactionType = "Cr";
                                cushistory.Source = "Commission " + p.Product.Name;
                                _context.WalletHistories.Add(cushistory);
                                await _context.SaveChangesAsync();

                                //create soa transaction
                                var ReferralCustomerNewtransactionStore = await _transactionAppService.CreateTransaction(new Transaction
                                {
                                    Amount = ReferralCustomerCommision,
                                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
                                    Status = EntityStatus.Success,
                                    TransactionType = TransactionTypeEnum.Credit,
                                    Note = "Referral Commision",
                                    UserId = ReferralCustomer.UserId,
                                    WalletId = ReferralCustomerWallet.Id,
                                    TrackCode = TransactionCode,
                                    PayoutStatus = PayoutStatus.Ledger,
                                    OrderItemId = p.Id,
                                    TransactionSection = TransactionSection.CustomerReferralCommission,
                                    Description = "Customer Referral (" + p.Product.Name + ")"
                                });
                                //send sms and email.
                                //
                                //
                                RefCustomerEmail = ReferralCustomer.User.Email;
                                RefCustomerPhone = ReferralCustomer.User.PhoneNumber;
                                RefCustomerTitle = "Hi " + ReferralCustomer.Surname;
                                RefCustomerMessageSubject += ReferralCustomerCommision;
                                //RefCustomerSMS = "Deposit of NGN" + ReferralCustomerCommision.ToString("#.##") + " received for " + p.Product.Name;
                                RefCustomerMessage += "You have received a deposit of NGN" + ReferralCustomerCommision.ToString("#.##") + " for " + p.Product.Name+" (Qty: " + p.Quantity + ")" + ".<br>";
                                //
                                //

                            }
                            else
                            {
                                var AdminReferralUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == "referral@ahioma.com");
                                //get soa wallet
                                var AdminReferralSoaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == AdminReferralUser.UserId);
                                //soa commision of 5% of referral commision
                                decimal AdminReferralSoaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                                AdminReferralSoaWallet.Balance += AdminReferralSoaCommision;
                                AdminReferralSoaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                                await _walletAppService.Update(AdminReferralSoaWallet);


                                //wallet histiory
                                WalletHistory adcushistory = new WalletHistory();
                                adcushistory.Amount = AdminReferralSoaCommision;
                                adcushistory.CreationTime = DateTime.UtcNow.AddHours(1);
                                adcushistory.LedgerBalance = AdminReferralSoaWallet.Balance;
                                adcushistory.AvailableBalance = AdminReferralSoaWallet.WithdrawBalance;
                                adcushistory.WalletId = AdminReferralSoaWallet.Id;
                                adcushistory.UserId = AdminReferralSoaWallet.UserId;
                                adcushistory.UserProfileId = AdminReferralUser.Id;
                                adcushistory.TransactionType = "Cr";
                                adcushistory.Source = "Commission " + p.Product.Name;
                                _context.WalletHistories.Add(adcushistory);
                                await _context.SaveChangesAsync();

                                //create soa transaction
                                var AdminReferralSoaNewtransactionStore = await _transactionAppService.CreateTransaction(new Transaction
                                {
                                    Amount = AdminReferralSoaCommision,
                                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
                                    Status = EntityStatus.Success,
                                    TransactionType = TransactionTypeEnum.Credit,
                                    Note = "Customer Referral Commision",
                                    UserId = AdminReferralUser.UserId,
                                    WalletId = AdminReferralSoaWallet.Id,
                                    TrackCode = TransactionCode,
                                    PayoutStatus = PayoutStatus.Ledger,
                                    OrderItemId = p.Id,
                                    TransactionSection = TransactionSection.ReferralCommission,
                                    Description = "Customer Referral ("+p.Product.Name+")"
                                });
                                //send sms and email.
                                //
                                //
                                ARefEmail = AdminReferralUser.User.Email;

                                ARefTitle = "Hi " + AdminReferralUser.Surname;
                                ARefSubject += AdminReferralSoaCommision;
                                // RefsoaSMS += "Deposit of NGN" + ReferralSoaCommision.ToString("#.##") + " received for " + p.Product.Name +"<br>";
                                ARefMessage += "You have received a deposit of NGN" + AdminReferralSoaCommision.ToString("#.##") + " for " + p.Product.Name + "(Qty: " + p.Quantity + ")" + ".<br>";
                                //
                                //
                            }
                            //admin commission
                            var getadmin = await _userManager.FindByEmailAsync("jinmcever@gmail.com");
                            var admin = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == getadmin.Id);
                            //get soa wallet
                            var AdminWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == getadmin.Id);
                            //soa commision of 70% of shop commision
                            decimal AdminCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(65) / Convert.ToDecimal(100));
                            AdminWallet.Balance += AdminCommision;
                            AdminWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                            await _walletAppService.Update(AdminWallet);

                            //wallet histiory
                            WalletHistory adminhistory = new WalletHistory();
                            adminhistory.Amount = AdminCommision;
                            adminhistory.CreationTime = DateTime.UtcNow.AddHours(1);
                            adminhistory.LedgerBalance = AdminWallet.Balance;
                            adminhistory.AvailableBalance = AdminWallet.WithdrawBalance;
                            adminhistory.WalletId = AdminWallet.Id;
                            adminhistory.UserId = AdminWallet.UserId;
                            adminhistory.UserProfileId = admin.Id;
                            adminhistory.TransactionType = "Cr";
                            adminhistory.Source = "Commission " + p.Product.Name;
                            _context.WalletHistories.Add(adminhistory);
                            await _context.SaveChangesAsync();


                            //create soa transaction
                            var AdmintransactionStore = await _transactionAppService.CreateTransaction(new Transaction
                            {
                                Amount = AdminCommision,
                                DateOfTransaction = DateTime.UtcNow.AddHours(1),
                                Status = EntityStatus.Success,
                                TransactionType = TransactionTypeEnum.Credit,
                                Note = "Ahioma Commision",
                                UserId = getadmin.Id,
                                WalletId = AdminWallet.Id,
                                TrackCode = TransactionCode,
                                OrderItemId = p.Id,
                                TransactionSection = TransactionSection.Commission,
                                PayoutStatus = PayoutStatus.Ledger,
                                Description = "AHIOMA ("+p.Product.Name+")"
                            });
                            //send sms and email.
                            //
                            AdminEmail = getadmin.Email;
                            AdminTitle = "Hi " + admin.Surname;
                            AdminSubject += AdminCommision;
                            AdminMessage += "You have received a deposit of NGN" + AdminCommision.ToString("#.##") + " for " + p.Product.Name + "(Qty: " + p.Quantity + ")" + ". <br>";
                            //
                            //
                           
                        }
                        //logistic commission
                        var Loggetadmin = await _userManager.FindByEmailAsync("ahiaexpress@ahioma.com");
                        var Logadmin = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == Loggetadmin.Id);
                        //get soa wallet
                        var LogAdminWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == Loggetadmin.Id);
                        //soa commision of 70% of shop commision
                        decimal? LogAdminCommision = orderitems.LogisticAmount;
                        LogAdminWallet.Balance += LogAdminCommision ?? 0;
                        LogAdminWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                        await _walletAppService.Update(LogAdminWallet);

                        //create soa transaction
                        var LogAdmintransactionStore = await _transactionAppService.CreateTransaction(new Transaction
                        {
                            Amount = LogAdminCommision ?? 0,
                            DateOfTransaction = DateTime.UtcNow.AddHours(1),
                            Status = EntityStatus.Success,
                            TransactionType = TransactionTypeEnum.Credit,
                            Note = "Logistic Fee",
                            UserId = Loggetadmin.Id,
                            WalletId = LogAdminWallet.Id,
                            TrackCode = TransactionCode,
                            TransactionSection = TransactionSection.Commission,
                            PayoutStatus = PayoutStatus.Ledger,
                            Description = "Logistics for Order "+orderid 
                        });
                        //send sms and email.
                        //
                        LogEmail = Loggetadmin.Email;
                        LogTitle = "Hi " + Logadmin.Surname;
                        LogSubject += LogAdminCommision;
                        LogMessage += "You have received a deposit of NGN" + LogAdminCommision.Value.ToString("#.##") + " for Order" + orderId+ ". <br>";
                        //
                        //

                        await _context.SaveChangesAsync();
                        IQueryable<Transaction> iTransact = from s in _context.Transactions
                                              .Where(x => x.TrackCode == tcodecolor)
                                                         select s;
                        var coloradd = GetRandomColor().ToString();
                        var coloraddk = GetRandomColor();
                        var coloradjd = GetRandomColor().ToString();
                        var cololradd = GetRandomColor();
                        var colormadd = GetRandomColor().ToString();
                        foreach (var c in iTransact)
                        {
                            c.Color = coloradd.Replace("Color [", "").Replace("]", "");
                            _context.Entry(c).State = EntityState.Modified;
                           
                        }
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception d)
                    {

                    }
                    try
                    {

                        var ProductCartlist = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.Tenant).Where(x => x.UserProfileId == profile.Id && x.CartStatus == CartStatus.Active).ToListAsync();

                        foreach (var p in ProductCartlist)
                        {

                            p.CartStatus = Enums.CartStatus.CheckOut;
                            _context.Attach(p).State = EntityState.Modified;

                        }
                        orderitems.Status = OrderStatus.Processing;
                        orderitems.AmountPaid = transaction.Amount;
                        orderitems.OrderId = orderitems.Id.ToString("0000000");
                        _context.Attach(orderitems).State = EntityState.Modified;

                        await _context.SaveChangesAsync();
                        foreach (var iOrder in orderitems.OrderItems)
                        {

                            iOrder.Status = OrderStatus.Processing;
                            _context.Attach(iOrder).State = EntityState.Modified;

                            TrackOrder itrack = new TrackOrder();
                            itrack.OrderItemId = iOrder.Id;
                            itrack.Status = "ORDER PLACED";
                            itrack.Date = DateTime.UtcNow.AddHours(1);
                            _context.TrackOrders.Add(itrack);

                            TrackOrder itrack2 = new TrackOrder();
                            itrack2.OrderItemId = iOrder.Id;
                            itrack2.Status = "PENDING CONFIRMATION";
                            itrack2.Date = DateTime.UtcNow.AddHours(1);
                            _context.TrackOrders.Add(itrack2);

                            await _context.SaveChangesAsync();

                            var ProductLink = Url.Page(
                                       "/ProductInfo",
                                       pageHandler: null,
                                       values: new { area = "", id = iOrder.ProductId, name = iOrder.Product.Name, desc = iOrder.Product.ShortDescription },
                                       protocol: Request.Scheme);
                            string op = $"href='{HtmlEncoder.Default.Encode(ProductLink)}'";
                            string links = string.Format("<a {0}><td>{1}</td></a>", op, iOrder.Product.Name);
                            
                            var oitem = await _context.OrderItems.Include(x => x.Product).Include(x => x.Product.ProductPictures).FirstOrDefaultAsync(x => x.Id == iOrder.Id);
                            decimal total = iOrder.Product.Price * iOrder.Quantity;
                            var ImageUrl = "<img src=\"https://ahioma.com/" + oitem.Product.ProductPictures.FirstOrDefault().PictureUrlThumbnail + "\" height=\"40\"/>";
                            itemlist = itemlist + string.Format("<tr align=\"center\" valign=\"middle\"><td>{0}</td>{1}<td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>", ImageUrl, links, iOrder.Itemcolor ?? "--", iOrder.ItemSize ?? "--", iOrder.Product.Price, iOrder.Quantity, total);

                        }
                        //send sms and email. store
                        //

                        OrderEmail = user.Email;
                        OrderPhone = user.PhoneNumber;
                        OrderTitle = userProfile.Surname;
                        OrderSMS = "Your Ahioma Order - " + orderitems.ReferenceId + " is Pending Confirmation. we will get back to you. thanks for shopping on Ahioma";
                        AdminOrderSMS = "New Ahioma Order - " + orderitems.ReferenceId + " is Pending Confirmation. Kindly Call for confirmation from shop.";
                        OrderSubject = "Your Ahioma Order - " + orderitems.ReferenceId + " is been Processed";
                        OrderMessage = string.Format("Your Order with transaction Code {0} has been Placed. Delivery update coming soon <h4 style=\"text-align:center;color:#000000;\">Order Information</h4><table border=\"1\" style=\"color:#000000;\"><tr><th>Image</th><th>Product</th><th>Color</th><th>Size</th><th>Amount</th><th>Quantity</th><th>Total</th></tr>{1}</table>", TransactionCode, itemlist);
                        //AdminOrderEmail = string.Format("New Ahioma Order Product - {0} has been Orderd. Delivery update coming soon <br><h4 style=\"text-align:center;\">Order Information</h4><br><br><table border=\"1\"><tr><th>Image</th><th>Product</th><th>Color</th><th>Size</th><th>Amount</th></tr>{1}</table>", TransactionCode, itemlist);

                        //


                    }
                    catch (Exception d)
                    {

                    }
                //    string CheckTrackOrderId = HttpContext.Session.GetString("TrackOrderId");
                //    if (CheckTrackOrderId == null)
                //    {
                //        HttpContext.Session.Remove("TrackOrderId");
                //    }
                //    string CheckCartId = HttpContext.Session.GetString("CartUserId");
                //    if (CheckCartId == null)
                //    {
                //        HttpContext.Session.Remove("CartUserId");
                //    }

                //    send emails and sms
                //declear notification variable
                // shopEmail
                // shopTitle
                // shopSMS
                // shopMessageSubject
                // shopMessage

                var subjectShop = "Deposit of NGN" + shopMessageSubject.ToString("#.##") + " received";
                    await _emailSender.SendToOne(shopEmail, subjectShop, shopTitle, shopMessage + " Visit shop for more.https://ahioma.com/");
                    await _emailSender.SMSToOne(shopPhone, shopSMS);
                    ////
                    // soaEmail
                    // soaTitle
                    // soaMessageSubject
                    // soaSMS
                    // soaMessage
                    ////
                    var subjectSoa = "Deposit of NGN" + soaMessageSubject.ToString("#.##") + " received";
                    await _emailSender.SendToOne(soaEmail, subjectSoa, soaTitle, soaMessage + " Visit shop for more.https://ahioma.com/");
                    await _emailSender.SMSToOne(soaPhone, soaSMS);
                    // RefsoaEmail
                    // RefsoaTitle
                    // RefsoaMessageSubject
                    // RefsoaSMS
                    // RefsoaMessage
                    ////
                    ///
                    var subjectRefSoa = "Deposit of NGN" + RefsoaMessageSubject.ToString("#.##") + " received";
                    await _emailSender.SendToOne(RefsoaEmail, subjectRefSoa, RefsoaTitle, RefsoaMessage + " Visit shop for more.https://ahioma.com/");
                    await _emailSender.SMSToOne(RefsoaPhone, RefsoaSMS);


                    //customer
                    var customerRefSoa = "Deposit of NGN" + RefCustomerMessageSubject.ToString("#.##") + " received";
                    await _emailSender.SendToOne(RefCustomerEmail, customerRefSoa, RefCustomerTitle, RefCustomerMessage + " Visit shop for more.https://ahioma.com/");
                    await _emailSender.SMSToOne(RefCustomerPhone, RefCustomerSMS);


                    // AdminEmail
                    // AdminTitle
                    // AdminSubject
                    // AdminMessage
                    ////
                    ////
                    await _emailSender.SendToOne(AdminEmail, AdminSubject, AdminTitle, AdminMessage + " Visit shop for more.https://ahioma.com/");

                    // OrderEmail
                    // OrderTitle
                    // OrderSMS
                    // OrderSubject
                    // OrderMessage

                    await _emailSender.SMSToOne("08165680904,07060530000,08037915777,08165529721,07069168978", AdminOrderSMS);

                    var link = Url.Page(
                     "/Account/MyOrders",
                     pageHandler: null,
                     values: new { area = "User" },
                     protocol: Request.Scheme);
                    MailMessage mail = new MailMessage();
                    try
                    {
                        StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "CustomerOrder.html"));
                        //create the mail message 
                        MailAddress addr = new MailAddress(user.Email);
                        string username = addr.User;
                        string domain = addr.Host;

                        string mailmsg = sr.ReadToEnd();
                        mailmsg = mailmsg.Replace("{customername}", username);
                        mailmsg = mailmsg.Replace("{ListItems}", OrderMessage);
                        mailmsg = mailmsg.Replace("{link}", link);


                        mail.Body = mailmsg;
                        sr.Close();
                    }
                    catch (Exception c) { }

                    await _emailSender.NewSendToOne(user.Email, "New Order", mail);
                    await _emailSender.SendToMany("onwukaemeka41@gmail.com;logistics.ahioma@gmail.com", "Ahioma Order", OrderTitle, AdminOrderEmail);
                   await _emailSender.SMSToOne(OrderPhone, OrderSMS);
                  //  await _emailSender.SendToOne(OrderEmail, OrderSubject, OrderTitle, OrderMessage);
                    return RedirectToPage("/Account/MyOrders", new { area = "User", message = StatusMessage });
                }



                return Page();





            }

        

            return Page();
    }

       public static Color[] colors = { Color.Red, Color.Green, Color.DeepPink, Color.Brown,
        Color.YellowGreen, Color.Gold, Color.OrangeRed, Color.MediumVioletRed,
        Color.PaleVioletRed, Color.BlanchedAlmond, Color.ForestGreen, Color.PaleGreen,
        Color.PaleTurquoise, Color.LightSalmon, Color.SeaGreen, Color.RosyBrown};
        static Color GetRandomColor()
        {
            var random = new Random();
            return colors[random.Next(colors.Length)];
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
