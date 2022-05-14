using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    public class FlutterCallBackModel : PageModel
    {

        private readonly IWalletRepository _walletAppService;
        private readonly IUserProfileRepository _profile;
        private readonly AhiomaDbContext _context;
        private readonly ITransactionRepository _transactionAppService;
        private readonly IFlutterTransactionService _flutterTransactionAppService;
        private readonly IUserLogging _log;

        private readonly IConfiguration _config;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSendService _emailSender;

        public FlutterCallBackModel(ITransactionRepository transactionAppService,
          IFlutterTransactionService flutterTransactionAppService,
          IEmailSendService emailSender,
          AhiomaDbContext context,
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
        //public async Task<IActionResult> OnGetAsync()
        //{
        //    // tx_ref = ref &transaction_id = 30490 & status = successful

        //    // = checkout
        //    var source = HttpContext.Request.Query["source"].ToString();
        //    var tranxRef = HttpContext.Request.Query["tx_ref"].ToString();
        //    var transaction_id = HttpContext.Request.Query["transaction_id"].ToString();
        //    var status = HttpContext.Request.Query["status"].ToString();

        //    //declear notification variable
        //    var shopEmail = "";
        //    var shopTitle = "";
        //    var shopSMS = "";
        //    var shopPhone = "";
        //    var shopMessageSubject = "";
        //    var shopMessage = "";
        //    //
        //    var soaEmail = "";
        //    var soaTitle = "";
        //    var soaMessageSubject = "";
        //    var soaSMS = "";
        //    var soaPhone = "";
        //    var soaMessage = "";
        //    //

        //    var RefsoaEmail = "";
        //    var RefsoaTitle = "";
        //    var RefsoaMessageSubject = "";
        //    var RefsoaPhone = "";
        //    var RefsoaSMS = "";
        //    var RefsoaMessage = "";
        //    //
        //    var AdminEmail = "";
        //    var AdminTitle = "";
        //    var AdminSubject = "";
        //    var AdminMessage = "";
        //    //

        //   var OrderEmail = "";
        //   var OrderTitle = "";
        //   var OrderSMS = "";
        //   var OrderPhone = "";
        //    var OrderSubject = "";
        //   var OrderMessage = "";

        //    //
        //    string itemlist = "";
        //    //
        //    string TransactionCode = "";

          
        //    if (tranxRef != null)
        //    {

        //        if (tranxRef != null)
        //        {
        //            var response = await _flutterTransactionAppService.VerifyTransaction(transaction_id);
        //            //var response = await _flutterTransactionAppService.VerifyTransaction(tranxRef);
        //            long t_id = Convert.ToInt64(tranxRef);

        //            var transaction = await _transactionAppService.GetById(t_id);
        //            transaction.TransactionReference = transaction_id;
        //            var updatemain = await _transactionAppService.Update(transaction);
                    
        //            var user1 = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
        //            //var user = await _userManager.GetUserAsync(User);
        //            try
        //            {
        //                var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
        //                var lognew = await _log.LogData(transaction.UserId, "", "");
        //                Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
        //                _context.Attach(Userlog).State = EntityState.Modified;
        //                await _context.SaveChangesAsync();

        //            }
        //            catch (Exception s)
        //            {

        //            }
        //            var wallet = await _walletAppService.GetWallet(transaction.UserId);
        //            if (response == null)
        //            {
        //                transaction.WalletId = wallet.Id;
        //                transaction.Status = Enums.EntityStatus.Failed;
        //                transaction.TransactionReference = tranxRef;
        //                transaction.Note = "Online Deposit";
        //                var update = await _transactionAppService.Update(transaction);
        //                StatusMessage = $"Error! Transaction with Reference {transaction_id} failed.";
        //                StatusMessageSend = $"Transaction with Reference {transaction_id} failed. We help you sell more. Ahioma";
        //                await _signInManager.SignInAsync(user1.User, isPersistent: false);


        //                await _emailSender.SendToOne(user1.User.Email, "AHIA PAY", "Hi, "+ user1.Fullname, StatusMessageSend);


        //                await _emailSender.SMSToOne(user1.User.PhoneNumber, StatusMessageSend);


        //                return Page();
        //            }
        //            if (response.data.status == "successful" || status == "completed")
        //            {

                        
        //                Amount = transaction.Amount;

        //                if (transaction == null)
        //                {
        //                    TempData["StatusMessage"] = $"Transaction with Reference {transaction_id} was successful. But Wallet was not updated. Please contact Help Desk.";
        //                    StatusMessage = $"Transaction with Reference {transaction_id} was successful. But Wallet was not updated. Please contact Help Desk.";
        //                    await _signInManager.SignInAsync(user1.User, isPersistent: false);
        //                    await _emailSender.SendToOne(user1.User.Email, "AHIA PAY", "Hi, " + user1.Fullname, StatusMessage);


        //                    await _emailSender.SMSToOne(user1.User.PhoneNumber, StatusMessage);

        //                    return Page();
        //                }
        //                else if (!string.IsNullOrEmpty(transaction.TransactionReference))
        //                {
        //                    TempData["StatusMessage"] = $"Transaction with Reference {transaction_id} was successful.";
        //                    StatusMessage = $"Transaction with Reference {transaction_id} was successful.";
                          

        //                    WalletTotal = wallet.WithdrawBalance;
        //                    transaction.WalletId = wallet.Id;
        //                    transaction.Status = Enums.EntityStatus.Success;
        //                    transaction.TransactionReference = transaction_id;
        //                    transaction.DateOfTransaction = DateTime.UtcNow.AddHours(1);
        //                    var update = await _transactionAppService.Update(transaction);

        //                    wallet.WithdrawBalance += transaction.Amount;
        //                    wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
        //                    await _walletAppService.Update(wallet);
        //                    var walletcurrent = await _walletAppService.GetWallet(transaction.UserId);
        //                    var user = await _userManager.FindByIdAsync(transaction.UserId);
        //                    var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(x=>x.UserId == user.Id);

        //                    await _signInManager.SignInAsync(user, isPersistent: false);
        //                    TempData["StatusMessage"] = $"Transaction with Reference {transaction_id} was successful.";
        //                    StatusMessage = $"Transaction with Reference {transaction_id} was successful.";
        //                    await _emailSender.SendToOne(user1.User.Email, "AHIA PAY", "Hi, " + user1.Fullname, StatusMessage);


        //                    await _emailSender.SMSToOne(user1.User.PhoneNumber, StatusMessage);
        //                    if (source == "checkout")
        //                    {
        //                        var newtransaction = await _transactionAppService.CreateTransaction(new Transaction
        //                        {
        //                            Amount = transaction.Amount,
        //                            DateOfTransaction = DateTime.UtcNow.AddHours(1),
        //                            Status = EntityStatus.Success,
        //                            TransactionType = TransactionTypeEnum.Debit,
        //                            Note = "Online Order",
        //                            UserId = user.Id,
        //                            WalletId = wallet.Id,
        //                            Description = "Online Order Transaction"
        //                        });

        //                        wallet.WithdrawBalance -= transaction.Amount;
        //                        wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
        //                        await _walletAppService.Update(wallet);

        //                        //update store wallets
        //                        var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
        //                        var ProductCarts = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.Tenant).Where(x => x.UserProfileId == profile.Id && x.CartStatus == CartStatus.Active).ToListAsync();
        //                        try
        //                        {
        //                            foreach (var p in ProductCarts)
        //                            {
        //                                //generate token for tracking
        //                                string date1 = DateTime.UtcNow.AddHours(1).ToString("ssfff");

        //                                // The random number sequence
        //                                Random num = new Random();

        //                                // Create new string from the reordered char array
        //                                string rand = new string(date1.ToCharArray().
        //                                                OrderBy(s => (num.Next(2) % 2) == 0).ToArray());

        //                                var code = Token(5);
        //                                //
        //                                var xxx = date1 + code;
        //                                string TokenTracker = xxx;
        //                                string xNumber = new string(TokenTracker.ToCharArray().
        //                                                OrderBy(s => (num.Next(2) % 2) == 0).ToArray());
        //                                 TransactionCode = xNumber.Substring(1, 8).ToUpper();

        //                                //get product
        //                                var shop = await _context.Tenants.Include(x=>x.User).Include(x=>x.UserProfile).FirstOrDefaultAsync(x => x.Id == p.Product.TenantId);
        //                                //get store wallet
        //                                var storeWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == shop.UserId);
        //                                //get commision
        //                                decimal pDivision = Convert.ToDecimal(p.Product.Commision);
        //                                decimal productAmountAfterCommisionAdd = pDivision / Convert.ToDecimal(100);
        //                                decimal productAmountAfterCommision = p.Product.Price * productAmountAfterCommisionAdd;
        //                                //update store wallet
                                      
        //                                var storewallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == p.Product.Tenant.UserId);
        //                                var amountCommision = Convert.ToDecimal(p.Product.Price) * (Convert.ToDecimal(p.Product.Commision) / Convert.ToDecimal(100));
        //                                var shopProductBalance = Convert.ToDecimal(p.Product.Price) - amountCommision;
        //                                storewallet.Balance += shopProductBalance;
        //                                storewallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
        //                                await _walletAppService.Update(storewallet);

        //                                //create transaction for store
        //                                var storeNewewtransactionStore = await _transactionAppService.CreateTransaction(new Transaction
        //                                {
        //                                    Amount = shopProductBalance,
        //                                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
        //                                    Status = EntityStatus.Success,
        //                                    TransactionType = TransactionTypeEnum.Credit,
        //                                    Note = "Sales",
        //                                    UserId = shop.UserId,
        //                                    WalletId = storewallet.Id,
        //                                    TrackCode = TransactionCode,
        //                                    Description = "Amount after commision on product " + p.Product.Name + " with ID " + p.ProductId
        //                                });
        //                                //send sms and email. store
        //                                //
        //                                 shopEmail = shop.User.Email;
        //                                 shopPhone = shop.User.PhoneNumber;
        //                                shopTitle = "Hi "+ shop.BusinessName;
        //                                 shopSMS = "Deposit of NGN" + shopProductBalance + " received for " + p.Product.Name;
        //                                 shopMessageSubject = "Deposit of NGN" + shopProductBalance + " received";
        //                                 shopMessage = "You have received a deposit of NGN" + shopProductBalance + " for "+p.Product.Name+ ". Visit shop for more. https://ahioma.com/";
        //                                //

        //                                //get SOA
        //                                var soaUser = await _context.UserProfiles.Include(x=>x.User).FirstOrDefaultAsync(x => x.UserId == shop.CreationUserId);
        //                                //get soa wallet
        //                                var soaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == soaUser.UserId);
        //                                //soa commision of 25% of shop commision
        //                                decimal soaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(25) / Convert.ToDecimal(100));
        //                                soaWallet.Balance += soaCommision;
        //                                soaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
        //                                await _walletAppService.Update(soaWallet);

        //                                //create soa transaction
        //                                var SoaNewtransactionStore = await _transactionAppService.CreateTransaction(new Transaction
        //                                {
        //                                    Amount = soaCommision,
        //                                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
        //                                    Status = EntityStatus.Success,
        //                                    Note = "Commision",
        //                                    TransactionType = TransactionTypeEnum.Credit,
        //                                    UserId = soaUser.UserId,
        //                                    WalletId = soaWallet.Id,
        //                                    TrackCode = TransactionCode,
        //                                    Description = "Product Sale Commission from "+ shop.BusinessName
        //                                });
        //                                //send sms and email.
        //                                //
        //                                //
        //                                 soaEmail = soaUser.User.Email;
        //                                 soaPhone = soaUser.User.PhoneNumber;
        //                                soaTitle = "Hi " + soaUser.Surname;
        //                                 soaMessageSubject = "Deposit of NGN" + soaCommision + " received";
        //                                 soaSMS = "Deposit of NGN" + soaCommision + " received for " + p.Product.Name + "for SO ("+shop.BusinessName+").";
        //                                 soaMessage = "You have received a deposit of NGN" + soaCommision + " for " + p.Product.Name + ". Visit shop for more. https://ahioma.com/";
        //                                //
        //                                //

        //                                //get Referral
        //                                if (!String.IsNullOrEmpty(soaUser.ReferralLink))
        //                                {


        //                                    var ReferralUser = await _context.UserProfiles.Include(x=>x.User).FirstOrDefaultAsync(x => x.IdNumber == soaUser.ReferralLink);
        //                                    //get soa wallet
        //                                    var ReferralSoaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == ReferralUser.UserId);
        //                                    //soa commision of 5% of referral commision
        //                                    decimal ReferralSoaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
        //                                    ReferralSoaWallet.Balance += ReferralSoaCommision;
        //                                    ReferralSoaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
        //                                    await _walletAppService.Update(ReferralSoaWallet);

        //                                    //create soa transaction
        //                                    var ReferralSoaNewtransactionStore = await _transactionAppService.CreateTransaction(new Transaction
        //                                    {
        //                                        Amount = ReferralSoaCommision,
        //                                        DateOfTransaction = DateTime.UtcNow.AddHours(1),
        //                                        Status = EntityStatus.Success,
        //                                        TransactionType = TransactionTypeEnum.Credit,
        //                                        Note = "Referral Commision",
        //                                        UserId = ReferralUser.UserId,
        //                                        WalletId = ReferralSoaWallet.Id,
        //                                        TrackCode = TransactionCode,
        //                                        Description = "Referral Commision from " +soaUser.Fullname
        //                                    });
        //                                    //send sms and email.
        //                                    //
        //                                    //
        //                                     RefsoaEmail = ReferralUser.User.Email;
        //                                     RefsoaPhone = ReferralUser.User.PhoneNumber;
        //                                    RefsoaTitle = "Hi " + ReferralUser.Surname;
        //                                     RefsoaMessageSubject = "Deposit of NGN" + ReferralSoaCommision + " received";
        //                                     RefsoaSMS = "Deposit of NGN" + ReferralSoaCommision + " received for " + p.Product.Name;
        //                                     RefsoaMessage = "You have received a deposit of NGN" + ReferralSoaCommision + " for " + p.Product.Name + ". Visit shop for more. https://ahioma.com/";
        //                                    //
        //                                    //

        //                                }
        //                                //admin commission
        //                                var getadmin = await _userManager.FindByEmailAsync("jinmcever@gmail.com");
        //                                var admin = await _context.UserProfiles.Include(x=>x.User).FirstOrDefaultAsync(x => x.UserId == getadmin.Id);
        //                                //get soa wallet
        //                                var AdminWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == getadmin.Id);
        //                                //soa commision of 70% of shop commision
        //                                decimal AdminCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(70) / Convert.ToDecimal(100));
        //                                AdminWallet.Balance += AdminCommision;
        //                                AdminWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
        //                                await _walletAppService.Update(AdminWallet);

        //                                //create soa transaction
        //                                var AdmintransactionStore = await _transactionAppService.CreateTransaction(new Transaction
        //                                {
        //                                    Amount = AdminCommision,
        //                                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
        //                                    Status = EntityStatus.Success,
        //                                    TransactionType = TransactionTypeEnum.Credit,
        //                                    Note = "Ahioma Commision",
        //                                    UserId = getadmin.Id,
        //                                    WalletId = AdminWallet.Id,
        //                                    TrackCode = TransactionCode,
        //                                    Description = "Commision from " + soaUser.Fullname
        //                                });
        //                                //send sms and email.
        //                                //
        //                                 AdminEmail = getadmin.Email;
        //                                 AdminTitle = "Hi " + admin.Surname;
        //                                 AdminSubject = "Deposit of NGN" + AdminCommision + " received";
        //                                 AdminMessage = "You have received a deposit of NGN" + AdminCommision + " for " + p.Product.Name + ". Visit shop for more. https://ahioma.com/";
        //                                //
        //                                //
        //                            }
        //                            await _context.SaveChangesAsync();
        //                        }
        //                        catch (Exception d)
        //                        {

        //                        }
        //                        try
        //                        {
        //                            foreach (var p in ProductCarts)
        //                            {

        //                                p.CartStatus = Enums.CartStatus.CheckOut;
        //                                _context.Attach(p).State = EntityState.Modified;

        //                                //
        //                                Order neworder = new Order();
        //                                neworder.AmountPaid = p.Product.Price;
        //                                //neworder.DateOfTransaction = DateTime.UtcNow.AddHours(1);
        //                                //neworder.DeliveryMethod = p.DeliveryMethod;
        //                                //neworder.Itemcolor = p.Itemcolor;
        //                                //neworder.ItemSize = p.ItemSize;
        //                                //neworder.ProductId = p.ProductId;
        //                                //neworder.Quantity = p.Quantity;
        //                                //neworder.UserProfileId = p.UserProfileId;
        //                                //neworder.TotalAmount = p.Product.Price;
        //                                //neworder.Status = OrderStatus.Processing;
        //                                //neworder.OrderId = "";
        //                                //neworder.GroupOrderId = TransactionCode;
        //                                _context.Orders.Add(neworder);
        //                                await _context.SaveChangesAsync();
        //                                //update orderid
        //                                var orderupdate = await _context.Orders.FirstOrDefaultAsync(x => x.Id == neworder.Id);
        //                                orderupdate.OrderId = neworder.Id.ToString("0000000");
        //                                _context.Attach(orderupdate).State = EntityState.Modified;
        //                                await _context.SaveChangesAsync();

        //                                //long? id, string name, string shop, string mktstate, string mktaddress
        //                                //var ProductLink = Url.Page(
        //                                //           "/ProductInfo",
        //                                //           pageHandler: null,
        //                                //           values: new { area = "", id = orderupdate.ProductId, name = orderupdate.Product.Name, desc = orderupdate.Product.ShortDescription },
        //                                //           protocol: Request.Scheme);

        //                                //itemlist = itemlist + string.Format("<tr><td>{0}</td><a href=\"{1}\"><td>{2}</td></a><td>{3}</td><td>{4}</td><td>{5}</td></tr>", orderupdate.OrderId, ProductLink, orderupdate.Product.Name, orderupdate.Itemcolor, orderupdate.ItemSize, orderupdate.AmountPaid) ;

        //                            }
                                   
                                     
        //                            //send sms and email. store
        //                            //
        //                            OrderEmail = user.Email;
        //                            OrderPhone = user.PhoneNumber;
        //                            OrderTitle = "Hi " + userProfile.Surname;
        //                            OrderSMS = "Your Ahioma Order - " + TransactionCode + " is been Processed";
        //                            OrderSubject = "Your Ahioma Order - " + TransactionCode + " is been Processed";
        //                            OrderMessage = string.Format("Your Ahioma Product - {0} has been Orderd. Delivery update coming soon <br><h4 style=\"text-align:center;\">Order Information</h4><br><br><table><tr><th>ID</th><th>Product</th><th>Color</th><th>Size</th><th>Amount</th></tr>{1}</table>", TransactionCode, itemlist);
                                   
        //                            //


        //                        }
        //                        catch (Exception d)
        //                        {

        //                        }
        //                        //send emails and sms
        //                        //declear notification variable
        //                        // shopEmail
        //                        // shopTitle
        //                        // shopSMS
        //                        // shopMessageSubject
        //                        // shopMessage
        //                        await _emailSender.SendToOne(shopEmail, shopMessageSubject, shopTitle, shopMessage);
        //                        await _emailSender.SMSToOne(shopPhone, shopSMS);
        //                        ////
        //                        // soaEmail
        //                        // soaTitle
        //                        // soaMessageSubject
        //                        // soaSMS
        //                        // soaMessage
        //                        ////
        //                        await _emailSender.SendToOne(soaEmail, soaMessageSubject, soaTitle, soaMessage);
        //                        await _emailSender.SMSToOne(soaPhone, soaSMS);
        //                        // RefsoaEmail
        //                        // RefsoaTitle
        //                        // RefsoaMessageSubject
        //                        // RefsoaSMS
        //                        // RefsoaMessage
        //                        ////
        //                        await _emailSender.SendToOne(RefsoaEmail, RefsoaMessageSubject, RefsoaTitle, RefsoaMessage);
        //                        await _emailSender.SMSToOne(RefsoaPhone, RefsoaSMS);
        //                        // AdminEmail
        //                        // AdminTitle
        //                        // AdminSubject
        //                        // AdminMessage
        //                        ////
        //                        ////
        //                        await _emailSender.SendToOne(AdminEmail, AdminSubject, AdminTitle, AdminMessage);

        //                        // OrderEmail
        //                        // OrderTitle
        //                        // OrderSMS
        //                        // OrderSubject
        //                        // OrderMessage
        //                        await _emailSender.SendToMany("onwukaemeka41@gmail.com", OrderSubject, OrderTitle, OrderMessage);
        //                        await _emailSender.SMSToOne(OrderPhone+"07060530000,08165680904", OrderSMS);
        //                        return RedirectToPage("/Account/MyOrders", new { area = "User", message = StatusMessage });
        //                    }



        //                    return Page();
        //                }


        //            }

        //            else
        //            {
        //                transaction.WalletId = wallet.Id;
        //                transaction.Status = Enums.EntityStatus.Failed;
        //                transaction.TransactionReference = tranxRef;
        //                var update = await _transactionAppService.Update(transaction);
        //                StatusMessage = $"Error! Transaction with Reference {transaction_id} failed.";
        //                return Page();

        //            }

        //        }

        //    }

        //    return Page();
        //}

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
