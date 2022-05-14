using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Reconsile.Pages.Accounts
{
    [Authorize(Roles = "mSuperAdmin")]
    public class GetDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public GetDataModel(
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

        public UserProfile Profile { get; set; }
        public UserAddress UserAddress { get; set; }
        public UserProfileSocialMedia UserProfileSocialMedia { get; set; }
        public Wallet Wallet { get; set; }
        public IQueryable<WalletHistory> WalletHistory { get; set; }

        public Tenant Tenant { get; set; }
        public TenantAddress TenantAddress { get; set; }
        public TenantSetting TenantSetting { get; set; }
        public TenantSocialMedia TenantSocialMedia { get; set; }
        public IQueryable<Product> Products { get; set; }
        public IQueryable<ProductCart> ProductCarts { get; set; }
        public IQueryable<Order> Orders { get; set; }
        public IQueryable<OrderItem> OrderItems { get; set; }
        public IQueryable<AhiaPayTransfer> AhiaPayTransfers { get; set; }
        public LogisticProfile LogisticProfile { get; set; }
       

        public async Task OnGetAsync(string uid)
        {
            var user = await _userManager.FindByIdAsync(uid);
            Profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            IQueryable<UserAddress> UserAddress = from s in _context.UserAddresses                                                    
                                                     .Where(x => x.UserId ==user.Id)
                                           select s;

        }

        public async Task<IActionResult> OnPostUpdateUserStatus(string uid, int statusType)
        {
            return RedirectToPage();
        }

    }
}
