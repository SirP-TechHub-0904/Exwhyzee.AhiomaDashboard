using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Balance;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Transfer;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RestSharp;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin")]

    public class TransferModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IUserLogging _log;

        private readonly IFlutterTransactionService _flutterTransactionAppService;


        public TransferModel(
                UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IUserLogging log,
                SignInManager<IdentityUser> signInManager,
                IFlutterTransactionService flutterTransactionAppService,
                IUserProfileRepository account, AhiomaDbContext context,
                IEmailSendService emailSender)
        {
            _userManager = userManager;
            _context = context;
            _log = log;
            _flutterTransactionAppService = flutterTransactionAppService;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;

        }
        [BindProperty]
        public string AccountNumber { get; set; }
        [BindProperty]
        public decimal Amount { get; set; }
        [BindProperty]
        public string Bank { get; set; }
        [BindProperty]
        public string UID { get; set; }
        public List<SelectListItem> BankListing { get; set; }
      public Balance Balance { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var list = await _flutterTransactionAppService.GetBanks();

            Balance = await _flutterTransactionAppService.GetBalance();


            BankListing = list.data.Select(a =>
                                new SelectListItem
                                {
                                    Value = a.code,
                                    Text = a.name
                                }).ToList();
            return Page();
       
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var authenticatorCode = UID.Replace(" ", string.Empty).Replace("-", string.Empty);
            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, authenticatorCode);

            if (!is2faTokenValid)
            {
                TempData["error"] = "Verification code is invalid.";
                return Page();
            }
            if (!String.IsNullOrEmpty(AccountNumber))
            {
               
                string account_bank = Bank;
                string account_number = AccountNumber;
                int amount = Convert.ToInt32(Amount);
                string narration = "Ahioma Transfer";
                string currency = "NGN";
                string reference = "";
                string callback_url = "";
                string debit_currency = "NGN";

                //var response = await _flutterTransactionAppService.Transfer(account_bank, account_number, amount, narration,
                //    currency, reference, callback_url, debit_currency, UID);

                try
                {
                    try
                    {
                        string apiurl = $"https://api.flutterwave.com/v3/transfers";
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                        var client = new RestClient(apiurl);
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("content-type", "application/json");
                        request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

                        var lad = new TModel
                        {
                            account_bank = account_bank,
                            account_number = account_number,
                            amount = amount,
                            narration = narration,
                            currency = currency,
                            reference = reference,
                            callback_url = callback_url,
                            debit_currency = debit_currency
                        };

                        request.AddJsonBody(lad);

                        Thread.Sleep(4000);
                       

                        IRestResponse responses = client.Execute(request);
                        var contents = responses.Content.ToString();
                        //var json = await response.Content.ReadAsStringAsync();

                        var mainresponse = JsonConvert.DeserializeObject<TransferModel>(contents);
                        TempData["error"] = mainresponse;
                    }
                    catch (Exception g)
                    {
                        return null;
                    }

                }
                catch (Exception s)
                {

                }
               
            }
            else
            {
                TempData["error"] = "Error";
            }
           
            return Page();
        }
    }
}
