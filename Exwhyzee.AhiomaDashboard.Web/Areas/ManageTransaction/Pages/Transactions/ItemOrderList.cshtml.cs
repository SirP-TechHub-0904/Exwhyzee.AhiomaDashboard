using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]

    public class ItemOrderListModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
       


        public ItemOrderListModel(
                UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,
               
                SignInManager<IdentityUser> signInManager,AhiomaDbContext context
                )
        {
            _userManager = userManager;
            _context = context;
           
            _roleManager = roleManager;
            _signInManager = signInManager;

        }
       
        public OrderItem OrderItem { get; set; }
        public IQueryable<Transaction> Transaction { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; }
        
        public async Task<IActionResult> OnGetAsync(long id)
        {

            OrderItem = await _context.OrderItems.Include(x => x.Order).Include(x => x.Product).Include(x => x.Product.ProductPictures)
                .Include(x => x.Product.Tenant)
                .Include(x => x.Product.Tenant.User)
                .Include(x => x.Product.Tenant.UserProfile)
                .Include(x => x.Product.Tenant.TenantAddresses)
                .FirstOrDefaultAsync(x => x.Id == id);



            IQueryable<Transaction> itransaction = from s in _context.Transactions
               .OrderBy(x => x.DateOfTransaction)
               .Where(x => x.OrderItemId == id)
                                                   select s;
            Transaction = itransaction.AsQueryable();
            return Page();
        }
    }
}
