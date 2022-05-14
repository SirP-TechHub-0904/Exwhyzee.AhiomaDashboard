using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServiceStack;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    [Authorize]
    public class CheckoutPaymentModel : PageModel
    {
        private readonly IMessageRepository _message;

        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWalletRepository _walletAppService;
        private readonly IUserProfileRepository _profile;
        private readonly IUserLogging _log;
        private readonly ITransactionRepository _transactionAppService;
        private readonly IFlutterTransactionService _flutterTransactionAppService;

        private readonly IConfiguration _config;

        public CheckoutPaymentModel(AhiomaDbContext context, UserManager<IdentityUser> userManager,
            ITransactionRepository transactionAppService,
            IUserLogging log,
          IFlutterTransactionService flutterTransactionAppService,
          IUserProfileRepository profile, IWalletRepository walletAppService,
          IConfiguration configuration
, IMessageRepository message
            )
        {
            _context = context;
            _userManager = userManager;
            _transactionAppService = transactionAppService;
            _flutterTransactionAppService = flutterTransactionAppService;
            _log = log;
            _profile = profile;
            _walletAppService = walletAppService;
            _config = configuration;
            _message = message;
        }
        [BindProperty]
        public UserProfile UserProfile { get; set; }
        [BindProperty]
        public UserAddress UserAddress { get; set; }
        public Wallet Wallet { get; set; }
        public string Deliverymethod { get; set; }
        [BindProperty]
        public IList<ProductCart> ProductCartsBUser { get; set; }

        [BindProperty]
        public string PaymentMethod { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }

        [BindProperty]
        public long UserAddId { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef)
        {
            CustomerRef = customerRef;
            var user = await _userManager.GetUserAsync(User);
            UserProfile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (UserProfile.DisableBuy == true)
            {
                TempData["error"] = "You have been Suspended from buying";
                return RedirectToPage("/Account/Index", new { Area="User" });

            }

            UserAddress = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Default == true);

            if (UserAddress == null)
            {
                var data = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserProfileId == UserProfile.Id);

                data.Default = true;

                _context.Entry(data).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            ProductCartsBUser = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == UserProfile.Id).ToListAsync();

            Deliverymethod = ProductCartsBUser.Select(x => x.DeliveryMethod).FirstOrDefault();
            Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var UserProfile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
            var cart = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == UserProfile.Id && x.CartStatus == Enums.CartStatus.Active && x.TrackOrderId != null).ToListAsync();

            //
            var iUserProfile = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == UserAddId);



            decimal sum = 0;
            decimal delivery = 0;
            // var ProductCarts = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == profile.Id).ToListAsync();

            foreach (var p in cart)
            {
                p.PaymentMethod = PaymentMethod;
                _context.Attach(p).State = EntityState.Modified;

            }

            await _context.SaveChangesAsync();

            var ItemDeliveryAndPayment = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).FirstOrDefaultAsync(x => x.UserProfileId == UserProfile.Id && x.CartStatus == Enums.CartStatus.Active && x.TrackOrderId != null);

            foreach (var p in cart)
            {
                var eachtotal = p.Quantity * p.Product.Price;
                sum = sum + eachtotal;
            }
            TempData["sum"] = sum.ToString();
            var addr = UserProfile.UserAddresses.FirstOrDefault(x => x.Default == true && x.State.ToUpper().Contains("IMO"));

            var settings = await _context.Settings.FirstOrDefaultAsync();
            if (settings.ActivateFreeDelivery == true && addr != null)
            {
              
                delivery = 0;

            }
            else
            {
                if (cart.Count() < 5)
                {

                    delivery = 1000;
                }
                else if (cart.Count() < 5 && cart.Count() > 9)
                {
                    delivery = 1500;
                }
                else
                {
                    delivery = 2000;
                }
            }
            //generate token for tracking

            string OrderCode = TCode();

            string totalsum = (sum + delivery).ToString();
            long OrderId = 0;

            var delcheckCartCompare = await _context.Orders.Include(x => x.OrderItems).AsNoTracking().FirstOrDefaultAsync(x => x.TrackOrderId == cart.FirstNonDefault().TrackOrderId && x.Status == OrderStatus.Pending);
            if (delcheckCartCompare != null)
            {
                foreach (var del in delcheckCartCompare.OrderItems)
                {

                    var data = await _context.OrderItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == del.Id);
                    _context.OrderItems.Remove(data);
                    try
                    {
                        var data2 = await _context.TrackOrders.AsNoTracking().FirstOrDefaultAsync(x => x.OrderItemId == del.Id);
                        _context.TrackOrders.Remove(data2);
                    }
                    catch (Exception d)
                    {

                    }
                    await _context.SaveChangesAsync();
                }

            }

            var checkCartCompare = await _context.Orders.Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.TrackOrderId == cart.FirstOrDefault().TrackOrderId && x.Status == OrderStatus.Pending);
            if (checkCartCompare == null)
            {
                string cartsessionid = HttpContext.Session.GetString("TrackOrderId");
                Order neworder = new Order();
                neworder.OrderAmount = Convert.ToDecimal(sum);
                neworder.OrderCostAfterProcessing = Convert.ToDecimal(sum);
                neworder.ReferenceId = OrderCode;
                neworder.UserProfileId = UserProfile.Id;
                neworder.TrackCode = OrderCode;


                neworder.UserAddressId = iUserProfile.Id;
                neworder.DateOfOrder = DateTime.UtcNow.AddHours(1);
                neworder.Vat = 0;
                neworder.DeliveryMethod = ItemDeliveryAndPayment.DeliveryMethod;
                neworder.PaymentMethod = ItemDeliveryAndPayment.PaymentMethod;
                neworder.LogisticAmount = delivery;
                neworder.Status = OrderStatus.Pending;
                neworder.TrackOrderId = cartsessionid;
                neworder.CustomerRef = CustomerRef;
                _context.Orders.Add(neworder);
                await _context.SaveChangesAsync();
                //add items
                OrderId = neworder.Id;
                foreach (var p in cart)
                {
                    OrderItem item = new OrderItem();
                    item.ProductId = p.ProductId;
                    item.Price = p.Product.Price;
                    item.OrderId = neworder.Id;
                    item.ReferenceId = neworder.ReferenceId;
                    item.Status = OrderStatus.Pending;
                    item.ShopStatus = ShopStatus.Pending;
                    item.DateOfOrder = DateTime.UtcNow.AddHours(1);
                    item.LogisticStatus = LogisticStatus.Pending;
                    item.CustomerStatus = CustomerStatus.Pending;
                    item.Quantity = p.Quantity;
                    item.CustomerRef = CustomerRef;
                    item.ItemSize = p.ItemSize;
                    item.Itemcolor = p.Itemcolor;
                    item.DeliveryMethod = p.DeliveryMethod;
                    item.TrackCode = OrderCode;
                    item.PaymentMethod = p.PaymentMethod;
                    _context.OrderItems.Add(item);

                    TrackOrder iod = new TrackOrder();

                }
            }
            else
            {
                ////update

                checkCartCompare.OrderAmount = sum;
                checkCartCompare.OrderCostAfterProcessing = sum;
                checkCartCompare.ReferenceId = OrderCode;
                checkCartCompare.UserProfileId = UserProfile.Id;
                
                checkCartCompare.UserAddressId = UserProfile.UserAddresses.FirstOrDefault(x => x.Default == true).Id;
                checkCartCompare.DateOfOrder = DateTime.UtcNow.AddHours(1);
                checkCartCompare.Vat = 0;
                checkCartCompare.LogisticAmount = delivery;
                checkCartCompare.Status = OrderStatus.Pending;
                checkCartCompare.TrackCode = OrderCode;
                checkCartCompare.CustomerRef = CustomerRef;
                checkCartCompare.DeliveryMethod = ItemDeliveryAndPayment.DeliveryMethod;
                checkCartCompare.PaymentMethod = ItemDeliveryAndPayment.PaymentMethod;
                _context.Attach(checkCartCompare).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                //add items

                await _context.SaveChangesAsync();
                foreach (var p in cart)
                {

                    OrderItem item = new OrderItem();
                    item.ProductId = p.ProductId;
                    item.Price = p.Product.Price;
                    item.OrderId = checkCartCompare.Id;
                    item.ReferenceId = checkCartCompare.ReferenceId;
                    item.Status = OrderStatus.Processing;
                    item.ShopStatus = ShopStatus.Pending;
                    item.DateOfOrder = DateTime.UtcNow.AddHours(1);
                    item.LogisticStatus = LogisticStatus.Pending;
                    item.CustomerStatus = CustomerStatus.Pending;
                    item.CustomerRef = CustomerRef;
                    item.Quantity = p.Quantity;
                    item.ItemSize = p.ItemSize;
                    item.Itemcolor = p.Itemcolor;
                    item.DeliveryMethod = p.DeliveryMethod;
                    item.PaymentMethod = p.PaymentMethod;
                    item.TrackCode = OrderCode;
                    _context.OrderItems.Add(item);

                }
                OrderId = checkCartCompare.Id;
            }

            await _context.SaveChangesAsync();

            if (PaymentMethod == "Pay with AhiaPay")
            {
                var Walletbalanceamount = await _walletAppService.GetWallet(user.Id);
                if (Convert.ToDecimal(totalsum) > Walletbalanceamount.WithdrawBalance)
                {
                    TempData["errorcart"] = "Insufficient fund in AhiaPay";

                    UserProfile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
                    UserAddress = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    ProductCartsBUser = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == UserProfile.Id).ToListAsync();

                    Deliverymethod = ProductCartsBUser.Select(x => x.DeliveryMethod).FirstOrDefault();
                    Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    return Page();

                }
                else
                {
                    string ahiapaystatus = "";
                    string transac_id = "";
                    try
                    {
                        var wallet = await _walletAppService.GetWallet(user.Id);
                        Transaction transaction = new Transaction();

                        transaction.Amount = Convert.ToDecimal(totalsum);
                        transaction.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                        transaction.Status = EntityStatus.Success;
                        transaction.TransactionType = TransactionTypeEnum.Debit;
                        transaction.UserId = user.Id;
                        transaction.WalletId = wallet.Id;
                        transaction.TrackCode = OrderCode;
                        transaction.TransactionReference = wallet.UserId.Replace("-", "") + DateTime.UtcNow.ToString("ddmmyyyyhhmmss");
                        transaction.Description = "Ahiapay Order Transaction";
                        transaction.TransactionSection = TransactionSection.Transaction;
                        transaction.UserProfileId = UserProfile.Id;
                        _context.Transactions.Add(transaction);

                        //update order with transaction id
                        var updateorder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == OrderId);

                        updateorder.OrderId = updateorder.Id.ToString("0000000");
                        _context.Attach(updateorder).State = EntityState.Modified;


                        string amountInnaira = totalsum;


                        wallet.WithdrawBalance -= Convert.ToDecimal(totalsum);
                        wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                        _context.Attach(wallet).State = EntityState.Modified;

                        //wallet histiory
                        WalletHistory history = new WalletHistory();
                        history.Amount = Convert.ToDecimal(totalsum);
                        history.CreationTime = DateTime.UtcNow.AddHours(1);
                        history.LedgerBalance = wallet.Balance;
                        history.AvailableBalance = wallet.WithdrawBalance;
                        history.WalletId = wallet.Id;
                        history.UserId = wallet.UserId;
                        history.UserProfileId = UserProfile.Id;
                        history.TransactionType = "Dr";
                        history.Source = "Online Order for Order Id (" + updateorder.OrderId + ")";
                        _context.WalletHistories.Add(history);
                       
                        await _context.SaveChangesAsync();
                        AddMessageDto sms = new AddMessageDto();
                        sms.Content = "Pending ahiapay Order for Order Id (" + updateorder.OrderId + ") by " + UserProfile.IdNumber +" "+ UserProfile.Fullname;
                        sms.Recipient = "onwukaemeka41@gmail.comAhioma";
                        sms.NotificationType = Enums.NotificationType.Email;
                        sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                        sms.Retries = 0;
                        sms.Title = "Pending ahiapay Order";
                        //
                        var stss = await _message.AddMessage(sms);
                        ahiapaystatus = "success";
                        transac_id = transaction.Id.ToString();


                    }
                    catch (Exception d)
                    {
                        ahiapaystatus = "fail";
                    }
                    //ahiapaystatus = "success";
                    //transac_id = transaction.Id.ToString();
                    //var callbackUrl = Url.Page(
                    //   "/Account/CallBackLink",
                    //   pageHandler: null,
                    //   values: new { area = "User" },
                    //   protocol: Request.Scheme);

                    var callbackUrl = Url.Page(
                      "/Account/XyzAhiomaCallLink",
                      pageHandler: null,
                      values: new { area = "User" },
                      protocol: Request.Scheme);

                    string return_url = callbackUrl + "?source=checkout&oid=" + OrderId.ToString() + "&ahiapaystatus=" + ahiapaystatus + "&customerRef=" + CustomerRef + "&transac_id=" + transac_id;

                    //string return_url = "https://localhost:44357/User/Account/CallBackLink?source=checkout&oid=" + OrderId.ToString() + "&ahiapaystatus=" + ahiapaystatus + "&customerRef=" + CustomerRef + "&transac_id=" + transac_id;
                    return Redirect(return_url);
                }
            }
            else if (PaymentMethod == "Pay with Card")
            {
                var wallet = await _walletAppService.GetWallet(user.Id);
                Transaction transaction = new Transaction();

                transaction.Amount = Convert.ToDecimal(totalsum);
                transaction.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                transaction.Status = EntityStatus.Pending;
                transaction.TransactionType = TransactionTypeEnum.Credit;
                transaction.UserId = user.Id;
                transaction.WalletId = wallet.Id;
                transaction.TrackCode = OrderCode;
                transaction.Description = "Customer Total Payment";
                transaction.TransactionSection = TransactionSection.Transaction;
                transaction.UserProfileId = UserProfile.Id;
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                //update order with transaction id
                var updateorder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == OrderId);
                updateorder.TransactionId = transaction.Id;
                updateorder.OrderId = updateorder.Id.ToString("0000000");
                _context.Attach(updateorder).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                AddMessageDto sms = new AddMessageDto();
                sms.Content = "Pending Online Order for Order Id (" + updateorder.OrderId + ") by " + UserProfile.IdNumber +" "+ UserProfile.Fullname;
                sms.Recipient = "GENERAL@GENERAL.GENERAL";
                sms.NotificationType = Enums.NotificationType.Email;
                sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                sms.Retries = 0;
                sms.Title = "Pending Online Order";
                //
                var stss = await _message.AddMessage(sms);
                string amountInnaira = totalsum;

                //var callbackUrl2 = Url.Page(
                //      "/Account/CallBackLink",
                //      pageHandler: null,
                //      values: new { area = "User" },
                //      protocol: Request.Scheme);

                var callbackUrl2 = Url.Page(
                      "/Account/XyzAhiomaCallLink",
                      pageHandler: null,
                      values: new { area = "User" },
                      protocol: Request.Scheme);

                string return_url = callbackUrl2 + "?source=checkout&oid=" + OrderId.ToString() + "&customerRef=" + CustomerRef;


                string currency = "NGN";
                string tx_id = transaction.Id.ToString();
                string pay_option = "card";
                int consumer_id = (int)UserProfile.Id;
                string consumer_mc = GetMACAddress();
                string email = user.Email;
                string phone = user.PhoneNumber;
                string name = UserProfile.Fullname;
                string title = "AHIOMA";
                string des = "Ahoma desc";
                string logo = "https://ahioma.com/images/NEW-VF.png";

                var response = await _flutterTransactionAppService.InitializeTransaction(tx_id, amountInnaira, currency, return_url,
                    pay_option, consumer_id, consumer_mc, email, phone, name, title, des, logo, "web");
                try
                {
                    var urllink = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                    var mainurllink = urllink.AbsoluteUri;

                    var Userlog = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    var lognew = await _log.LogData(user.UserName, "", mainurllink);
                    Userlog.Logs = Userlog.Logs + "<br/>" + lognew;
                    _context.Attach(Userlog).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                }
                catch (Exception s)
                {

                }
                if (response.status == "success")
                {
                    return Redirect(response.data.link);
                }

            }

            return RedirectToPage();
        }

        private string TCode()
        {
            // generate token for tracking
            string date1 = DateTime.UtcNow.AddHours(1).ToString("ssfff");

            //  The random number sequence
            Random num = new Random();

            // Create new string from the reordered char array
            string rand = new string(date1.ToCharArray().
                            OrderBy(s => (num.Next(2) % 2) == 0).ToArray());

            var code = Token(5);

            var xxx = date1 + "8d5dd1c19c994edd9d3a07cbcd4540eaa043bc711f5f4513981d8dcee224b2d6";
            string TokenTracker = xxx;
            string xNumber = new string(TokenTracker.ToCharArray().
                            OrderBy(s => (num.Next(2) % 2) == 0).ToArray());
            string OrderCode = xNumber.Substring(1, 10).ToUpper();
            return OrderCode;
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
                String += Chars[Random.Next(0, 19)];
            };

            return (String);
        }

        public string GetMACAddress()
        {
            string mac_src = "";
            string macAddress = "";

            foreach (System.Net.NetworkInformation.NetworkInterface nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                {
                    mac_src += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            while (mac_src.Length < 12)
            {
                mac_src = mac_src.Insert(0, "0");
            }

            for (int i = 0; i < 11; i++)
            {
                if (0 == (i % 2))
                {
                    if (i == 10)
                    {
                        macAddress = macAddress.Insert(macAddress.Length, mac_src.Substring(i, 2));
                    }
                    else
                    {
                        macAddress = macAddress.Insert(macAddress.Length, mac_src.Substring(i, 2)) + "-";
                    }
                }
            }
            return macAddress;
        }

    }
}
