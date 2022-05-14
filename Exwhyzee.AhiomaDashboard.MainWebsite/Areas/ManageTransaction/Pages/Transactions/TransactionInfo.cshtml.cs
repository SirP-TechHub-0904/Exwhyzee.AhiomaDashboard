using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.ManageTransaction.Pages.Transactions
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]

    public class TransactionInfoModel : PageModel
    {
        private readonly AhiomaDbContext _context;

        public TransactionInfoModel(AhiomaDbContext context
            )
        {
            _context = context;

        }
        public IList<Transaction> Transactions { get; set; }
        public Order Order { get; set; }


        public async Task<IActionResult> OnGetAsync(string id, long? oid)
        {
            if(oid == null)
            {
                return RedirectToPage("./PageNotFound");
            }
            //Order = await _context.Orders.Include(x => x.UserProfile).Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == oid);
            //Transactions = await _context.Transactions.Where(x => x.TrackCode == id).OrderByDescending(x => x.DateOfTransaction).ToListAsync();
            return Page();
        }
    }
}
