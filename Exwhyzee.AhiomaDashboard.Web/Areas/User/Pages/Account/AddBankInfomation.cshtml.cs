using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class AddBankInfomationModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IUserProfileRepository _profile;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFlutterTransactionService _flutterTransactionAppService;

        public AddBankInfomationModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, IUserProfileRepository profile,
            IFlutterTransactionService flutterTransactionAppService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _profile = profile;
            _userManager = userManager;
            _flutterTransactionAppService = flutterTransactionAppService;
        }
        public List<SelectListItem> BankListing { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var list = await _flutterTransactionAppService.GetBanks();
            
            BankListing = list.data.Select(a =>
                                new SelectListItem
                                {
                                    Value = a.code,
                                    Text = a.name
                                }).ToList();
            return Page();
        }

        [BindProperty]
        public string Bank { get; set; }
        [BindProperty]
        public string BankAccountName { get; set; }
        [BindProperty]
        public string BankNumber { get; set; }

      
        public long ProfileId { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
        
            //                <partial name="_StatusMessage" for="StatusMessage" />

            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
            //var user = await _userManager.GetUserAsync(User);

            //var userProfile = await _profile.GetByUserId(user.Id);
            //userProfile.BankName = BankName;
            //userProfile.AccountName = BankAccountName;
            //userProfile.AccountNumber = BankNumber;

            //await _profile.Update(userProfile);
            return RedirectToPage("./ValidateBankInfo", new { bank = Bank, number = BankNumber });
        }
    }
}
