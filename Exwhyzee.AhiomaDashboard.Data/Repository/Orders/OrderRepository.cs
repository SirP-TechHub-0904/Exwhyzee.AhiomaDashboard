using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IWalletRepository _walletAppService;
        private readonly IUserProfileRepository _profile;
        private readonly AhiomaDbContext _context;
        private readonly ITransactionRepository _transactionAppService;
        private readonly IFlutterTransactionService _flutterTransactionAppService;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IConfiguration _config;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMessageRepository _message;

        public OrderRepository(ITransactionRepository transactionAppService,
          IFlutterTransactionService flutterTransactionAppService,
          AhiomaDbContext context, IHostingEnvironment hostingEnv,
          IUserProfileRepository profile, SignInManager<IdentityUser> signInManager,
          UserManager<IdentityUser> userManager, IWalletRepository walletAppService,
          IConfiguration configuration, IMessageRepository message)
        {
            _transactionAppService = transactionAppService;
            _flutterTransactionAppService = flutterTransactionAppService;
            _userManager = userManager;
            _profile = profile;
            _walletAppService = walletAppService;
            _signInManager = signInManager;
            _config = configuration;
            _context = context;
            _hostingEnv = hostingEnv;
            _message = message;
        }

        //source, tranxRef, transaction_id, 
        //status, customerRef, orderid, ahiapaystatus, Ahia_transac_Id, transactiontype
        public async Task<string> Insert(string source, string tranxRef,
            string transaction_id, string status, string customerRef, string orderid,
            string ahiapaystatus, string Ahia_transac_Id, string transactiontype, string from, string skip)
        {
            //signin user
            long t_id = 0;
            Transaction transaction = new Transaction();
            Order order = new Order();
            UserProfile user = new UserProfile();
            Wallet wallet = new Wallet();
            #region ...
            if (!String.IsNullOrEmpty(tranxRef))
            {
                t_id = Convert.ToInt64(tranxRef);
                if (t_id > 0)
                {
                                        //data contexts
                    transaction = await _transactionAppService.GetById(t_id);
                    user = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
                    wallet = await _walletAppService.GetWallet(transaction.UserId);
                    await _signInManager.SignInAsync(user.User, isPersistent: false);
                }
                else
                {
                    return "Fail: Not Found E510011";
                }
            }

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

            //online deposite 
            if (!String.IsNullOrEmpty(transactiontype))
            {
                if (transactiontype == "deposit")
                {
                    if (!String.IsNullOrEmpty(tranxRef))
                    {
                        var response = await _flutterTransactionAppService.VerifyTransaction(transaction_id);

                        //data contexts

                        //add transaction paramaeters
                        transaction.TransactionReference = transaction_id;
                        transaction.WalletId = wallet.Id;
                        transaction.TrackCode = TransactionCode;
                        transaction.Note = "Online Deposit";
                        transaction.From = from;
                        if (response.data != null)
                        {
                            if (response.data.status == "successful" || status == "completed")
                            {
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
                                a.UserProfileId = user.Id;
                                a.TransactionType = "Cr";
                                a.Source = "Online Deposit";
                                a.From = from;
                                _context.WalletHistories.Add(a);
                            }
                            else
                            {
                                transaction.Status = Enums.EntityStatus.Failed;
                                _context.Entry(transaction).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            transaction.Status = Enums.EntityStatus.Failed;
                            _context.Entry(transaction).State = EntityState.Modified;
                        }
                        //
                        try
                        {
                            await _context.SaveChangesAsync();
                            //
                        }
                        catch (Exception c)
                        {

                        }
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
                                sms.Recipient = user.User.PhoneNumber.Replace(" ", "");
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
                                mEmailContent = mEmailContent.Replace("|Surname|", user.Surname);
                                mEmailContent = mEmailContent.Replace("|Description|", "Online Deposit");


                                AddMessageDto email = new AddMessageDto();
                                email.Content = mEmailContent;
                                email.Recipient = user.User.Email.Replace(" ", "");
                                email.NotificationType = Enums.NotificationType.Email;
                                email.NotificationStatus = Enums.NotificationStatus.NotSent;
                                email.Retries = 0;
                                email.Title = "Online Deposit";

                                var sts = await _message.AddMessage(email);
                                return "Ok Deposit";
                            }
                            else
                            {
                                return "Fail Deposit";
                            }
                        }
                        catch (Exception c)
                        {
                            return "Fail Deposit";
                        }
                    }
                }
            }

            else if (source == "checkout")
            {
                if (!String.IsNullOrEmpty(orderid))
                {
                    long orderId = Convert.ToInt64(orderid);

                    //
                    order = await _context.Orders.Include(x => x.OrderItems).Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.Id == orderId);

                    if (order == null)
                    {
                        return "Fail: invalid Order. E41001";
                    }
                    //
                }
                else
                {
                    return "Fail: Order not found. E31001";
                }

                user = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == order.UserProfile.User.Id);
                wallet = await _walletAppService.GetWallet(user.UserId);
                if (skip != "skip")
                {
                    await _signInManager.SignInAsync(user.User, isPersistent: false);
                }
                if (ahiapaystatus == "fail")
                {
                    //update order
                    order.Note = "Payment Not Successful";
                    order.Status = OrderStatus.Pending;
                    order.TransactionId = transaction.Id;
                    order.AmountPaid = 0;
                    _context.Attach(order).State = EntityState.Modified;
                    foreach (var p in order.OrderItems)
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
                        itrack2.Status = "PENDING - PAYMENT UNSUCCESSFUL";
                        itrack2.Date = DateTime.UtcNow.AddHours(1);
                        _context.TrackOrders.Add(itrack2);


                    }
                    var ProductCartlist = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.Tenant).Where(x => x.UserProfileId == user.Id && x.CartStatus == CartStatus.Active).ToListAsync();

                    foreach (var p in ProductCartlist)
                    {

                        p.CartStatus = Enums.CartStatus.CheckOut;
                        _context.Attach(p).State = EntityState.Modified;

                    }
                    await _context.SaveChangesAsync();
                    return "Fail: transaction not found. E11001";
                }
                if (Ahia_transac_Id != null && ahiapaystatus == "success")
                {
                    long tid = Convert.ToInt64(Ahia_transac_Id);
                    transaction = await _transactionAppService.GetById(tid);

                    transaction.TransactionReference = transaction_id;
                    transaction.WalletId = wallet.Id;
                    transaction.TrackCode = order.TrackCode;
                    transaction.Note = "Online Order Payment";
                    transaction.Status = Enums.EntityStatus.Success;
                    transaction.From = from;
                    _context.Entry(transaction).State = EntityState.Modified;
                    //update order
                    order.Note = "Payment Successful";
                    order.Status = OrderStatus.Processing;
                    order.TransactionId = transaction.Id;
                    order.AmountPaid = transaction.Amount;
                    _context.Attach(order).State = EntityState.Modified;
                    foreach (var p in order.OrderItems)
                    {
                        p.Note = "Payment Successful";
                        p.Status = OrderStatus.Processing;
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

                    }
                    var ProductCartlist = await _context.ProductCarts.Where(x => x.UserProfileId == user.Id && x.CartStatus == Enums.CartStatus.Active && x.TrackOrderId != null).ToListAsync();

                    foreach (var p in ProductCartlist)
                    {

                        p.CartStatus = Enums.CartStatus.CheckOut;
                        _context.Attach(p).State = EntityState.Modified;

                    }

                }
                else if (!String.IsNullOrEmpty(transaction_id))
                {
                    if (!String.IsNullOrEmpty(tranxRef))
                    {
                        var response = await _flutterTransactionAppService.VerifyTransaction(transaction_id);
                        //data contexts

                        //add transaction paramaeters
                        transaction.TransactionReference = transaction_id;
                        transaction.WalletId = wallet.Id;
                        transaction.TrackCode = TransactionCode;
                        transaction.Note = "Online Order Payment";
                        transaction.From = from;
                        if (response.data != null)
                        {
                            if (response.data.status == "successful" || status == "completed")
                            {
                                transaction.Status = Enums.EntityStatus.Success;
                                _context.Entry(transaction).State = EntityState.Modified;
                                //update order
                                order.Note = "Payment Successful";
                                order.Status = OrderStatus.Processing;
                                order.TransactionId = transaction.Id;
                                order.AmountPaid = transaction.Amount;
                                _context.Attach(order).State = EntityState.Modified;
                                foreach (var p in order.OrderItems)
                                {
                                    p.Note = "Payment Successful";
                                    p.Status = OrderStatus.Processing;
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

                                }

                                var ProductCartlist = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.Tenant).Where(x => x.UserProfileId == user.Id && x.CartStatus == CartStatus.Active).ToListAsync();

                                foreach (var p in ProductCartlist)
                                {

                                    p.CartStatus = Enums.CartStatus.CheckOut;
                                    _context.Attach(p).State = EntityState.Modified;

                                }
                            }
                            else
                            {
                                transaction.Status = Enums.EntityStatus.Failed;
                                _context.Entry(transaction).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            transaction.Status = Enums.EntityStatus.Failed;
                            _context.Entry(transaction).State = EntityState.Modified;

                            //update order
                            order.Note = "Payment Not Successful";
                            order.Status = OrderStatus.Pending;
                            order.AmountPaid = 0;
                            _context.Attach(order).State = EntityState.Modified;
                            foreach (var p in order.OrderItems)
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
                                itrack2.Status = "PENDING - PAYMENT UNSUCCESSFUL";
                                itrack2.Date = DateTime.UtcNow.AddHours(1);
                                _context.TrackOrders.Add(itrack2);


                            }

                            var ProductCartlist = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.Tenant).Where(x => x.UserProfileId == user.Id && x.CartStatus == CartStatus.Active).ToListAsync();

                            foreach (var p in ProductCartlist)
                            {

                                p.CartStatus = Enums.CartStatus.CheckOut;
                                _context.Attach(p).State = EntityState.Modified;

                            }
                            await _context.SaveChangesAsync();
                            return "Fail: transaction failed. E21001";
                        }
                        //


                    }
                    else
                    {
                        if (skip == "skip")
                        {
                            transaction = await _transactionAppService.GetById(Convert.ToInt64(transaction_id));
                            transaction.Status = EntityStatus.Success;
                            _context.Entry(transaction).State = EntityState.Modified;
                            order.Note = "Payment Successful";
                            order.Status = OrderStatus.Processing;
                            order.TransactionId = transaction.Id;
                            order.AmountPaid = transaction.Amount;
                            _context.Attach(order).State = EntityState.Modified;
                            foreach (var p in order.OrderItems)
                            {
                                p.Note = "Payment Successful";
                                p.Status = OrderStatus.Processing;
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

                            }
                            var ProductCartlist = await _context.ProductCarts.Where(x => x.UserProfileId == user.Id && x.CartStatus == Enums.CartStatus.Active && x.TrackOrderId != null).ToListAsync();

                            foreach (var p in ProductCartlist)
                            {

                                p.CartStatus = Enums.CartStatus.CheckOut;
                                _context.Attach(p).State = EntityState.Modified;

                            }
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            //update order
                            order.Note = "Payment Not Successful";
                            order.Status = OrderStatus.Pending;
                            order.AmountPaid = 0;
                            _context.Attach(order).State = EntityState.Modified;
                            foreach (var p in order.OrderItems)
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
                                itrack2.Status = "PENDING - PAYMENT UNSUCCESSFUL";
                                itrack2.Date = DateTime.UtcNow.AddHours(1);
                                _context.TrackOrders.Add(itrack2);


                            }
                            var ProductCartlist = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.Tenant).Where(x => x.UserProfileId == user.Id && x.CartStatus == CartStatus.Active).ToListAsync();

                            foreach (var p in ProductCartlist)
                            {

                                p.CartStatus = Enums.CartStatus.CheckOut;
                                _context.Attach(p).State = EntityState.Modified;

                            }
                            await _context.SaveChangesAsync();
                            return "Fail: transaction not found. E11001";
                        }
                    }

                }



                if (transaction.Status == Enums.EntityStatus.Success)
                {

                    //get ahioma account
                    var ahiomaaccount = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == "ahiomaorder@ahioma.com");
                    var ahiomawallet = await _walletAppService.GetWallet(ahiomaaccount.UserId);
                    //transaction and wallet
                    Transaction ahiomaTransaction = new Transaction();
                    ahiomaTransaction.Amount = order.AmountPaid;
                    ahiomaTransaction.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                    ahiomaTransaction.TransactionType = TransactionTypeEnum.Credit;
                    ahiomaTransaction.Note = "New Order";
                    ahiomaTransaction.UserId = ahiomawallet.UserId;
                    ahiomaTransaction.WalletId = ahiomawallet.Id;
                    ahiomaTransaction.TrackCode = order.TrackCode;
                    ahiomaTransaction.TransactionSection = TransactionSection.Sales;
                    ahiomaTransaction.PayoutStatus = PayoutStatus.Ledger;
                    ahiomaTransaction.Description = "Order from customer (" + order.UserProfile.Fullname + ")";
                    ahiomaTransaction.Status = Enums.EntityStatus.Success;
                    ahiomaTransaction.From = from;
                    ahiomaTransaction.UserProfileId = ahiomaaccount.Id;
                    _context.Transactions.Add(ahiomaTransaction);

                    //update wallet
                    ahiomawallet.Balance += ahiomaTransaction.Amount;
                    ahiomawallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                    _context.Entry(ahiomawallet).State = EntityState.Modified;

                    //update history
                    WalletHistory a = new WalletHistory();
                    a.Amount = ahiomaTransaction.Amount;
                    a.CreationTime = DateTime.UtcNow.AddHours(1);
                    a.LedgerBalance = ahiomawallet.Balance;
                    a.AvailableBalance = ahiomawallet.WithdrawBalance + ahiomaTransaction.Amount;
                    a.WalletId = ahiomawallet.Id;
                    a.UserId = ahiomawallet.UserId;
                    a.UserProfileId = ahiomaaccount.Id;
                    a.TransactionType = "Cr";
                    a.Source = "Online Order for "+orderid;
                    a.From = from;
                    _context.WalletHistories.Add(a);
                }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception c)
                {
                    return "Fail: Error #655111";
                }
                //get order by shopid
                IQueryable<OrderItem> iOrderItem = from s in _context.OrderItems
              .Include(x => x.Order)
              .Include(x => x.Product)
              .Include(x => x.Product.Tenant)
              .Include(x => x.Product.Tenant.User)
              .Include(x => x.Product.Tenant.UserProfile)
              .Include(x => x.Product.ProductPictures)
              .Include(x => x.Product.Tenant.TenantAddresses)
              .Where(x => x.OrderId == order.Id)
                                                   select s;

                var orderStatus = await iOrderItem.ToListAsync();
                var orderlistbyshop = orderStatus.Select(x => x.Product.TenantId).Distinct().ToList();
                var cz = orderlistbyshop.Count();
                try
                {
                    foreach (var shop in orderlistbyshop)
                    {
                        //
                        IQueryable<OrderItem> ShopOrderItem = from s in _context.OrderItems
        .Include(x => x.Order)
        .Include(x => x.Product)
        .Include(x => x.Product.Tenant)
        .Include(x => x.Product.Tenant.User)
        .Include(x => x.Product.Tenant.UserProfile)
        .Include(x => x.Product.ProductPictures)
        .Include(x => x.Product.Tenant.TenantAddresses)
        .Where(x => x.Product.TenantId == shop && x.Order.Id == order.Id)
                                                              select s;
                        var csdd = ShopOrderItem.Count();
                        //
                        string itemlist = "";
                        foreach (var item in ShopOrderItem)
                        {
                            var oitem = await _context.OrderItems.Include(x => x.Product).Include(x => x.Product.ProductPictures).FirstOrDefaultAsync(x => x.Id == item.Id);
                            decimal total = item.Product.Price * item.Quantity;
                            string img = "";
                            if (oitem.Product.ProductPictures.Count() > 0)
                            {
                                img = oitem.Product.ProductPictures.FirstOrDefault().PictureUrlThumbnail;
                            }


                            var ImageUrl = "<img src=\"https://ahioma.com/" + img + "\" height=\"40\"/>";
                            itemlist = itemlist + string.Format("<tr align=\"center\" valign=\"middle\"><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>", ImageUrl, item.Product.Name, item.Itemcolor ?? "--", item.ItemSize ?? "--", item.Product.Price, item.Quantity, total);

                        }
                        string table = string.Format("<table border=\"1\" style=\"color:#000000;\"><tr><th>Image</th><th>Product</th><th>Color</th><th>Size</th><th>Amount</th><th>Quantity</th><th>Total</th></tr>{0}</table>", itemlist);
                        //
                        string mSmsContent = "";
                        try
                        {
                            mSmsContent = await _message.GetMessage(Enums.ContentType.OrderFromShopSMS);
                        }
                        catch (Exception c) { }

                        //update content Data
                        mSmsContent = mSmsContent.Replace("|ItemCount|", ShopOrderItem.Count().ToString());
                        mSmsContent = mSmsContent.Replace("|Shopname|", ShopOrderItem.FirstOrDefault().Product.Tenant.BusinessName);
                        mSmsContent = mSmsContent.Replace("|Date|", order.DateOfOrder.ToString("dd/MM/yyyy hh:mm tt"));
                        mSmsContent = mSmsContent.Replace("|||", "\r\n");
                        //sms
                        AddMessageDto sms = new AddMessageDto();
                        sms.Content = mSmsContent;
                        sms.Recipient = ShopOrderItem.FirstOrDefault().Product.Tenant.User.PhoneNumber.Replace(" ", "");
                        sms.NotificationType = Enums.NotificationType.SMS;
                        sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                        sms.Retries = 0;
                        sms.Title = "New Order";
                        //
                        var stss = await _message.AddMessage(sms);

                        ////email
                        string mEmailContent = "";
                        try
                        {
                            mEmailContent = await _message.GetMessage(Enums.ContentType.OrderFromShopMail);
                        }
                        catch (Exception c) { }
                        string soaid = ShopOrderItem.FirstOrDefault().Product.Tenant.CreationUserId;
                        var soa = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == soaid);

                        ////update content Data
                        mEmailContent = mEmailContent.Replace("|Table|", table);
                        mEmailContent = mEmailContent.Replace("|Amount|", order.AmountPaid.ToString());
                        mEmailContent = mEmailContent.Replace("|Soname|", ShopOrderItem.FirstOrDefault().Product.Tenant.BusinessName);
                        mEmailContent = mEmailContent.Replace("|date|", order.DateOfOrder.ToString("dd/MM/yyyy hh:mm tt"));
                        mEmailContent = mEmailContent.Replace("|orderid|", order.OrderId);
                        mEmailContent = mEmailContent.Replace("|soaname|", soa.Fullname);
                        mEmailContent = mEmailContent.Replace("|soanumber|", soa.User.PhoneNumber);


                        AddMessageDto email = new AddMessageDto();
                        email.Content = mEmailContent;
                        email.Recipient = ShopOrderItem.FirstOrDefault().Product.Tenant.User.PhoneNumber.Replace(" ", "");
                        email.NotificationType = Enums.NotificationType.Email;
                        email.NotificationStatus = Enums.NotificationStatus.NotSent;
                        email.Retries = 0;
                        email.Title = "New Order";

                        var sts = await _message.AddMessage(email);
                        //return "Ok Deposit";
                    }

                }
                catch (Exception cc) { }
                //
                //message customer
                try
                {
                    IQueryable<OrderItem> ShopOrderItem = from s in _context.OrderItems
    .Include(x => x.Order)
    .Include(x => x.Product)
    .Include(x => x.Product.Tenant)
    .Include(x => x.Order.UserProfile)
    .Include(x => x.Order.UserProfile.User)
    .Include(x => x.Product.Tenant.User)
    .Include(x => x.Product.Tenant.UserProfile)
    .Include(x => x.Product.ProductPictures)
    .Include(x => x.Product.Tenant.TenantAddresses)
    .Where(x => x.Order.Id == order.Id)
                                                          select s;
                    var csdd = ShopOrderItem.Count();
                    //
                    string itemlist = "";
                    foreach (var item in ShopOrderItem)
                    {
                        var oitem = await _context.OrderItems.Include(x => x.Product).Include(x => x.Product.ProductPictures).FirstOrDefaultAsync(x => x.Id == item.Id);
                        decimal total = item.Product.Price * item.Quantity;
                        string img = "";
                        if (oitem.Product.ProductPictures.Count() > 0)
                        {
                            img = oitem.Product.ProductPictures.FirstOrDefault().PictureUrlThumbnail;
                        }


                        var ImageUrl = "<img src=\"https://ahioma.com/" + img + "\" height=\"40\"/>";
                        itemlist = itemlist + string.Format("<tr align=\"center\" valign=\"middle\"><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>", ImageUrl, item.Product.Name, item.Itemcolor ?? "--", item.ItemSize ?? "--", item.Product.Price, item.Quantity, total);

                    }
                    string table = string.Format("<table border=\"1\" style=\"color:#000000;\"><tr><th>Image</th><th>Product</th><th>Color</th><th>Size</th><th>Amount</th><th>Quantity</th><th>Total</th></tr>{0}</table>", itemlist);
                    //
                    string mSmsContent = "";
                    try
                    {
                        mSmsContent = await _message.GetMessage(Enums.ContentType.OrderFromCustomerSMS);
                    }
                    catch (Exception c) { }

                    //update content Data
                    mSmsContent = mSmsContent.Replace("|fullname|", user.Fullname.ToString());
                    mSmsContent = mSmsContent.Replace("|orderid|", ShopOrderItem.FirstOrDefault().Order.OrderId);
                    mSmsContent = mSmsContent.Replace("|Date|", order.DateOfOrder.ToString("dd/MM/yyyy hh:mm tt"));
                    mSmsContent = mSmsContent.Replace("|||", "\r\n");
                    //sms
                    AddMessageDto sms = new AddMessageDto();
                    sms.Content = mSmsContent;
                    sms.Recipient = user.User.PhoneNumber;
                    sms.NotificationType = Enums.NotificationType.SMS;
                    sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                    sms.Retries = 0;
                    sms.Title = "My Order";
                    //
                    var stss = await _message.AddMessage(sms);
                    //
                    AddMessageDto adminsms = new AddMessageDto();
                    adminsms.Content = "Admin Notic - " + mSmsContent;
                    adminsms.Recipient = "08165680904,07060530000,08037915777,08165529721,07069168978";
                    adminsms.NotificationType = Enums.NotificationType.SMS;
                    adminsms.NotificationStatus = Enums.NotificationStatus.NotSent;
                    adminsms.Retries = 0;
                    adminsms.Title = "New Order";
                    //
                    var stsss = await _message.AddMessage(adminsms);

                    ////email
                    string mEmailContent = "";
                    try
                    {
                        mEmailContent = await _message.GetMessage(Enums.ContentType.OrderFromCustomerMail);
                    }
                    catch (Exception c) { }
                    //string soaid = ShopOrderItem.FirstOrDefault().Product.Tenant.CreationUserId;
                    //var soa = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == soaid);

                    ////update content Data
                    mEmailContent = mEmailContent.Replace("|Table|", table);
                    mEmailContent = mEmailContent.Replace("|Amount|", order.AmountPaid.ToString());
                    mEmailContent = mEmailContent.Replace("|fullname|", user.Fullname);
                    mEmailContent = mEmailContent.Replace("|date|", order.DateOfOrder.ToString("dd/MM/yyyy hh:mm tt"));
                    mEmailContent = mEmailContent.Replace("|orderid|", order.OrderId);



                    AddMessageDto email = new AddMessageDto();
                    email.Content = mEmailContent;
                    email.Recipient = user.User.Email;
                    email.NotificationType = Enums.NotificationType.Email;
                    email.NotificationStatus = Enums.NotificationStatus.NotSent;
                    email.Retries = 0;
                    email.Title = "My Order";

                    var sts = await _message.AddMessage(email);
                    //
                    string imEmailContent = mEmailContent.Replace("|fullname|", "New Order from " + ShopOrderItem.FirstOrDefault().Product.Tenant.BusinessName);
                    //

                    AddMessageDto i = new AddMessageDto();
                    i.Content = imEmailContent;
                    i.Recipient = "onwukaemeka41@gmail.comAhioma";
                    i.NotificationType = Enums.NotificationType.Email;
                    i.NotificationStatus = Enums.NotificationStatus.NotSent;
                    i.Retries = 0;
                    i.Title = "My Order";
                    var stsa = await _message.AddMessage(i);

                    AddMessageDto ia = new AddMessageDto();
                    ia.Content = imEmailContent;
                    ia.Recipient = "logistics.ahioma@gmail.com";
                    ia.NotificationType = Enums.NotificationType.Email;
                    ia.NotificationStatus = Enums.NotificationStatus.NotSent;
                    ia.Retries = 0;
                    ia.Title = "My Order";
                    var aa = await _message.AddMessage(ia);

                    AddMessageDto ias = new AddMessageDto();
                    ias.Content = imEmailContent;
                    ias.Recipient = "Christopherikwuegbu9@gmail.com";
                    ias.NotificationType = Enums.NotificationType.Email;
                    ias.NotificationStatus = Enums.NotificationStatus.NotSent;
                    ias.Retries = 0;
                    ias.Title = "My Order";
                    var aams = await _message.AddMessage(ias);
                    

                     AddMessageDto iaa = new AddMessageDto();
                    iaa.Content = imEmailContent;
                    iaa.Recipient = "ahiomaad@gmail.com";
                    iaa.NotificationType = Enums.NotificationType.Email;
                    iaa.NotificationStatus = Enums.NotificationStatus.NotSent;
                    iaa.Retries = 0;
                    iaa.Title = "My Order";
                    var aaa = await _message.AddMessage(iaa);
                }
                catch (Exception cf) { }
                return "Success: checkout successful";
            }

            return "Fail: Error #655111";
            #endregion
        }


        //methods

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

        public async Task<string> ProcessOrderToLedger(long OrderId)
        {
            var color = GetRandomColor().ToString();
            var order = await _context.Orders.Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.Id == OrderId);

            IQueryable<OrderItem> OrderItem = from s in _context.OrderItems
                                                 .Include(p => p.Order)
                 .Include(x => x.Product).Include(x => x.Product.ProductPictures)
                .Include(x => x.Product.Tenant)
                .Include(x => x.Product.Tenant.TenantAddresses)
                .Where(x => x.OrderId == order.Id && x.Status != OrderStatus.OutOfStock)
                                              select s;

            string TransactionCode = "";
            try
            {
                var tcodecolor = "";

                foreach (var pi in await OrderItem.ToListAsync())
                {
                    var p = await _context.OrderItems
                        .Include(p => p.Order)
                 .Include(x => x.Product).Include(x => x.Product.ProductPictures)
                .Include(x => x.Product.Tenant)
                .Include(x => x.Product.Tenant.TenantAddresses).AsNoTracking().FirstOrDefaultAsync(x => x.Id == pi.Id);
                    if (p.Product.Name.Length > 15)
                    {
                        p.Product.Name = p.Product.Name.Substring(0, 14);
                    }

                    //generate token for tracking
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
                    TransactionCode = xNumber.Substring(1, 8).ToUpper();
                    decimal pDivision = Convert.ToDecimal(p.Product.Commision);
                    if (pDivision == 0)
                    {
                        pDivision = pDivision + 5;
                    }
                    decimal productAmountAfterCommisionAdd = pDivision / Convert.ToDecimal(100);
                    decimal productAmountAfterCommision = p.Product.Price * productAmountAfterCommisionAdd * p.Quantity;
                    var amountCommision = Convert.ToDecimal(p.Product.Price) * p.Quantity * (Convert.ToDecimal(pDivision) / Convert.ToDecimal(100));

                    //update store wallet
                    //get product
                    var shop = await _context.Tenants.Include(x => x.User).Include(x => x.UserProfile).FirstOrDefaultAsync(x => x.Id == p.Product.TenantId);
                    //get store wallet
                    var storeWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == shop.UserId);
                    var sTranscation = await _context.Transactions.FirstOrDefaultAsync(x => x.UserId == shop.UserId && x.OrderItemId == p.Id);
                    if (sTranscation == null)
                    {
                        //get commision


                        var storewallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == p.Product.Tenant.UserId);
                        var shopProductBalance = (Convert.ToDecimal(p.Product.Price) * p.Quantity) - amountCommision;
                        storewallet.Balance += shopProductBalance;
                        storewallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                        _context.Entry(storewallet).State = EntityState.Modified;

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

                        //create transaction for store
                        Transaction shopt = new Transaction();
                        shopt.Amount = shopProductBalance;
                        shopt.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                        shopt.Status = EntityStatus.Success;
                        shopt.TransactionType = TransactionTypeEnum.Credit;
                        shopt.Note = "Sales";
                        shopt.UserId = shop.UserId;
                        shopt.WalletId = storewallet.Id;
                        shopt.TrackCode = order.TrackCode;
                        shopt.OrderItemId = p.Id;
                        shopt.Color = color;
                        shopt.TransactionSection = TransactionSection.Sales;
                        shopt.PayoutStatus = PayoutStatus.Ledger;
                        shopt.Description = "SO (" + p.Product.Name + ")";
                        shopt.UserProfileId = shop.UserProfileId ?? 0;
                        _context.Transactions.Add(shopt);
                        await _context.SaveChangesAsync();
                    }

                    //get SOA
                    var soaUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == shop.CreationUserId);
                    //get soa wallet
                    var soaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == soaUser.UserId);
                    //soa commision of 25% of shop commision
                    var soaTranscation = await _context.Transactions.FirstOrDefaultAsync(x => x.UserId == soaWallet.UserId && x.OrderItemId == p.Id);
                    if (soaTranscation == null)
                    {
                        decimal soaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(25) / Convert.ToDecimal(100));
                        soaWallet.Balance += soaCommision;
                        soaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                        _context.Entry(soaWallet).State = EntityState.Modified;
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


                        //create soa transaction
                        Transaction soaT = new Transaction();

                        soaT.Amount = soaCommision;
                        soaT.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                        soaT.Status = EntityStatus.Success;
                        soaT.Note = "Commision";
                        soaT.TransactionType = TransactionTypeEnum.Credit;
                        soaT.UserId = soaUser.UserId;
                        soaT.WalletId = soaWallet.Id;
                        soaT.TrackCode = order.TrackCode;
                        soaT.PayoutStatus = PayoutStatus.Ledger;
                        soaT.OrderItemId = p.Id;
                        soaT.Color = color;
                        soaT.TransactionSection = TransactionSection.ShopCommission;
                        soaT.Description = "SOA (" + p.Product.Name + ")";
                        soaT.UserProfileId = soaUser.Id;

                        _context.Transactions.Add(soaT);
                        await _context.SaveChangesAsync();

                    }
                    //get Referral
                    if (!String.IsNullOrEmpty(soaUser.ReferralLink))
                    {


                        var ReferralUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.IdNumber == soaUser.ReferralLink);
                        //get soa wallet
                        var ReferralSoaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == ReferralUser.UserId);
                        //soa commision of 5% of referral commision
                        var rsoaTranscation = await _context.Transactions.FirstOrDefaultAsync(x => x.UserId == ReferralSoaWallet.UserId && x.OrderItemId == p.Id);
                        if (rsoaTranscation == null)
                        {
                            decimal ReferralSoaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                            ReferralSoaWallet.Balance += ReferralSoaCommision;
                            ReferralSoaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                            _context.Entry(ReferralSoaWallet).State = EntityState.Modified;
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

                            //create soa transaction
                            Transaction refsoaT = new Transaction();

                            refsoaT.Amount = ReferralSoaCommision;
                            refsoaT.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                            refsoaT.Status = EntityStatus.Success;
                            refsoaT.TransactionType = TransactionTypeEnum.Credit;
                            refsoaT.Note = "Referral Commision";
                            refsoaT.UserId = ReferralUser.UserId;
                            refsoaT.WalletId = ReferralSoaWallet.Id;
                            refsoaT.TrackCode = order.TrackCode;
                            refsoaT.PayoutStatus = PayoutStatus.Ledger;
                            refsoaT.OrderItemId = p.Id;
                            refsoaT.Color = color;
                            refsoaT.TransactionSection = TransactionSection.ReferralCommission;
                            refsoaT.Description = "SOA Upline (" + p.Product.Name + ")";
                            refsoaT.UserProfileId = ReferralUser.Id;
                            _context.Transactions.Add(refsoaT);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var AdminReferralUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == "referral@ahioma.com");
                        //get soa wallet
                        var AdminReferralSoaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == AdminReferralUser.UserId);
                        //soa commision of 5% of referral commision
                        var arsoaTranscation = await _context.Transactions.FirstOrDefaultAsync(x => x.UserId == AdminReferralSoaWallet.UserId && x.OrderItemId == p.Id);
                        if (arsoaTranscation == null)
                        {
                            decimal AdminReferralSoaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                            AdminReferralSoaWallet.Balance += AdminReferralSoaCommision;
                            AdminReferralSoaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                            _context.Entry(AdminReferralSoaWallet).State = EntityState.Modified;
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


                            //create soa transaction
                            Transaction adminrefsoaT = new Transaction();
                            adminrefsoaT.Amount = AdminReferralSoaCommision;
                            adminrefsoaT.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                            adminrefsoaT.Status = EntityStatus.Success;
                            adminrefsoaT.TransactionType = TransactionTypeEnum.Credit;
                            adminrefsoaT.Note = "Referral Commision";
                            adminrefsoaT.UserId = AdminReferralUser.UserId;
                            adminrefsoaT.WalletId = AdminReferralSoaWallet.Id;
                            adminrefsoaT.TrackCode = order.TrackCode;
                            adminrefsoaT.PayoutStatus = PayoutStatus.Ledger;
                            adminrefsoaT.OrderItemId = p.Id;
                            adminrefsoaT.Color = color;
                            adminrefsoaT.TransactionSection = TransactionSection.ReferralCommission;
                            adminrefsoaT.Description = "SOA Upline (" + p.Product.Name + ")";
                            adminrefsoaT.UserProfileId = AdminReferralUser.Id;
                            _context.Transactions.Add(adminrefsoaT);
                            await _context.SaveChangesAsync();
                        }
                        //
                    }
                    //get customer Referral
                    if (!String.IsNullOrEmpty(order.CustomerRef))
                    {


                        var ReferralCustomer = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.IdNumber == order.CustomerRef);
                        //get soa wallet
                        var ReferralCustomerWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == ReferralCustomer.UserId);
                        //soa commision of 5% of referral commision
                        var cusoaTranscation = await _context.Transactions.FirstOrDefaultAsync(x => x.UserId == ReferralCustomerWallet.UserId && x.OrderItemId == p.Id);
                        if (cusoaTranscation == null)
                        {
                            decimal ReferralCustomerCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                            ReferralCustomerWallet.Balance += ReferralCustomerCommision;
                            ReferralCustomerWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                            _context.Entry(ReferralCustomerWallet).State = EntityState.Modified;

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

                            //create soa transaction
                            Transaction crefT = new Transaction();
                            crefT.Amount = ReferralCustomerCommision;
                            crefT.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                            crefT.Status = EntityStatus.Success;
                            crefT.TransactionType = TransactionTypeEnum.Credit;
                            crefT.Note = "Referral Commision";
                            crefT.UserId = ReferralCustomer.UserId;
                            crefT.WalletId = ReferralCustomerWallet.Id;
                            crefT.TrackCode = order.TrackCode;
                            crefT.Color = color;
                            crefT.PayoutStatus = PayoutStatus.Ledger;
                            crefT.OrderItemId = p.Id;
                            crefT.TransactionSection = TransactionSection.CustomerReferralCommission;
                            crefT.Description = "Customer Referral (" + p.Product.Name + ")";
                            crefT.UserProfileId = ReferralCustomer.Id;
                            _context.Transactions.Add(crefT);
                            await _context.SaveChangesAsync();

                        }
                    }
                    else
                    {
                        var iAdminReferralUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == "customerreferral@ahioma.com");
                        //get soa wallet
                        var iAdminReferralSoaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == iAdminReferralUser.UserId);
                        //soa commision of 5% of referral commision
                        var adrTranscation = await _context.Transactions.FirstOrDefaultAsync(x => x.UserId == iAdminReferralSoaWallet.UserId && x.OrderItemId == p.Id);
                        if (adrTranscation == null)
                        {
                            decimal AdminReferralSoaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                            iAdminReferralSoaWallet.Balance += AdminReferralSoaCommision;
                            iAdminReferralSoaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                            _context.Entry(iAdminReferralSoaWallet).State = EntityState.Modified;


                            //wallet histiory
                            WalletHistory adcushistory = new WalletHistory();
                            adcushistory.Amount = AdminReferralSoaCommision;
                            adcushistory.CreationTime = DateTime.UtcNow.AddHours(1);
                            adcushistory.LedgerBalance = iAdminReferralSoaWallet.Balance;
                            adcushistory.AvailableBalance = iAdminReferralSoaWallet.WithdrawBalance;
                            adcushistory.WalletId = iAdminReferralSoaWallet.Id;
                            adcushistory.UserId = iAdminReferralSoaWallet.UserId;
                            adcushistory.UserProfileId = iAdminReferralUser.Id;
                            adcushistory.TransactionType = "Cr";
                            adcushistory.Source = "Commission " + p.Product.Name;
                            _context.WalletHistories.Add(adcushistory);

                            //create soa transaction
                            Transaction cadminT = new Transaction();

                            cadminT.Amount = AdminReferralSoaCommision;
                            cadminT.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                            cadminT.Status = EntityStatus.Success;
                            cadminT.TransactionType = TransactionTypeEnum.Credit;
                            cadminT.Note = "Customer Referral Commision";
                            cadminT.UserId = iAdminReferralUser.UserId;
                            cadminT.WalletId = iAdminReferralSoaWallet.Id;
                            cadminT.TrackCode = order.TrackCode;
                            cadminT.PayoutStatus = PayoutStatus.Ledger;
                            cadminT.OrderItemId = p.Id;
                            cadminT.Color = color;
                            cadminT.TransactionSection = TransactionSection.ReferralCommission;
                            cadminT.Description = "Customer Referral (" + p.Product.Name + ")";
                            cadminT.UserProfileId = iAdminReferralUser.Id;
                            _context.Transactions.Add(cadminT);

                            await _context.SaveChangesAsync();
                        }
                    }
                    //admin commission
                    var getadmin = await _userManager.FindByEmailAsync("jinmcever@gmail.com");
                    var admin = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == getadmin.Id);
                    //get soa wallet
                    var AdminWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == getadmin.Id);
                    //soa commision of 70% of shop commision
                    var aTranscation = await _context.Transactions.FirstOrDefaultAsync(x => x.UserId == AdminWallet.UserId && x.OrderItemId == p.Id);
                    if (aTranscation == null)
                    {
                        decimal AdminCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(65) / Convert.ToDecimal(100));
                        AdminWallet.Balance += AdminCommision;
                        AdminWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                        _context.Entry(AdminWallet).State = EntityState.Modified;
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



                        //create admin transaction
                        Transaction adminahiomaT = new Transaction();
                        adminahiomaT.Amount = AdminCommision;
                        adminahiomaT.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                        adminahiomaT.Status = EntityStatus.Success;
                        adminahiomaT.TransactionType = TransactionTypeEnum.Credit;
                        adminahiomaT.Note = "Ahioma Commision";
                        adminahiomaT.UserId = getadmin.Id;
                        adminahiomaT.WalletId = AdminWallet.Id;
                        adminahiomaT.TrackCode = order.TrackCode;
                        adminahiomaT.OrderItemId = p.Id;
                        adminahiomaT.Color = color;
                        adminahiomaT.TransactionSection = TransactionSection.Commission;
                        adminahiomaT.PayoutStatus = PayoutStatus.Ledger;
                        adminahiomaT.Description = "AHIOMA (" + p.Product.Name + ")";
                        adminahiomaT.UserProfileId = admin.Id;
                        _context.Transactions.Add(adminahiomaT);

                        //
                    }
                    await _context.SaveChangesAsync();
                }


                //logistic commission
                var Loggetadmin = await _userManager.FindByEmailAsync("ahiaexpress@ahioma.com");
                var Logadmin = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == Loggetadmin.Id);
                //get soa wallet
                var LogAdminWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == Loggetadmin.Id);
                var loTranscation = await _context.Transactions.FirstOrDefaultAsync(x => x.UserId == LogAdminWallet.UserId && x.TrackCode == order.TrackCode);
                if (loTranscation == null)
                {
                    //soa commision of 70% of shop commision
                    decimal? LogAdminCommision = order.LogisticAmount;
                    LogAdminWallet.Balance += LogAdminCommision ?? 0;
                    LogAdminWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                    _context.Entry(LogAdminWallet).State = EntityState.Modified;
                    //wallet histiory
                    WalletHistory logistichistory = new WalletHistory();
                    logistichistory.Amount = LogAdminCommision ?? 0;
                    logistichistory.CreationTime = DateTime.UtcNow.AddHours(1);
                    logistichistory.LedgerBalance = LogAdminWallet.Balance;
                    logistichistory.AvailableBalance = LogAdminWallet.WithdrawBalance;
                    logistichistory.WalletId = LogAdminWallet.Id;
                    logistichistory.UserId = LogAdminWallet.UserId;
                    logistichistory.UserProfileId = Logadmin.Id;
                    logistichistory.TransactionType = "Cr";
                    logistichistory.Source = "logistic ";
                    _context.WalletHistories.Add(logistichistory);

                    //create logistic transaction
                    Transaction logisticT = new Transaction();
                    logisticT.Amount = LogAdminCommision ?? 0;
                    logisticT.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                    logisticT.Status = EntityStatus.Success;
                    logisticT.TransactionType = TransactionTypeEnum.Credit;
                    logisticT.Note = "Logistic Fee";
                    logisticT.UserId = Loggetadmin.Id;
                    logisticT.WalletId = LogAdminWallet.Id;
                    logisticT.TrackCode = order.TrackCode;
                    logisticT.Color = color;
                    logisticT.TransactionSection = TransactionSection.Commission;
                    logisticT.PayoutStatus = PayoutStatus.Ledger;
                    logisticT.Description = "Logistics for Order " + order.OrderId;
                    logisticT.UserProfileId = Logadmin.Id;
                    _context.Transactions.Add(logisticT);

                    await _context.SaveChangesAsync();
                }

                await _context.SaveChangesAsync();

                try
                {
                    //subtract from order
                    var ahiomaaccount = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == "ahiomaorder@ahioma.com");
                    var ahiomawallet = await _walletAppService.GetWallet(ahiomaaccount.UserId);


                        //soa commision of 70% of shop commision
                        decimal? iOrderAmout = order.AmountPaid;
                    ahiomawallet.Balance -= iOrderAmout ?? 0;
                    ahiomawallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                        _context.Entry(ahiomawallet).State = EntityState.Modified;
                        //wallet histiory
                        WalletHistory ihistory = new WalletHistory();
                    ihistory.Amount = iOrderAmout ?? 0;
                    ihistory.CreationTime = DateTime.UtcNow.AddHours(1);
                    ihistory.LedgerBalance = ahiomawallet.Balance;
                    ihistory.AvailableBalance = ahiomawallet.WithdrawBalance;
                    ihistory.WalletId = ahiomawallet.Id;
                    ihistory.UserId = ahiomawallet.UserId;
                    ihistory.UserProfileId = ahiomaaccount.Id;
                    ihistory.TransactionType = "Dr";
                    ihistory.Source = "Order After Processed ";
                        _context.WalletHistories.Add(ihistory);

                        //create logistic transaction
                        Transaction itransact = new Transaction();
                    itransact.Amount = iOrderAmout ?? 0;
                    itransact.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                    itransact.Status = EntityStatus.Success;
                    itransact.TransactionType = TransactionTypeEnum.Credit;
                    itransact.Note = "Logistic Fee";
                    itransact.UserId = Loggetadmin.Id;
                    itransact.WalletId = LogAdminWallet.Id;
                    itransact.TrackCode = order.TrackCode;
                    itransact.Color = color;
                    itransact.TransactionSection = TransactionSection.Commission;
                    itransact.PayoutStatus = PayoutStatus.Ledger;
                    itransact.Description = "Order After Processed " + order.OrderId;
                    itransact.UserProfileId = ahiomaaccount.Id;
                        _context.Transactions.Add(itransact);

                    await _context.SaveChangesAsync();
                }
                catch(Exception d) { }
            }
            catch (Exception d)
            {

            }

            return "";
        }


        public async Task<string> ProcessOrderToWithdrawable(long OrderId)
        {

            var order = await _context.Orders.Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.Id == OrderId);

            IQueryable<OrderItem> OrderItem = from s in _context.OrderItems
                                                 .Include(p => p.Order)
                 .Include(x => x.Product).Include(x => x.Product.ProductPictures)
                .Include(x => x.Product.Tenant)
                .Include(x => x.Product.Tenant.TenantAddresses)
                .Where(x => x.OrderId == order.Id && x.Status != OrderStatus.OutOfStock)
                                              select s;

            string TransactionCode = "";
            try
            {
                #region ....
                var tcodecolor = "";

                foreach (var pi in await OrderItem.ToListAsync())
                {
                    var p = await _context.OrderItems
                        .Include(p => p.Order)
                 .Include(x => x.Product).Include(x => x.Product.ProductPictures)
                .Include(x => x.Product.Tenant)
                .Include(x => x.Product.Tenant.TenantAddresses).AsNoTracking().FirstOrDefaultAsync(x => x.Id == pi.Id);


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

                    var sTranscation = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == storewallet.UserId && x.OrderItemId == p.Id);
                    if (sTranscation != null)
                    {
                        if (sTranscation.PayoutStatus == PayoutStatus.Ledger)
                        {
                            storewallet.Balance -= shopProductBalance;
                            storewallet.WithdrawBalance += shopProductBalance;
                            storewallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                            _context.Entry(storewallet).State = EntityState.Modified;
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
                            storehistory.Source = "transfer from ledger to available ";
                            _context.WalletHistories.Add(storehistory);


                            sTranscation.PayoutStatus = PayoutStatus.Available;
                            _context.Entry(sTranscation).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                        }
                    }

                    //get SOA
                    var soaUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == shop.CreationUserId);
                    //get soa wallet
                    var soaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == soaUser.UserId);
                    //soa commision of 25% of shop commision
                    var soaTranscation = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == soaWallet.UserId && x.OrderItemId == p.Id);
                    if (soaTranscation != null)
                    {
                        if (soaTranscation.PayoutStatus == PayoutStatus.Ledger)
                        {
                            decimal soaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(25) / Convert.ToDecimal(100));
                            soaWallet.Balance -= soaCommision;
                            soaWallet.WithdrawBalance += soaCommision;
                            soaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                            _context.Entry(soaWallet).State = EntityState.Modified;

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
                            soahistory.Source = "transfer from ledger to available";
                            _context.WalletHistories.Add(soahistory);

                            soaTranscation.PayoutStatus = PayoutStatus.Available;
                            _context.Entry(soaTranscation).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                        }
                    }
                    //get Referral
                    if (!String.IsNullOrEmpty(soaUser.ReferralLink))
                    {


                        var ReferralUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.IdNumber == soaUser.ReferralLink);
                        //get soa wallet
                        var ReferralSoaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == ReferralUser.UserId);
                        //soa commision of 5% of referral commision
                        var srTranscation = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == ReferralSoaWallet.UserId && x.OrderItemId == p.Id);
                        if (srTranscation != null)
                        {
                            if (srTranscation.PayoutStatus == PayoutStatus.Ledger)
                            {
                                decimal ReferralSoaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                                ReferralSoaWallet.Balance -= ReferralSoaCommision;
                                ReferralSoaWallet.WithdrawBalance += ReferralSoaCommision;
                                ReferralSoaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                                _context.Entry(ReferralSoaWallet).State = EntityState.Modified;
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
                                refsoahistory.Source = "transfer from ledger to available ";
                                _context.WalletHistories.Add(refsoahistory);

                                srTranscation.PayoutStatus = PayoutStatus.Available;
                                _context.Entry(srTranscation).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                            }
                        }

                    }
                    else
                    {
                        var AdminReferralUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == "referral@ahioma.com");
                        //get soa wallet
                        var AdminReferralSoaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == AdminReferralUser.UserId);
                        //soa commision of 5% of referral commision
                        var asrTranscation = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == AdminReferralSoaWallet.UserId && x.OrderItemId == p.Id);
                        if (asrTranscation != null)
                        {
                            if (asrTranscation.PayoutStatus == PayoutStatus.Ledger)
                            {
                                decimal AdminReferralSoaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                                AdminReferralSoaWallet.Balance -= AdminReferralSoaCommision;
                                AdminReferralSoaWallet.WithdrawBalance += AdminReferralSoaCommision;
                                AdminReferralSoaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                                _context.Entry(AdminReferralSoaWallet).State = EntityState.Modified;

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
                                adrefsoahistory.Source = "transfer from ledger to available ";
                                _context.WalletHistories.Add(adrefsoahistory);

                                asrTranscation.PayoutStatus = PayoutStatus.Available;
                                _context.Entry(asrTranscation).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                            }
                        }


                        //
                    }
                    //get customer Referral
                    if (!String.IsNullOrEmpty(order.CustomerRef))
                    {


                        var ReferralCustomer = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.IdNumber == order.CustomerRef);
                        //get soa wallet
                        var ReferralCustomerWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == ReferralCustomer.UserId);
                        //soa commision of 5% of referral commision
                        var rsrTranscation = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == ReferralCustomerWallet.UserId && x.OrderItemId == p.Id);
                        if (rsrTranscation != null)
                        {
                            if (rsrTranscation.PayoutStatus == PayoutStatus.Ledger)
                            {
                                decimal ReferralCustomerCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                                ReferralCustomerWallet.Balance -= ReferralCustomerCommision;
                                ReferralCustomerWallet.WithdrawBalance += ReferralCustomerCommision;
                                ReferralCustomerWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                                _context.Entry(ReferralCustomerWallet).State = EntityState.Modified;

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
                                cushistory.Source = "transfer from ledger to available ";
                                _context.WalletHistories.Add(cushistory);

                                rsrTranscation.PayoutStatus = PayoutStatus.Available;
                                _context.Entry(rsrTranscation).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                            }
                        }

                    }
                    else
                    {
                        var iAdminReferralUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == "customerreferral@ahioma.com");
                        //get soa wallet
                        var iAdminReferralSoaWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == iAdminReferralUser.UserId);
                        //soa commision of 5% of referral commision

                        var adsrTranscation = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == iAdminReferralSoaWallet.UserId && x.OrderItemId == p.Id);
                        if (adsrTranscation != null)
                        {
                            if (adsrTranscation.PayoutStatus == PayoutStatus.Ledger)
                            {

                                decimal AdminReferralSoaCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(5) / Convert.ToDecimal(100));
                                iAdminReferralSoaWallet.Balance -= AdminReferralSoaCommision;
                                iAdminReferralSoaWallet.WithdrawBalance += AdminReferralSoaCommision;
                                iAdminReferralSoaWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                                _context.Entry(iAdminReferralSoaWallet).State = EntityState.Modified;


                                //wallet histiory
                                WalletHistory adcushistory = new WalletHistory();
                                adcushistory.Amount = AdminReferralSoaCommision;
                                adcushistory.CreationTime = DateTime.UtcNow.AddHours(1);
                                adcushistory.LedgerBalance = iAdminReferralSoaWallet.Balance;
                                adcushistory.AvailableBalance = iAdminReferralSoaWallet.WithdrawBalance;
                                adcushistory.WalletId = iAdminReferralSoaWallet.Id;
                                adcushistory.UserId = iAdminReferralSoaWallet.UserId;
                                adcushistory.UserProfileId = iAdminReferralUser.Id;
                                adcushistory.TransactionType = "Cr";
                                adcushistory.Source = "transfer from ledger to available ";
                                _context.WalletHistories.Add(adcushistory);

                                adsrTranscation.PayoutStatus = PayoutStatus.Available;
                                _context.Entry(adsrTranscation).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    //admin commission
                    var getadmin = await _userManager.FindByEmailAsync("jinmcever@gmail.com");
                    var admin = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == getadmin.Id);
                    //get soa wallet
                    var AdminWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == getadmin.Id);

                    var admTranscation = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == AdminWallet.UserId && x.OrderItemId == p.Id);
                    if (admTranscation != null)
                    {
                        if (admTranscation.PayoutStatus == PayoutStatus.Ledger)
                        {
                            //soa commision of 70% of shop commision
                            decimal AdminCommision = Convert.ToDecimal(amountCommision) * (Convert.ToDecimal(65) / Convert.ToDecimal(100));
                            AdminWallet.Balance -= AdminCommision;
                            AdminWallet.WithdrawBalance += AdminCommision;
                            AdminWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                            _context.Entry(AdminWallet).State = EntityState.Modified;
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
                            adminhistory.Source = "transfer from ledger to available ";
                            _context.WalletHistories.Add(adminhistory);

                            admTranscation.PayoutStatus = PayoutStatus.Available;
                            _context.Entry(admTranscation).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                        }
                    }
                    await _context.SaveChangesAsync();
                }
                #endregion
                #region data
                //logistic commission
                var Loggetadmin = await _userManager.FindByEmailAsync("ahiaexpress@ahioma.com");
                var Logadmin = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == Loggetadmin.Id);
                //get soa wallet
                var LogAdminWallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == Loggetadmin.Id);

                var admmTranscation = await _context.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == LogAdminWallet.UserId && x.TrackCode == order.TrackCode);
                if (admmTranscation != null)
                {
                    if (admmTranscation.PayoutStatus == PayoutStatus.Ledger)
                    {
                        //soa commision of 70% of shop commision
                        decimal? LogAdminCommision = order.LogisticAmount;
                        LogAdminWallet.Balance -= LogAdminCommision ?? 0;
                        LogAdminWallet.WithdrawBalance += LogAdminCommision ?? 0;
                        LogAdminWallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                        _context.Entry(LogAdminWallet).State = EntityState.Modified;
                        //wallet histiory
                        WalletHistory logistichistory = new WalletHistory();
                        logistichistory.Amount = LogAdminCommision ?? 0;
                        logistichistory.CreationTime = DateTime.UtcNow.AddHours(1);
                        logistichistory.LedgerBalance = LogAdminWallet.Balance;
                        logistichistory.AvailableBalance = LogAdminWallet.WithdrawBalance;
                        logistichistory.WalletId = LogAdminWallet.Id;
                        logistichistory.UserId = LogAdminWallet.UserId;
                        logistichistory.UserProfileId = Logadmin.Id;
                        logistichistory.TransactionType = "Cr";
                        logistichistory.Source = "transfer from ledger to available ";
                        _context.WalletHistories.Add(logistichistory);

                        admmTranscation.PayoutStatus = PayoutStatus.Available;
                        _context.Entry(admmTranscation).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                await _context.SaveChangesAsync();
                #endregion

                var iOrderItems = from s in _context.OrderItems
                                                       .Include(p => p.Product)
                                                       .Include(p => p.Product.Tenant)
                       .Where(x => x.OrderId == order.Id)
                                  select s;




                var orderStatus = await iOrderItems.ToListAsync();
                var tenantIds = orderStatus.Select(x => x.Product.TenantId).Distinct().ToList();


            }
            catch (Exception d)
            {

            }


            return "";
        }


    }
}
