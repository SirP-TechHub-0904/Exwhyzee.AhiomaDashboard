using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Verify;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin,CustomerCare,Logistic")]

    public class PrintInPosModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IFlutterTransactionService _flutterTransactionAppService;


        public PrintInPosModel(
                UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,
               IFlutterTransactionService flutterTransactionAppService,
                SignInManager<IdentityUser> signInManager, AhiomaDbContext context
                )
        {
            _userManager = userManager;
            _context = context;
            _flutterTransactionAppService = flutterTransactionAppService;
            _roleManager = roleManager;
            _signInManager = signInManager;

        }

        public OrderItem OrderItem { get; set; }
        public Order Order { get; set; }
        public IQueryable<Transaction> Transaction { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; }
        public FlutterTransactionVerify Payment { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {

            OrderItem = await _context.OrderItems
                .Include(x => x.Order)
                .Include(x => x.Product)

                .Include(x => x.Product.Tenant)
                .Include(x => x.Product.Tenant.User)
                .Include(x => x.Product.Tenant.UserProfile)
                .Include(x => x.Product.ProductPictures)
                .Include(x => x.Product.Tenant.TenantAddresses)
                .FirstOrDefaultAsync(x => x.Id == id);


            Order = await _context.Orders
                .Include(x => x.LogisticVehicle)
                .Include(x => x.OrderItems)
                .Include(x => x.OrderItems)
                .Include(x => x.UserProfile)
                .Include(x => x.UserProfile.User)
                .Include(x => x.UserProfile.UserAddresses)
                .FirstOrDefaultAsync(x => x.Id == OrderItem.OrderId);
            try
            {
                var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == Order.TransactionId);

                Payment = await _flutterTransactionAppService.VerifyTransaction(transaction.TransactionReference);
            }
            catch (Exception i) { }
            return Page();
        }
    }
}
