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


    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IFlutterTransactionService _flutterTransactionAppService;



        public DetailsModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, IFlutterTransactionService flutterTransactionAppService)
        {
            _context = context;
            _flutterTransactionAppService = flutterTransactionAppService;
        }

        public TransferQueue Que { get; set; }
        public UserProfile Profile { get; set; }
        [BindProperty]
        public long PTID { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {

            Que = await _context.TransferQueues.FirstOrDefaultAsync(x => x.Id == id);

            Profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == Que.uid);
            return Page();


        }
        public async Task<IActionResult> OnPostApprove()

        {

            var result = await _context.TransferQueues.FirstOrDefaultAsync(x => x.Id == PTID);
            try
            {
                var response = await _flutterTransactionAppService.MajorTransfer(result.Id);

                if (!response.Contains("Not"))
                {
                    TempData["success"] = response;
                    var c = await _context.TransferQueues.FirstOrDefaultAsync(x => x.Id == PTID);
                    c.QueueStatus = Enums.QueueStatus.Success;
                    c.response = response;
                    _context.Entry(c).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    return RedirectToPage("./Details", new { id = PTID });
                }
                else
                {
                    TempData["error"] = response;
                }

            }
            catch (Exception h) { }
            return RedirectToPage("./Index");


        }


        public async Task<IActionResult> OnPostCancel()

        {
            var c = await _context.TransferQueues.FirstOrDefaultAsync(x => x.Id == PTID);
            c.QueueStatus = Enums.QueueStatus.Cancel;
            _context.Entry(c).State = EntityState.Modified;

            var t = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == c.TransactionId);
            t.Status = Enums.EntityStatus.Failed;
            _context.Entry(t).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = PTID });
        }
    }
}
