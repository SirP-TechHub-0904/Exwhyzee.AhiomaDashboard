using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.SOA.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SOA,mSuperAdmin,SubAdmin")]
    public class MainModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public MainModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,

            IUserProfileRepository account, AhiomaDbContext context,
            IEmailSendService emailSender)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        [BindProperty]
        public string UserId { get; set; }

        public int AllShops { get; set; }
        public int AllProducts { get; set; }
        public int PublishedProducts { get; set; }
        public int UnPublishProducts { get; set; }
        public int WithImageProducts { get; set; }
        public int WithOutImageProducts { get; set; }
        public int TotalProductsSold { get; set; }
        public int TotalProductsProcessing { get; set; }
        public int TotalProductsReversed { get; set; }
        public int TotalProductsCanceled { get; set; }

        public int MySold { get; set; }
        public int Referred { get; set; }
        public int MyProcessing { get; set; }
        public int MyReversed { get; set; }
        public int MyCanceled { get; set; }
        public int TotalOrder { get; set; }
        public decimal TotalEarnsShops { get; set; }
        public decimal TotalEarnsReferral { get; set; }
        public decimal Ledger { get; set; }
        public decimal Available { get; set; }


        public IList<Order> OrderItems { get; set; }
        public IList<Order> OrderItemsUser { get; set; }
        public IList<Tenant> Tenants { get; set; }
        public IList<Transaction> Transactions { get; set; }
        public decimal OneView { get; set; }
        public async Task OnGetAsync(string uid)
        {
            #region condition
            if (String.IsNullOrEmpty(uid))
            {
                var user = await _userManager.GetUserAsync(User);
                UserId = user.Id;
            }
            else
            {
                UserId = uid;
            }
            #endregion

            #region check
            //shops
            IQueryable<Tenant> iTenants = from s in _context.Tenants
                                          .Include(x => x.UserProfile)
                                          .Include(x => x.UserProfile.User)
                                          .Where(x => x.CreationUserId == UserId)
                                          select s;
            AllShops = await iTenants.CountAsync();
            //products
            IQueryable<Product> iProducts = from s in _context.Products
                                            .Include(x => x.Tenant)
                                         .Where(x => x.Tenant.CreationUserId == UserId)
                                            select s;
            AllProducts = await iProducts.CountAsync();
            PublishedProducts = await iProducts.Where(x => x.Published == true).CountAsync();
            UnPublishProducts = await iProducts.Where(x => x.Published == false).CountAsync();
            #endregion
            #region ordercount
            //sales count
            IQueryable<OrderItem> iOrder = from s in _context.OrderItems
                                            .Include(x => x.Product.Tenant)
                                            .Include(x => x.Product.ProductPictures)
                                         .Where(x => x.Product.Tenant.CreationUserId == UserId)
                                           select s;

            var orderids = iOrder.Select(x => x.OrderId).Distinct();
            IQueryable<Order> iOrderMain = from s in _context.Orders
                                           .Include(x => x.UserProfile)
                                           .Include(x => x.OrderItems)
                                           .Include(x => x.UserProfile.User)
                                           .Where(x => orderids.Contains(x.Id))
                                            .OrderByDescending(x => x.DateOfOrder)
                                           select s;

            TotalOrder = await iOrderMain.CountAsync();
            TotalProductsSold = await iOrderMain.Where(x => x.Status == Enums.OrderStatus.Completed).CountAsync();
            TotalProductsProcessing = await iOrderMain.Where(x => x.Status == Enums.OrderStatus.Processing).CountAsync();
            TotalProductsReversed = await iOrderMain.Where(x => x.Status == Enums.OrderStatus.Reversed).CountAsync();
            TotalProductsCanceled = await iOrderMain.Where(x => x.Status == Enums.OrderStatus.Cancel).CountAsync();
            #endregion
            //amonut earned
            IQueryable<Transaction> iEarned = from s in _context.Transactions
                                         .Where(x => x.UserId == UserId)
                                              select s;
            TotalEarnsShops = await iEarned.Where(x => x.TransactionSection == Enums.TransactionSection.ShopCommission).Select(x => x.Amount).SumAsync();
            TotalEarnsReferral = await iEarned.Where(x => x.TransactionSection == Enums.TransactionSection.ReferralCommission).Select(x => x.Amount).SumAsync();

            OrderItems = await iOrderMain.OrderByDescending(x => x.DateOfOrder).Take(10).ToListAsync();
            Tenants = await iTenants.OrderByDescending(x => x.CreationTime).Take(10).ToListAsync();
            Transactions = await iEarned.OrderByDescending(x => x.DateOfTransaction).Take(10).ToListAsync();
            OneView = await iEarned.Where(x => x.Note.Contains("1 View 1 Naira")).SumAsync(x => x.Amount);

            var iWallet = await _context.Wallets
                                        .FirstOrDefaultAsync(x => x.UserId == UserId);


            Ledger = iWallet.Balance;
            Available = iWallet.WithdrawBalance;

            IQueryable<OrderItem> iOrderUser = from s in _context.OrderItems
                                            .Include(x => x.Product.Tenant)
                                            .Include(x => x.Order)
                                            .Include(x => x.Order.UserProfile)
                                         .Where(x => x.Order.UserProfile.UserId == UserId)
                                               select s;

            var orderidsUser = iOrderUser.Select(x => x.OrderId).Distinct();
            IQueryable<Order> iOrderMainUser = from s in _context.Orders
                                           .Include(x => x.OrderItems)
                                           .Include(x => x.UserProfile)
                                           .Include(x => x.UserProfile.User)
                                           .Where(x => orderidsUser.Contains(x.Id))
                                            .OrderByDescending(x => x.DateOfOrder)
                                               select s;

            OrderItemsUser = await iOrderMainUser.OrderByDescending(x => x.DateOfOrder).Take(10).ToListAsync();

            MySold = await iOrderMainUser.Where(x => x.Status == Enums.OrderStatus.Completed).CountAsync();
            MyProcessing = await iOrderMainUser.Where(x => x.Status == Enums.OrderStatus.Processing).CountAsync();
            MyReversed = await iOrderMainUser.Where(x => x.Status == Enums.OrderStatus.Reversed).CountAsync();
            MyCanceled = await iOrderMainUser.Where(x => x.Status == Enums.OrderStatus.Cancel).CountAsync();


            var soauser = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == UserId);
            var Profile = from s in _context.UserProfiles
                                             .Where(x => x.ReferralLink == soauser.IdNumber && x.Roles.Contains("SOA"))
                          select s;

            Referred = await Profile.CountAsync();



        }



    }
}
