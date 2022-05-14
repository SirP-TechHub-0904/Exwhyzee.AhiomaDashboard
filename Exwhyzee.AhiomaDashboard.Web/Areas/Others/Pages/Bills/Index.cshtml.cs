using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Bill;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Others.Pages.Bills
{
    public class IndexModel : PageModel
    {
        private readonly IFlutterTransactionService _flutterTransactionAppService;

        public IndexModel(IFlutterTransactionService flutterTransactionAppService)
        {
            _flutterTransactionAppService = flutterTransactionAppService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            RequestBill m = new RequestBill();
            m.customer = "08165680904";
            m.amount = 100;
            m.type = "AIRTIME";
               m.reference = "peter";

            var banknamell = await _flutterTransactionAppService.CreateBill(m);
            var bankname = await _flutterTransactionAppService.GetBills();
            return Page();
        }
    }
}
