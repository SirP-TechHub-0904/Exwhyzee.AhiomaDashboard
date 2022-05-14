using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.ManageTransaction.Pages.Transactions
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]

    public class OrderDetailsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IMessageRepository _message;
        private readonly IOrderRepository _order;


        public OrderDetailsModel(
                UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,

                SignInManager<IdentityUser> signInManager, AhiomaDbContext context
, IMessageRepository message, IOrderRepository order)
        {
            _userManager = userManager;
            _context = context;

            _roleManager = roleManager;
            _signInManager = signInManager;
            _message = message;
            _order = order;
        }
        [BindProperty]
        public long OrderId { get; set; }
        [BindProperty]
        public string UserID { get; set; }
        [BindProperty]
        public Order Order { get; set; }
        public IList<OrderItem> OrderItems { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; }
        
        public async Task<IActionResult> OnGetAsync(long id)
        {
            Order = await _context.Orders
                .Include(x => x.OrderItems)
                .Include(x => x.LogisticVehicle)
                .Include(x => x.Transaction)
                .Include(x => x.UserProfile.User)
                .Include(x => x.LogisticVehicle.LogisticProfile)
                .Include(x => x.UserProfile)
                .Include(x => x.UserAddress)
                .Include(x => x.UserProfile.UserAddresses)
                .FirstOrDefaultAsync(x => x.Id == id);
           


            IQueryable<OrderItem> OrderItem = from s in _context.OrderItems
                                                 .Include(p => p.Order)
                 .Include(x => x.Product).Include(x => x.Product.ProductPictures)
                .Include(x => x.Product.Tenant)
                .Include(x => x.Product.Tenant.TenantAddresses)
                .Where(x => x.OrderId == id)
                                              select s;
            //update cost
            decimal cost = 0;
            foreach (var i in OrderItem)
            {
                cost = cost + (i.Price * i.Quantity);
            }
            var ordernewcost = await _context.Orders.FirstOrDefaultAsync(x => x.Id == Order.Id);
            ordernewcost.OrderCostAfterProcessing = cost;
            _context.Entry(ordernewcost).State = EntityState.Modified;
            
                await _context.SaveChangesAsync();
            OrderItems = OrderItem.ToList();



            var iUserName = await _context.UserProfiles.FirstOrDefaultAsync(x=>x.UserId == Order.Transaction.UserId);
            UserName = iUserName.Fullname + "(" + iUserName.IdNumber + ")";


            return Page();
        }
        
        [BindProperty]
        public long OrderItemId { get; set; }

        public async Task<IActionResult> OnPostUpdatePrice()
        {
            long id = 0;
            try
            {
                var data = await _context.OrderItems.Include(x => x.Order).Include(x=>x.Product).FirstOrDefaultAsync(x => x.Id == OrderItemId);
                //product
                

                id = data.OrderId;
               data.Price = data.Product.Price;
                _context.Entry(data).State = EntityState.Modified;

                IQueryable<OrderItem> OrderItem = from s in _context.OrderItems
                                                  .Include(p => p.Order)
                  .Where(x => x.OrderId == data.OrderId)
                                                  select s;
                //update cost
                decimal cost = 0;
                foreach (var i in OrderItem)
                {
                    cost = cost + (i.Price * i.Quantity);
                }
                var ordernewcost = await _context.Orders.FirstOrDefaultAsync(x => x.Id == data.OrderId);
                ordernewcost.OrderCostAfterProcessing = cost;
                _context.Entry(ordernewcost).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["success"] = "success updated item price";
                }
                catch (Exception c)
                {
                    TempData["error"] = "failed to update item price";

                }
                return RedirectToPage("OrderDetails", new { id = data.OrderId });
            }
            catch (Exception c)
            {
                return RedirectToPage("OrderDetails", new { id = id });

            }
        }

        public async Task<IActionResult> OnPostOutOfStock()
        {
            long id = 0;
            try
            {
                
                var data = await _context.OrderItems.Include(x => x.Order).Include(x=>x.Product).FirstOrDefaultAsync(x => x.Id == OrderItemId);
                id = data.OrderId;

                if (data.Status != Enums.OrderStatus.OutOfStock)
                {

                    data.Status = Enums.OrderStatus.OutOfStock;
                    data.Price = 0;
                }
                else
                {
                    var data1 = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);

                    decimal? orderamt = data1.AmountPaid - data1.LogisticAmount;
                    decimal? diff = (orderamt + (data1.AmountMovedToCustomer + data1.AmountMovedToAdmin + data1.AdditionalPayment)) - data1.OrderCostAfterProcessing;
                

    if (data.Product.Price > diff)
                    {
                        TempData["error"] = "Insufficient fund add fund";
                        return RedirectToPage("OrderDetails", new { id = id });
                    }
                    data.Status = Enums.OrderStatus.Processing;
                    data.Price = data.Product.Price;
                }
                _context.Entry(data).State = EntityState.Modified;

                IQueryable<OrderItem> OrderItem = from s in _context.OrderItems
                                                  .Include(p => p.Order)
                  .Where(x => x.OrderId == data.OrderId)
                                                  select s;
                //update cost
                decimal cost = 0;
                foreach(var i in OrderItem)
                {
                    cost = cost + (i.Price * i.Quantity);
                }
                var ordernewcost = await _context.Orders.FirstOrDefaultAsync(x => x.Id == data.OrderId);
                ordernewcost.OrderCostAfterProcessing = cost;
                _context.Entry(ordernewcost).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["success"] = "success updated item out of stock";
                }
                catch (Exception c)
                {
                    TempData["error"] = "failed to update item out of stock";

                }
                return RedirectToPage("OrderDetails", new { id = data.OrderId });
            }catch(Exception c)
            {
                return RedirectToPage("OrderDetails", new { id = id });

            }
        }

        [BindProperty]
        public string GetUserId { get; set; }
        [BindProperty]
        public long GetOrderId { get; set; }
        [BindProperty]
        public long Amount { get; set; }

        [BindProperty]
        public bool IncludeLogistic { get; set; }


        public async Task<IActionResult> OnPostMoveToCustomer()
        {
             
            try
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

                //
                var data = await _context.Orders.FirstOrDefaultAsync(x => x.Id == GetOrderId);
                decimal? orderamt = data.AmountPaid - data.LogisticAmount;
                decimal? diff = (orderamt + (data.AmountMovedToCustomer + data.AmountMovedToAdmin + data.AdditionalPayment)) - data.OrderCostAfterProcessing;
                if (Amount > diff)
                {
                    TempData["error"] = "Insufficient fund";
                    return RedirectToPage("OrderDetails", new { id = GetOrderId });
                }
                //get customer wallet
                var customeraccount = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == GetUserId);
                var customerwallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == customeraccount.UserId);
                //transaction and wallet
                
                
               

                decimal totalMoney = 0;
                if (IncludeLogistic == true)
                {
                    data.AmountMovedToCustomer = data.AmountMovedToCustomer + Amount + data.LogisticAmount ?? 0;
                    data.LogisticAmount = 0;
                    totalMoney = (Amount + data.LogisticAmount ?? 0);
                }
                else
                {
                    data.AmountMovedToCustomer = data.AmountMovedToCustomer + Amount;
                    totalMoney = Amount;
                }
                Transaction ahiomaTransaction = new Transaction();
                ahiomaTransaction.Amount = totalMoney;
                ahiomaTransaction.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                ahiomaTransaction.TransactionType = TransactionTypeEnum.Credit;
                ahiomaTransaction.Note = "Refund";
                ahiomaTransaction.UserId = customerwallet.UserId;
                ahiomaTransaction.WalletId = customerwallet.Id;
                ahiomaTransaction.TrackCode = TransactionCode;
                ahiomaTransaction.TransactionReference = TransactionCode;
                ahiomaTransaction.TransactionSection = TransactionSection.Reverse;
                ahiomaTransaction.PayoutStatus = PayoutStatus.Ledger;
                ahiomaTransaction.Description = "Balance After Processing Order " + GetOrderId;
                ahiomaTransaction.Status = Enums.EntityStatus.Success;
                ahiomaTransaction.UserProfileId = customeraccount.Id;
                _context.Transactions.Add(ahiomaTransaction);
                _context.Entry(data).State = EntityState.Modified;
                //update wallet
               
                customerwallet.WithdrawBalance += totalMoney;
                customerwallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                _context.Entry(customerwallet).State = EntityState.Modified;

                //update history
                WalletHistory a = new WalletHistory();
                a.Amount = ahiomaTransaction.Amount;
                a.CreationTime = DateTime.UtcNow.AddHours(1);
                a.LedgerBalance = customerwallet.Balance;
                a.AvailableBalance = customerwallet.WithdrawBalance + ahiomaTransaction.Amount;
                a.WalletId = customerwallet.Id;
                a.UserId = customerwallet.UserId;
                a.UserProfileId = customerwallet.Id;
                a.TransactionType = "Cr";
                if (IncludeLogistic == true)
                {
                    a.Source = "Balance After Processing Order " + GetOrderId +" and Logistic Money";
                }
                else
                {
                    a.Source = "Balance After Processing Order " + GetOrderId;

                }
                _context.WalletHistories.Add(a);

               

                await _context.SaveChangesAsync();
                //
                    string mSmsContent = "";
                    try
                    {
                        mSmsContent = await _message.GetMessage(Enums.ContentType.AhiaPayTransferAfterProcessingOrderSms);
                    }
                    catch (Exception c) { }

                    //update content Data
                    mSmsContent = mSmsContent.Replace("|Amount|", ahiomaTransaction.Amount.ToString());
                string mmm = "";
                if (IncludeLogistic == true)
                {
                    mmm = "Balance After Processing Order " + GetOrderId + " and Logistic Money";
                }
                else
                {
                   mmm = "Balance After Processing Order " + GetOrderId;

                }
                mSmsContent = mSmsContent.Replace("|Desc|", mmm);
                    mSmsContent = mSmsContent.Replace("|Date|", ahiomaTransaction.DateOfTransaction.ToString("dd/MM/yyyy hh:mm tt"));
                    mSmsContent = mSmsContent.Replace("|Balance|", customerwallet.WithdrawBalance.ToString());
                    mSmsContent = mSmsContent.Replace("|||", "\r\n");
                    //sms
                    AddMessageDto sms = new AddMessageDto();
                    sms.Content = mSmsContent;
                    sms.Recipient = customeraccount.User.PhoneNumber.Replace(" ", "");
                    sms.NotificationType = Enums.NotificationType.SMS;
                    sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                    sms.Retries = 0;
                
                sms.Title = mmm;
                    //
                    var stss = await _message.AddMessage(sms);

                    ////email
                    string mEmailContent = "";
                    try
                    {
                        mEmailContent = await _message.GetMessage(Enums.ContentType.AhiaPayTransferAfterProcessingOrderEmail);
                    }
                    catch (Exception c) { }


                    ////update content Data
                    mEmailContent = mEmailContent.Replace("|Amount|", ahiomaTransaction.Amount.ToString());
                    mEmailContent = mEmailContent.Replace("|Ref|", ahiomaTransaction.TransactionReference);
                    mEmailContent = mEmailContent.Replace("|Date|", ahiomaTransaction.DateOfTransaction.ToString("dd/MM/yyyy hh:mm tt"));
                    mEmailContent = mEmailContent.Replace("|Balance|", customerwallet.WithdrawBalance.ToString());
                    mEmailContent = mEmailContent.Replace("|Surname|", customeraccount.Surname);
                    mEmailContent = mEmailContent.Replace("|Description|", mmm);


                    AddMessageDto email = new AddMessageDto();
                    email.Content = mEmailContent;
                    email.Recipient = customeraccount.User.Email.Replace(" ", "");
                    email.NotificationType = Enums.NotificationType.Email;
                    email.NotificationStatus = Enums.NotificationStatus.NotSent;
                    email.Retries = 0;
                    email.Title = mmm;

                    var sts = await _message.AddMessage(email);
                   

                TempData["success"] = "success transfered "+Amount +" to customer";
            }
            catch (Exception c)
            {
                TempData["error"] = "failed to transfered " + Amount + " to customer";
            }
            
                return RedirectToPage("OrderDetails", new { id = GetOrderId });
           
        }


        public async Task<IActionResult> OnPostMoveToAdmin()
        {

            try
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

                //
                var data = await _context.Orders.FirstOrDefaultAsync(x => x.Id == GetOrderId);
                decimal? orderamt = data.AmountPaid - data.LogisticAmount;
                decimal? diff = (orderamt + (data.AmountMovedToCustomer + data.AmountMovedToAdmin + data.AdditionalPayment)) - data.OrderCostAfterProcessing;
                if (Amount > diff)
                {
                    TempData["error"] = "Insufficient fund";
                    return RedirectToPage("OrderDetails", new { id = GetOrderId });
                }
                //get customer wallet
                var ahiomaaccount = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == "ahiomaorder@ahioma.com");
                var ahiomawallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == ahiomaaccount.UserId);
                //transaction and wallet
                Transaction ahiomaTransaction = new Transaction();
                ahiomaTransaction.Amount = Amount;
                ahiomaTransaction.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                ahiomaTransaction.TransactionType = TransactionTypeEnum.Credit;
                ahiomaTransaction.Note = "New Order ";
                ahiomaTransaction.UserId = ahiomawallet.UserId;
                ahiomaTransaction.WalletId = ahiomawallet.Id;
                ahiomaTransaction.TrackCode = TransactionCode;
                ahiomaTransaction.TransactionReference = TransactionCode;
                ahiomaTransaction.TransactionSection = TransactionSection.Sales;
                ahiomaTransaction.PayoutStatus = PayoutStatus.Available;
                ahiomaTransaction.Description = "Balance After Processing Order " + GetOrderId;
                ahiomaTransaction.Status = Enums.EntityStatus.Success;
                ahiomaTransaction.UserProfileId = ahiomaaccount.Id;
                _context.Transactions.Add(ahiomaTransaction);

                decimal totalMoney = 0;

                    data.AmountMovedToAdmin = data.AmountMovedToAdmin + Amount;
                    totalMoney = Amount;
                
                _context.Entry(data).State = EntityState.Modified;
                //update wallet

                ahiomawallet.WithdrawBalance += totalMoney;
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
                a.UserProfileId = ahiomawallet.Id;
                a.TransactionType = "Cr";
                if (IncludeLogistic == true)
                {
                    a.Source = "Balance After Processing Order " + GetOrderId + " and Logistic Money";
                }
                else
                {
                    a.Source = "Balance After Processing Order " + GetOrderId;

                }
                _context.WalletHistories.Add(a);



                await _context.SaveChangesAsync();
                //
                


                TempData["success"] = "success transfered " + Amount + " to admin";
            }
            catch (Exception c)
            {
                TempData["error"] = "failed to transfered " + Amount + " to admin";
            }

            return RedirectToPage("OrderDetails", new { id = GetOrderId });

        }


        [BindProperty]
        public decimal FundAmount { get; set; }
        [BindProperty]
        public long GetId { get; set; }
        [BindProperty]
        public string CustomerName { get; set; }
        public async Task<IActionResult> OnPostAddFund()
        {

            try
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

                var ahiomaaccount = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == "addfund@ahioma.com");
                var ahiomawallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == ahiomaaccount.UserId);

                //
                if (FundAmount > ahiomawallet.WithdrawBalance)
                {
                    TempData["error"] = "Insufficient fund";
                    return RedirectToPage("OrderDetails", new { id = GetId });
                }
                //get customer wallet
                 //transaction and wallet
                Transaction ahiomaTransaction = new Transaction();
                ahiomaTransaction.Amount = FundAmount;
                ahiomaTransaction.DateOfTransaction = DateTime.UtcNow.AddHours(1);
                ahiomaTransaction.TransactionType = TransactionTypeEnum.Debit;
                ahiomaTransaction.Note = "Add Fund for Order ";
                ahiomaTransaction.UserId = ahiomawallet.UserId;
                ahiomaTransaction.WalletId = ahiomawallet.Id;
                ahiomaTransaction.TrackCode = TransactionCode;
                ahiomaTransaction.TransactionReference = TransactionCode;
                ahiomaTransaction.TransactionSection = TransactionSection.Order;
                ahiomaTransaction.PayoutStatus = PayoutStatus.Available;
                ahiomaTransaction.Description = "Add Fund to " + CustomerName;
                ahiomaTransaction.Status = Enums.EntityStatus.Success;
                ahiomaTransaction.UserProfileId = ahiomaaccount.Id;
                _context.Transactions.Add(ahiomaTransaction);

                var data = await _context.Orders.FirstOrDefaultAsync(x => x.Id == GetId);


                data.AdditionalPayment = data.AdditionalPayment + FundAmount;
                
                _context.Entry(data).State = EntityState.Modified;
                //update wallet

                ahiomawallet.WithdrawBalance -= FundAmount;
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
                a.UserProfileId = ahiomawallet.Id;
                a.TransactionType = "Cr";
               
                    a.Source = "fund added for " + GetId;

                _context.WalletHistories.Add(a);



                await _context.SaveChangesAsync();
                //



                TempData["success"] = "success added fund " + FundAmount + " to order";
            }
            catch (Exception c)
            {
                TempData["error"] = "failed to add fund " + FundAmount + " to order";
            }

            return RedirectToPage("OrderDetails", new { id = GetId });

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
