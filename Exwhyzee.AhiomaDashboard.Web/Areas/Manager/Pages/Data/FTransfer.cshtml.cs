using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Flutter.TransferGetAsync;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Manager.Pages.Data
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "MainAdmin,mSuperAdmin")]

    public class FTransferModel : PageModel
    {
    
            private readonly AhiomaDbContext _context;
            private readonly IFlutterTransactionService _flutterTransactionAppService;


            public FTransferModel(

                    IFlutterTransactionService flutterTransactionAppService, AhiomaDbContext context)
            {

                _context = context;
                _flutterTransactionAppService = flutterTransactionAppService;

            }

            public FetchTransfer FetchTransfer { get; set; }
            public async Task<IActionResult> OnGetAsync(int page = 0, string status = null)
            {
                if (page == 0)
                {
                    page = 1;
                }
                if (status == "failed")
                {
                    status = "failed";
                }
                else if (status == "successful")
                {
                    status = "successful";
                }
                else
                {
                    status = "";
                }
                var dataoutput = await _flutterTransactionAppService.GetAllTransfer(page.ToString(), status);
            FetchTransfer = dataoutput;
                return Page();

            }
        }
    
}
