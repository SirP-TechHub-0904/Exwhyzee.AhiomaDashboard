using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Store.Pages.Transactions
{
    public class IndexModel : PageModel
    {
        private readonly IWalletRepository _wallet;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITransactionRepository _transaction;
        private readonly AhiomaDbContext _context;


        public IndexModel(IWalletRepository wallet, UserManager<IdentityUser> userManager,
                ITransactionRepository transaction, AhiomaDbContext context
                )
        {
            _wallet = wallet;
            _transaction = transaction;
            _userManager = userManager;
            _context = context;
        }
        public IList<Transaction> Transactions { get; set; }

        public Wallet Wallet { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("./Info/PageNotFound", new { area = "" });
            }


            Wallet = await _wallet.GetWallet(user.Id);
            Transactions = await _transaction.GetAsyncAllByUserId(user.Id);


            IQueryable<Transaction> iTransaction = from s in _context.Transactions
                              .Where(x => x.UserId == user.Id && x.TransactionSection == Enums.TransactionSection.Sales)
                              .OrderByDescending(x => x.DateOfTransaction)
                                                   select s;
            Transactions = iTransaction.ToList();
            return Page();
        }
    }
}
