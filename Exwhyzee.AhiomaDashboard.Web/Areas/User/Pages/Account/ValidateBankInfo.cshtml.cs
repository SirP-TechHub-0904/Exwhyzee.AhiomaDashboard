using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class ValidateBankInfoModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IUserProfileRepository _profile;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFlutterTransactionService _flutterTransactionAppService;
        private readonly IEmailSendService _emailSender;


        public ValidateBankInfoModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, IUserProfileRepository profile,
            IFlutterTransactionService flutterTransactionAppService, UserManager<IdentityUser> userManager, IEmailSendService emailSender)
        {
            _context = context;
            _profile = profile;
            _userManager = userManager;
            _flutterTransactionAppService = flutterTransactionAppService;
            _emailSender = emailSender;
        }
        [BindProperty]
        public string BankName { get; set; }
        [BindProperty]
        public string Account { get; set; }
        [BindProperty]
        public string Name { get; set; }
        public async Task<IActionResult> OnGetAsync(string bank, string number)
        {
            var checkaccount = await _flutterTransactionAppService.AccountInfomation(number, bank);
            if(checkaccount.data == null)
            {
                TempData["check"] = "Wrong Account Number or Bank Name";
                return RedirectToPage("./AddBankInfomation");
            }
            var bankname = await _flutterTransactionAppService.GetBanks();
            foreach (var i in bankname.data)
            {
                if (i.code == bank)
                {
                    BankName = i.name;
                    break;
                }
            }
            Account = checkaccount.data.account_number;
            Name = checkaccount.data.account_name;
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {


           
            var user = await _userManager.GetUserAsync(User);

            var userProfile = await _profile.GetByUserId(user.Id);
            userProfile.BankName = BankName;
            userProfile.AccountName = Name;
            userProfile.AccountNumber = Account;

            await _profile.Update(userProfile);

            string email = user.Email;
            string phone = user.PhoneNumber;
            string Title = "Hi " + userProfile.Surname;
            string SMS = "Your Ahia Pay Bank Information has been Updated.";
            string Subject = "Ahia Pay Bank Information";
            string Message = string.Format("<h4>Your Ahia Pay Bank infomation.</h4> <br><h5>Bank Name: {0}</h5><br><h5>Account Number: {1}</h5><br><h5>Account Name: {2}</h5>", BankName, Name, Account);

            await _emailSender.SendToOne(email, Subject, Title, Message);
            await _emailSender.SMSToOne(phone, SMS);
            return RedirectToPage("./Index");
        }
    }
}
