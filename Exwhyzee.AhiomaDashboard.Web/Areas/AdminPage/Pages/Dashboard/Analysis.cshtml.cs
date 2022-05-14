using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Balance;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.AdminPage.Pages.Dashboard
{
  
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SubAdmin")]

    public class AnalysisModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;
        private readonly IFlutterTransactionService _flutterTransactionAppService;
        private readonly IHostingEnvironment _hostingEnv;

        public AnalysisModel(
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

            //transaction


            //customer


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
