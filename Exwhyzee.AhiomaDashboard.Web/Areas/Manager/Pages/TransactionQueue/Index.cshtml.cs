using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Manager.Pages.TransactionQueue
{
    [Microsoft.AspNetCore.Authorization.Authorize]


    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IFlutterTransactionService _flutterTransactionAppService;



        public IndexModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, IFlutterTransactionService flutterTransactionAppService)
        {
            _context = context;
            _flutterTransactionAppService = flutterTransactionAppService;
        }

        public IList<TransferQueue> TransactionQueues { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            TransactionQueues = await _context.TransferQueues.OrderByDescending(x => x.Date).ToListAsync();

            return Page();
           

        }


    }
}
