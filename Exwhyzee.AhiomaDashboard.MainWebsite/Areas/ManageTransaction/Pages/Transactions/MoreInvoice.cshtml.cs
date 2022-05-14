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

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.ManageTransaction.Pages.Transactions
{
    
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin,CustomerCare,Logistic")]

    public class MoreInvoiceModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IFlutterTransactionService _flutterTransactionAppService;


        public MoreInvoiceModel(
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

        public IQueryable<OrderItem> OrderItem { get; set; }
        public IList<OrderItem> OrderItems { get; set; }
        public IList<OrderItem> ReversedOrderItems { get; set; }
        public long Order { get; set; }
        public IQueryable<Transaction> Transaction { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; }
        public FlutterTransactionVerify Payment { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {


            Order = id;
            return Page();
        }
    }
}
