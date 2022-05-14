
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Balance;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Dashboard.Pages.Analysis
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin")]

    public class MainModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;
        private readonly IFlutterTransactionService _flutterTransactionAppService;
        private readonly IHostingEnvironment _hostingEnv;

        public MainModel(
                UserManager<IdentityUser> userManager,
                IUserProfileRepository account,
                IFlutterTransactionService flutterTransactionAppService, AhiomaDbContext context, IHostingEnvironment hostingEnv)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _flutterTransactionAppService = flutterTransactionAppService;
            _hostingEnv = hostingEnv;

        }

        public int PendingOrder { get; set; }
        public int AwaitingConfirmation { get; set; }
        public int DeliveredOrder { get; set; }
        public int CanceledOrder { get; set; }
        public int TotalOrder { get; set; }
        //
        //transaction
        public int AhiaPayCredit { get; set; }
        public int AhiaPayDebit { get; set; }
        public int OnlineTransactionCredit { get; set; }
        public int OrderTransactionDebit { get; set; }

        //
        //customer
        public int ActiveCustomer { get; set; }
        public int NonActiveCustomer { get; set; }

        //soa
        public int ActiveSOA { get; set; }
        public int NonActiveSOA { get; set; }
        public int SOAWithNoShop { get; set; }
        public int SOAWithNoProduct { get; set; }
        public int SOATotal { get; set; }

        //so
        public int SOWithNoPrice { get; set; }

        public int ActiveSO { get; set; }
        public int NonActiveSO { get; set; }
        public int TotalSO { get; set; }
        public int SOWithNoProduct { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        //wallet
        public Balance Balance { get; set; }
        public decimal TLedgerBalance { get; set; }
        public decimal TWithdrawableBalance { get; set; }

        public decimal FTLedgerBalance { get; set; }
        public decimal FTWithdrawableBalance { get; set; }

        public decimal AhiomaLedgerBalance { get; set; }
        public decimal AhiomaWithdrawableBalance { get; set; }

        //product 
        //so
        public int TotalProduct { get; set; }
        public int LowInStock { get; set; }
        public int NoPrice { get; set; }
        public int NoImage { get; set; }
        //public IQueryable<OrderItem> TopTenOrderedItem { get; set; }
        //public IQueryable<Transaction> TopTenTransaction { get; set; }
        //public IQueryable<WalletDto> MostUsedWallet { get; set; }
        //public IQueryable<Product> MostViewedProduct { get; set; }

        public string Durationoflist { get; set; }
        public async Task OnGetAsync(DateTime? DateOne, DateTime? DateTwo, string Duration = "All")
        {
            Durationoflist = Duration;
            var user = await _userManager.GetUserAsync(User);
            #region date
            if (DateOne != null)
            {
                DateOne = DateOne.Value.Date;
            }
            else
            {
                DateOne = DateTime.UtcNow.AddHours(1).Date;
            }
            if (DateTwo != null)
            {
                DateTwo = DateTwo.Value.Date;
            }
            else
            {
                DateTwo = DateTime.UtcNow.AddHours(1).Date;
            }
            StartDate = DateOne.Value;
            EndDate = DateTwo.Value;

            #endregion
            //order
            try
            {
                IQueryable<Order> orderitems = from s in _context.Orders
                                               select s;
                if (Duration != "All")
                {
                    IQueryable<Order> iOrderitem = from s in _context.Orders
                                                  .Where(x => x.DateOfOrder.Date >= DateOne).Where(x => x.DateOfOrder.Date <= DateTwo)
                                                   select s;

                    orderitems = iOrderitem;

                }

                PendingOrder = orderitems.Where(x => x.Status == Enums.OrderStatus.Pending).Count();
                AwaitingConfirmation = orderitems.Where(x => x.Status == Enums.OrderStatus.Processing).Count();
                DeliveredOrder = orderitems.Where(x => x.Status == Enums.OrderStatus.Completed).Count();
                CanceledOrder = orderitems.Where(x => x.Status == Enums.OrderStatus.Cancel).Count();
                TotalOrder = orderitems.Count();
            }
            catch (Exception c)
            {
                PendingOrder = 0;
                AwaitingConfirmation = 0;
                DeliveredOrder = 0;
                CanceledOrder = 0;
            }
            //transaction

            try
            {
                var transactions = await _context.Transactions.ToListAsync();

                if (Duration != "All")
                {
                    IQueryable<Transaction> transaction = from s in _context.Transactions
                                                           .Where(x => x.DateOfTransaction.Date >= DateOne).Where(x => x.DateOfTransaction.Date <= DateTwo)
                                                          select s;
                    //  transactions = await itransaction.ToListAsync();

                }
                AhiaPayCredit = transactions.Where(x => x.TransactionType == Enums.TransactionTypeEnum.TransferCredit).Count();
                AhiaPayDebit = transactions.Where(x => x.TransactionType == Enums.TransactionTypeEnum.TransferDebit).Count();
                OnlineTransactionCredit = transactions.Where(x => x.TransactionType == Enums.TransactionTypeEnum.Credit).Count();
                OrderTransactionDebit = transactions.Where(x => x.TransactionType == Enums.TransactionTypeEnum.Debit).Count();
            }
            catch (Exception c)
            {
                AhiaPayCredit = 0;
                AhiaPayDebit = 0;
                OnlineTransactionCredit = 0;
                OrderTransactionDebit = 0;
            }

            //customer
            try
            {
                var Customer = await _account.GetAsyncAllByRole("Customer");
                var AllCustomer = Customer;

                if (Duration != "All")
                {
                    AllCustomer = AllCustomer.Where(x => x.DateRegistered.Date >= DateOne).Where(x => x.DateRegistered.Date <= DateTwo);

                }

                ActiveCustomer = AllCustomer.Where(x => x.Status == Enums.AccountStatus.Active).Count();
                NonActiveCustomer = AllCustomer.Where(x => x.Status == Enums.AccountStatus.Pending).Count();
            }
            catch (Exception c)
            {
                ActiveCustomer = 0;
                NonActiveCustomer = 0;
            }

            //soa
            try
            {
                var soa = await _account.GetAsyncAllByRole("SOA");

                var Allsoa = soa;

                if (Duration != "All")
                {
                    Allsoa = Allsoa.Where(x => x.DateRegistered.Date >= DateOne).Where(x => x.DateRegistered.Date <= DateTwo);

                }


                ActiveSOA = Allsoa.Where(x => x.Status == Enums.AccountStatus.Active).Count();
                NonActiveSOA = Allsoa.Where(x => x.Status != Enums.AccountStatus.Active).Count();
                SOATotal = Allsoa.Count();
            }
            catch (Exception c)
            {
                ActiveSOA = 0;
                NonActiveSOA = 0;
                SOAWithNoShop = 0;
                SOAWithNoProduct = 0;
            }
            //soa
            try
            {
                var shop = await _account.GetAsyncAllByRole("Store");
                var Allso = shop.AsQueryable();

                if (Duration != "All")
                {
                    Allso = Allso.Where(x => x.DateRegistered.Date >= DateOne).Where(x => x.DateRegistered.Date <= DateTwo);

                }

                ActiveSO = Allso.Where(x => x.Status == Enums.AccountStatus.Active).Count();
                NonActiveSO = Allso.Where(x => x.Status != Enums.AccountStatus.Active).Count();
                TotalSO = shop.Count();
            }
            catch (Exception c)
            {
                ActiveSO = 0;
                NonActiveSO = 0;
                TotalSO = 0;
                SOWithNoProduct = 000;
            }

            try
            {
                Balance = await _flutterTransactionAppService.GetBalance();
                var result = Balance.data.FirstOrDefault(x => x.currency == "NGN");
                FTLedgerBalance = result.ledger_balance;
                FTWithdrawableBalance = result.available_balance;

                var wallets = await _context.Wallets.ToListAsync();
                var allwallets = wallets.Select(x => new WalletDto
                {
                    WithdrawBalance = x.WithdrawBalance,
                    LedgerBalance = x.Balance

                }).ToList();
                TLedgerBalance = allwallets.Sum(x => x.LedgerBalance);
                TWithdrawableBalance = allwallets.Sum(x => x.WithdrawBalance);
            }
            catch (Exception d)
            {

            }
            IQueryable<Product> iproduct = from s in _context.Products
                                           select s;

            try
            {

                var product = iproduct;
                var Allp = product;

                if (Duration != "All")
                {
                    Allp = Allp.Where(x => x.CreatedOnUtc.Date >= DateOne).Where(x => x.CreatedOnUtc.Date <= DateTwo);

                }
                TotalProduct = Allp.Count();
                LowInStock = Allp.Where(x => x.Quantity < 10).Count();
                NoPrice = Allp.Where(x => x.Price == 0).Count();
                NoImage = Allp.Select(x => new ProductDto
                {
                    Id = x.Id,

                    ImageThumbnail = ValidImage(x.Id)
                }).Where(x => x.ImageThumbnail == "noimage").Count();

            }
            catch (Exception d)
            {

            }

            //try
            //{
            //    TopTenOrderedItem = _context.OrderItems.OrderByDescending(x => x.DateOfOrder).Take(10);
            //}
            //catch (Exception d) { }

            //try
            //{
            //    TopTenTransaction = _context.Transactions.OrderByDescending(x => x.DateOfTransaction).Take(10);
            //}
            //catch (Exception d) { }

            try
            {

                IQueryable<Wallet> wallets = from s in _context.Wallets
                                             select s;
                var allwallets = wallets.Select(x => new WalletDto
                {
                    UserId = x.UserId,
                    DateUpdated = x.LastUpdateTime,
                    Fullname = NameWallet(x.UserId),
                    IdNumber = IDNameWallet(x.UserId),
                    WithdrawBalance = x.WithdrawBalance,
                    LedgerBalance = x.Balance

                });
                //      MostUsedWallet = allwallets.Where(x => x.IdNumber != "0000001").Where(x => x.DateUpdated.Date >= DateTime.UtcNow.AddDays(-7).Date).Where(x => x.DateUpdated.Date <= DateTime.UtcNow.Date).Take(10);

                AhiomaLedgerBalance = allwallets.FirstOrDefault(x => x.IdNumber == "0000001").LedgerBalance;
                AhiomaWithdrawableBalance = allwallets.FirstOrDefault(x => x.IdNumber == "0000001").WithdrawBalance;
            }
            catch (Exception d) { }

            //try
            //{
            //    MostViewedProduct = iproduct.Include(x=>x.Tenant).Include(x=>x.ProductPictures).OrderByDescending(x => x.Viewed).Take(10);

            //}
            //catch (Exception d) { }


        }

        private string NameWallet(string id)
        {
            try
            {
                var userprofile = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
                return userprofile.Fullname;
            }
            catch (Exception k)
            {
                return "";
            }
        }
        private string IDNameWallet(string id)
        {
            try
            {
                var userprofile = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
                return userprofile.IdNumber;
            }
            catch (Exception k)
            {
                return "";
            }
        }
        public string ValidImage(long id)
        {
            string imgpath = "noimage";
            try
            {
                var pic = _context.ProductPictures.Where(x => x.ProductId == id).ToList();
                foreach (var i in pic)
                {

                    string webRootPath = _hostingEnv.WebRootPath;
                    var fullPaththum = webRootPath + i.PictureUrlThumbnail;
                    if (System.IO.File.Exists(fullPaththum))
                    {
                        imgpath = i.PictureUrlThumbnail;
                        break;
                    }
                }
                return imgpath;
            }
            catch (Exception c)
            {
                return imgpath;
            }
        }

    }
}
