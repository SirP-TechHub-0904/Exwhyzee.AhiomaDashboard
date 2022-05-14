using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Store.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Store,mSuperAdmin,SubAdmin")]

    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public IndexModel(
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

        public int AllProducts { get; set; }
        public int ProductWithOutPrice { get; set; }
        public int ProductWithSmallQuantity { get; set; }

        public int TotalProductsSold { get; set; }
        public int TotalProductsProcessing { get; set; }
        public int TotalProductsReversed { get; set; }
        public int TotalProductsCanceled { get; set; }

        public decimal MyEarnings { get; set; }

        public UserProfile MySOA { get; set; }
        public int HighestProductSold { get; set; }

        public int MyProcessing { get; set; }
        public int MyReversed { get; set; }
        public int MyCanceled { get; set; }
        public int TotalOrderCompleted { get; set; }

        public decimal Ledger { get; set; }
        public decimal Available { get; set; }


        public IList<Order> OrderItems { get; set; }
        public IList<Order> OrderItemsUser { get; set; }
        public IList<Transaction> Transactions { get; set; }

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

            //products
            IQueryable<Product> iProducts = from s in _context.Products
                                            .Include(x => x.Tenant)
                                         .Where(x => x.Tenant.UserId == UserId)
                                            select s;
            AllProducts = await iProducts.CountAsync();
            ProductWithOutPrice = await iProducts.Where(x => x.Price == 0).CountAsync();
            ProductWithSmallQuantity = await iProducts.Where(x => x.Quantity < 5).CountAsync();
            #endregion
            #region ordercount
            //sales count
            IQueryable<OrderItem> iOrder = from s in _context.OrderItems
                                            .Include(x => x.Product.Tenant)
                                            .Include(x => x.Product.ProductPictures)
                                         .Where(x => x.Product.Tenant.UserId == UserId)
                                           select s;



            var highestCountSaldProduct = iOrder
                .GroupBy(n => n.Product)
            .Select(n => new
            {
                Id = n.Key,
                Count = n.Count()
            }
            )
            .OrderBy(n => n.Count);

            var q = from x in _context.OrderItems
                                            .Include(x => x.Product.Tenant)
                                            .Include(x => x.Product.ProductPictures)
                                         .Where(x => x.Product.Tenant.UserId == UserId)
                    group x by x into g
                    let count = g.Count()
                    orderby count descending
                    select new { Value = g.Key, Count = count };

            //foreach (var x in q)
            //{
            //    Console.WriteLine("Value: " + x.Value + " Count: " + x.Count);
            //}
            //var output = highestCountSaldProduct.Count;
            //var outputo = highestCountSaldProduct.Id;

            var orderids = iOrder.Select(x => x.OrderId).Distinct();
            IQueryable<Order> iOrderMain = from s in _context.Orders
                                           .Include(x => x.UserProfile)
                                           .Include(x => x.OrderItems)
                                           .Include(x => x.UserProfile.User)
                                           .Where(x => orderids.Contains(x.Id))
                                            .OrderByDescending(x => x.DateOfOrder)
                                           select s;
            OrderItems = await iOrderMain.OrderByDescending(x => x.DateOfOrder).Take(10).ToListAsync();

            //TotalOrder = await iOrderMain.CountAsync();
            TotalProductsSold = await iOrderMain.Where(x => x.Status == Enums.OrderStatus.Completed).CountAsync();
            TotalProductsProcessing = await iOrderMain.Where(x => x.Status == Enums.OrderStatus.Processing).CountAsync();
            TotalProductsReversed = await iOrderMain.Where(x => x.Status == Enums.OrderStatus.Reversed).CountAsync();
            TotalProductsCanceled = await iOrderMain.Where(x => x.Status == Enums.OrderStatus.Cancel).CountAsync();


            #endregion
            //amonut earned
            IQueryable<Transaction> iEarned = from s in _context.Transactions
                                         .Where(x => x.UserId == UserId)
                                              select s;
            MyEarnings = await iEarned.Where(x => x.TransactionSection == Enums.TransactionSection.Sales).Select(x => x.Amount).SumAsync();
            var itenant = await _context.Tenants.FirstOrDefaultAsync(x => x.UserId == UserId);
            MySOA = (from e in _context.UserProfiles
                     .Include(x => x.User)
                     where e.UserId == itenant.CreationUserId
                     select e).SingleOrDefault();


            Transactions = await iEarned.OrderByDescending(x => x.DateOfTransaction).Take(10).ToListAsync();

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

            TotalOrderCompleted = await iOrderMainUser.Where(x => x.Status == Enums.OrderStatus.Completed).CountAsync();
            MyProcessing = await iOrderMainUser.Where(x => x.Status == Enums.OrderStatus.Processing).CountAsync();
            MyReversed = await iOrderMainUser.Where(x => x.Status == Enums.OrderStatus.Reversed).CountAsync();
            MyCanceled = await iOrderMainUser.Where(x => x.Status == Enums.OrderStatus.Cancel).CountAsync();






        }



    }
}
