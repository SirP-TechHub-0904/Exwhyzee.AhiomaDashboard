using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class IndexModel : PageModel
    {
        private readonly IWalletRepository _wallet;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITransactionRepository _transaction;
        private readonly IUserProfileRepository _profile;
        private readonly AhiomaDbContext _context;


        public IndexModel(IWalletRepository wallet, UserManager<IdentityUser> userManager,
            ITransactionRepository transaction, AhiomaDbContext context, IUserProfileRepository profile
            )
        {
            _wallet = wallet;
            _transaction = transaction;
            _userManager = userManager;
            _context = context;
            _profile = profile;
        }
        public IList<Transaction> Transactions { get; set; }

        public Wallet Wallet { get; set; }
        public UserProfile Profile { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("./Info/PageNotFound", new { area = "" });
            }
          

            Wallet = await _wallet.GetWallet(user.Id);
            Transactions = await _transaction.GetAsyncAllByUserId(user.Id);
            Transactions = Transactions.OrderByDescending(x=>x.DateOfTransaction).ToList();
            Profile = await _profile.GetByUserId(user.Id);
            return Page();
        }
    }
}
