using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.AhiomaDashboard.EntityFramework.Dtos;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using Exwhyzee.Enums;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Microsoft.AspNetCore.Hosting;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.OneNairaOneView.Pages.Dashboard
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare")]
    public class OneHistoryModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IWalletRepository _walletAppService;

        private readonly IHostingEnvironment _hostingEnv;


        public OneHistoryModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,

            IUserProfileRepository account, AhiomaDbContext context,
            IEmailSendService emailSender, IWalletRepository walletAppService, IHostingEnvironment hostingEnv)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _walletAppService = walletAppService;
            _hostingEnv = hostingEnv;
        }

        public PaginatedList<Transaction> Transaction { get; set; }
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
        public string LoggedInUser { get; set; }
        public decimal Balance { get; set; }


        [BindProperty]
        public UserProfile UserProfile { get; set; }
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
            
            IQueryable<Transaction> itransact = from s in _context.Transactions
                                                .Where(x => x.Description.Contains("1 View 1 Naira Comm."))
                                                   .OrderByDescending(x => x.DateOfTransaction)
                                                select s;

             
          
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    IQueryable<Transaction> listProduct = Enumerable.Empty<Transaction>().AsQueryable();
                    List<Transaction> alist = new List<Transaction>();
                    foreach (var item in searchStringCollection)
                    {
                        itransact = itransact.Where(s => (s.Amount != null) && s.Amount.ToString().ToUpper().Contains(item.ToUpper())
                        || (s.TrackCode != null) && s.TrackCode.ToUpper().Contains(item.ToUpper())
                        || (s.DateOfTransaction != null) && s.DateOfTransaction.ToString().ToUpper().Contains(item.ToUpper())
                        

                        );

                        var li = itransact.ToList();
                        alist.AddRange(li);
                    }

                }
                else
                {
                    itransact = itransact.Where(s => (s.Amount != null) && s.Amount.ToString().ToUpper().Contains(searchString.ToUpper())
                        || (s.TrackCode != null) && s.TrackCode.ToUpper().Contains(searchString.ToUpper())
                        || (s.DateOfTransaction != null) && s.DateOfTransaction.ToString().ToUpper().Contains(searchString.ToUpper())

                      );



                }

            }
           var gCount = itransact.ToList();
            Count = itransact.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;

            Transaction = await PaginatedList<Transaction>.CreateAsync(
                itransact.AsNoTracking(), pageIndex ?? 1, pageSize);
            CurrentPage = pageIndex ?? 1;

            return Page();
        }
        [TempData]
        public string StatusMessage { get; set; }



    }
}
