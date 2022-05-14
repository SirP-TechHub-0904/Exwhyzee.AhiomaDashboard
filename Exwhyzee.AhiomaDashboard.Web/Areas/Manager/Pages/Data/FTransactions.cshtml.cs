using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Flutter.TransactionGetAsync;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Manager.Pages.Data
{
    [Authorize(Roles = "MainAdmin,mSuperAdmin")]

    public class FTransactionsModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly IFlutterTransactionService _flutterTransactionAppService;


        public FTransactionsModel(

                IFlutterTransactionService flutterTransactionAppService, AhiomaDbContext context)
        {

            _context = context;
            _flutterTransactionAppService = flutterTransactionAppService;

        }

        public GetAllTransaction Transactions { get; set; }
        public async Task<IActionResult> OnGetAsync(string from, string to, string customerEmail, string status, string tx_ref, string customerName, int page = 0)
        {
            if (page == 0)
            {
                page = 1;
            }

            Transactions = await _flutterTransactionAppService.GetAllTransactions(from, to, page, customerEmail, status, tx_ref, customerName);

            return Page();

        }
    }
}
