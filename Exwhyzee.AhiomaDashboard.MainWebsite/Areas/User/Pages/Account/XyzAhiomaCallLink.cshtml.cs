using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    public class XyzAhiomaCallLinkModel : PageModel
    {
        private readonly IOrderRepository _order;

        public XyzAhiomaCallLinkModel(IOrderRepository order)
        {
            _order = order;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // tx_ref = ref &transaction_id = 30490 & status = successful

            // = checkouttransaction_id
            var source = HttpContext.Request.Query["source"].ToString();
            var tranxRef = HttpContext.Request.Query["tx_ref"].ToString();
            var transaction_id = HttpContext.Request.Query["transaction_id"].ToString();
            var status = HttpContext.Request.Query["status"].ToString();
            var customerRef = HttpContext.Request.Query["customerRef"].ToString();
            var orderid = HttpContext.Request.Query["oid"].ToString();
            var ahiapaystatus = HttpContext.Request.Query["ahiapaystatus"].ToString();
            var Ahia_transac_Id = HttpContext.Request.Query["transac_id"].ToString();
            var transactiontype = HttpContext.Request.Query["transactiontype"].ToString();
            var from = HttpContext.Request.Query["from"].ToString();

            var result = await _order.Insert(source, tranxRef, transaction_id, 
                status, customerRef, orderid, ahiapaystatus, Ahia_transac_Id, transactiontype, from, "");

            if (result.Contains("Ok Deposit"))
            {
                TempData["success"] = "Online Deposit Successful";
                return RedirectToPage("./Index");
            }
            else if (result.Contains("Fail Deposit"))
            {
                TempData["error"] = "Online Deposit Failed. Try Again";
                return RedirectToPage("./Index");
            }
            else if(result.Contains("Success: checkout successful"))
            {
                TempData["success"] = "Order Placed";
                return RedirectToPage("./MyOrders");
            }
            return Page();

        }
    }
}
