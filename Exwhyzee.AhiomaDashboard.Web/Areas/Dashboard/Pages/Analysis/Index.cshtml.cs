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

    public class IndexModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;
        public IndexModel(UserManager<IdentityUser> userManager, AhiomaDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public int Shops { get; set; }
        public int Users { get; set; }
        public int SOA { get; set; }
        public int Customer { get; set; }
        public int xLogistics { get; set; }
        public int pendorder { get; set; }
        public int revorder { get; set; }
        public int canorder { get; set; }
        public int comorder { get; set; }
        public int transac { get; set; }
        public int productf { get; set; }
        public int mkt { get; set; }
        public int categ { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            TempData["name"] = profile.Fullname;

            IQueryable<Tenant> shops = from s in _context.Tenants                                
                                          select s;
            Shops = shops.Count();

            IQueryable<IdentityUser> users = from s in _context.Users
                                       select s;
            Users = users.Count();
            IQueryable<UserProfile> soa = from s in _context.UserProfiles
                                  .Include(x => x.User).Where(x => x.User.Email != "jinmcever@gmail.com")
                                  .Where(x => x.Roles.Contains("SOA")).OrderByDescending(x => x.DateRegistered)
                                               select s;
            SOA = soa.Count();

            IQueryable<UserProfile> customer = from s in _context.UserProfiles
                                  .Include(x => x.User).Where(x => x.User.Email != "jinmcever@gmail.com").Where(x => x.Roles == "Customer").OrderByDescending(x => x.DateRegistered)
                                               select s;
            Customer = customer.Count();


            IQueryable<LogisticProfile> ilogistic = from s in _context.LogisticProfiles
                                                                                 select s;
            xLogistics = ilogistic.Count();
            IQueryable<Order> iOrders = from s in _context.Orders
                                               
                                        select s;

            pendorder = iOrders.Where(x => x.Status == Enums.OrderStatus.Pending).Count();
            comorder = iOrders.Where(x => x.Status == Enums.OrderStatus.Completed).Count();
            canorder = iOrders.Where(x => x.Status == Enums.OrderStatus.Cancel).Count();
            revorder = iOrders.Where(x => x.Status == Enums.OrderStatus.Reversed).Count();


            IQueryable<Transaction> iTransaction = from s in _context.Transactions
                                                    .Include(x => x.UserProfile)
                                           .OrderByDescending(x => x.DateOfTransaction).Where(x => x.TransactionShowEnum != Enums.TransactionShowEnum.Off)
                                                   select s;

            IQueryable<Product> product = from s in _context.Products
                                             select s;
            productf = product.Count();
            IQueryable<Market> market = from s in _context.Markets
                                          select s;
            mkt = market.Count();
            IQueryable<Category> category = from s in _context.Categories
                                        select s;
            categ = category.Count();
            transac = iTransaction.Count();
        }

    }
}
