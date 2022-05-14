using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    [Authorize(Roles = "Admin,mSuperAdmin,Customer Care")]

    public class AllTransferModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public AllTransferModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public IList<AhiaPayTransfer> Transfer { get; set; }

        public async Task OnGetAsync()
        {
            Transfer = await _context.AhiaPayTransfers.OrderByDescending(x => x.DateOfTransfer).ToListAsync();
        }
    }
}
