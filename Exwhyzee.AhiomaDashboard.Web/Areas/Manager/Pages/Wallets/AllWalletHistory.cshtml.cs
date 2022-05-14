using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Balance;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Manager.Pages.Wallets
{
    [Authorize(Roles = "MainAdmin,mSuperAdmin")]

    public class AllWalletHistoryModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IUserLogging _log;

        private readonly IFlutterTransactionService _flutterTransactionAppService;


        public AllWalletHistoryModel(
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

        public PaginatedList<WalletHistory> Wallet { get; set; }
        public int CountString(string searchString)
        {
            int result = 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();

                if (searchString == "")
                    return 0;

                while (searchString.Contains("  "))
                    searchString = searchString.Replace("  ", " ");

                foreach (string y in searchString.Split(' '))

                    result++;

            }
            return result;
        }

        public int Count { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int? CurrentPage { get; set; }
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;
        public async Task<IActionResult> OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            

            CurrentFilter = searchString;
            CurrentSort = sortOrder;

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            IQueryable<WalletHistory> iProfile = from s in _context.WalletHistories
                                               .Include(p => p.Profile)
              .OrderByDescending(x => x.CreationTime)
                                                 select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    IQueryable<WalletHistory> listProduct = Enumerable.Empty<WalletHistory>().AsQueryable();
                    List<WalletHistory> alist = new List<WalletHistory>();
                    foreach (var item in searchStringCollection)
                    {
                        iProfile = iProfile.Where(s => (s.CreationTime != null) && s.CreationTime.ToString().ToUpper().Contains(item.ToUpper())
                        || (s.Source != null) && s.Source.ToUpper().Contains(item.ToUpper())
                        || (s.Destination != null) && s.Destination.ToUpper().Contains(item.ToUpper())
                        || (s.Amount != null) && s.Amount.ToString().ToUpper().Contains(item.ToUpper())
                        || (s.LedgerBalance != null) && s.LedgerBalance.ToString().ToUpper().Contains(item.ToUpper())
                        || (s.AvailableBalance != null) && s.AvailableBalance.ToString().ToUpper().Contains(item.ToUpper())
                        
                        );

                        var li = iProfile.ToList();
                        alist.AddRange(li);
                    }

                }
                else
                {
                    iProfile = iProfile.Where(s => (s.CreationTime != null) && s.CreationTime.ToString().ToUpper().Contains(searchString.ToUpper())
                       || (s.Source != null) && s.Source.ToUpper().Contains(searchString.ToUpper())
                       || (s.Destination != null) && s.Destination.ToUpper().Contains(searchString.ToUpper())
                       || (s.Amount != null) && s.Amount.ToString().ToUpper().Contains(searchString.ToUpper())
                       || (s.LedgerBalance != null) && s.LedgerBalance.ToString().ToUpper().Contains(searchString.ToUpper())
                       || (s.AvailableBalance != null) && s.AvailableBalance.ToString().ToUpper().Contains(searchString.ToUpper())

                      );



                }

            }
            Count = iProfile.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;

            Wallet = await PaginatedList<WalletHistory>.CreateAsync(
                iProfile.AsNoTracking(), pageIndex ?? 1, pageSize);
            CurrentPage = pageIndex ?? 1;

            return Page();

        }

        private string NameWallet(string id)
        {
            try
            {
                var userprofile = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
                return userprofile.Fullname;
            }
            catch (Exception k)
            {
                return "";
            }
        }
        private string IDNameWallet(string id)
        {
            try
            {
                var userprofile = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
                return userprofile.IdNumber;
            }
            catch (Exception k)
            {
                return "";
            }
        }
    }
}


