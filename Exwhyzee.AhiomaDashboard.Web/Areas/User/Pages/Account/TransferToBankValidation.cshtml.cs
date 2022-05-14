using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class TransferToBankValidationModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IUserProfileRepository _profile;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFlutterTransactionService _flutterTransactionAppService;

        public TransferToBankValidationModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, IUserProfileRepository profile,
            IFlutterTransactionService flutterTransactionAppService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _profile = profile;
            _userManager = userManager;
            _flutterTransactionAppService = flutterTransactionAppService;
        }
        public Wallet Wallet { get; set; }
        [BindProperty]
        public decimal Amount { get; set; }
       

        public async Task<IActionResult> OnGetAsync()
        {
            var data = TempData["amt"] as string;

            if (data == null)
            {
                return RedirectToPage("./Invalid");
            }

            var result = JsonSerializer.Deserialize<decimal>(data);
            TempData["comfirmamt"] = JsonSerializer.Serialize(result);
            return RedirectToPage("./ComfirmTransfer");
           // return Page();
        }

    }

}
