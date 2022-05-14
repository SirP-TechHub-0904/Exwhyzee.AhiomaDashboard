using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UtilityBill;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Bills.Pages.Payment
{
     [Microsoft.AspNetCore.Authorization.Authorize]

    public class AirtimeModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBillRepository _billAppService;
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IWalletRepository _walletAppService;
        private readonly IHostingEnvironment _hostingEnv;

        public AirtimeModel(IBillRepository billAppService, EntityFramework.Data.AhiomaDbContext context, UserManager<IdentityUser> userManager, IHostingEnvironment hostingEnv, IWalletRepository walletAppService)
        {
            _billAppService = billAppService;
            _context = context;
            _userManager = userManager;
            _hostingEnv = hostingEnv;
            _walletAppService = walletAppService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public ParameterModel Parameter { get; set; }

        public class ParameterModel
        {
           
            public string Network { get; set; }
            public decimal Amount { get; set; }

            public string PhoneNumber { get; set; }
        }
        public UserProfile UserProfile { get; set; }
        public decimal Balance { get; set; }
        public async Task OnGet(string returnUrl = null)
        {
           string LoggedInUser = _userManager.GetUserId(HttpContext.User);
            var userbalance = await _walletAppService.GetWallet(LoggedInUser);
            Balance = userbalance.WithdrawBalance;
            UserProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == LoggedInUser);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {

                return RedirectToPage("Index");
            }

            return Page();

        }


    }
}
