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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class TrackOrderModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IFlutterTransactionService _flutterTransactionAppService;


        public TrackOrderModel(
                UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,
               IFlutterTransactionService flutterTransactionAppService,
                SignInManager<IdentityUser> signInManager,AhiomaDbContext context
                )
        {
            _userManager = userManager;
            _context = context;
            _flutterTransactionAppService = flutterTransactionAppService;
             _roleManager = roleManager;
            _signInManager = signInManager;

        }
       
        public IList<TrackOrder> TrackOrder { get; set; }
        public OrderItem OrderItem { get; set; }
      
        public async Task<IActionResult> OnGetAsync(long id)
        {
            OrderItem = await _context.OrderItems
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.Id == id);



            IQueryable<TrackOrder> iTrackOrder = from s in _context.TrackOrders
                                               .Include(p => p.OrderItem)
                                               .Include(p => p.OrderItem.Order)
                                               .OrderBy(x => x.Date)
               .Where(x => x.OrderItemId == id)
                                            select s;

            TrackOrder = await iTrackOrder.ToListAsync();
            var g = TrackOrder.Count();
            return Page();
        }
    }
}
