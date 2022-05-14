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

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Manager.Pages.TransactionQueue
{
    [Authorize(Roles = "mSuperAdmin")]
    public class WHistoryModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public WHistoryModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,

            IUserProfileRepository account, AhiomaDbContext context,
            IEmailSendService emailSender)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public PaginatedList<WalletHistory> Profile { get; set; }
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
                               .Include(x => x.Profile)
                               .Include(x => x.Profile.User)
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
                        iProfile = iProfile.Where(s => (s.Profile.Surname != null) && s.Profile.Surname.ToUpper().Contains(item.ToUpper())
                        || (s.Profile.FirstName != null) && s.Profile.FirstName.ToUpper().Contains(item.ToUpper())
                        || (s.Profile.OtherNames != null) && s.Profile.OtherNames.ToUpper().Contains(item.ToUpper())
                        || (s.Profile.IdNumber != null) && s.Profile.IdNumber.ToUpper().Contains(item.ToUpper())
                        || (s.Profile.User.Email != null) && s.Profile.User.Email.ToUpper().Contains(item.ToUpper())
                        || (s.Profile.User.PhoneNumber != null) && s.Profile.User.PhoneNumber.ToUpper().Contains(item.ToUpper())
                        || (s.Source != null) && s.Source.ToUpper().Contains(item.ToUpper())
                        || (s.Destination != null) && s.Destination.ToUpper().Contains(item.ToUpper())
                        || (s.Reason != null) && s.Reason.ToUpper().Contains(item.ToUpper())
                                              || (s.CreationTime.ToString().ToUpper().Contains(searchString.ToUpper()))

                        );

                        var li = iProfile.ToList();
                        alist.AddRange(li);
                    }

                }
                else
                {
                    iProfile = iProfile.Where(s => (s.Profile.Surname != null) && s.Profile.Surname.ToUpper().Contains(searchString.ToUpper())
                       || (s.Profile.FirstName != null) && s.Profile.FirstName.ToUpper().Contains(searchString.ToUpper())
                       || (s.Profile.OtherNames != null) && s.Profile.OtherNames.ToUpper().Contains(searchString.ToUpper())
                       || (s.Profile.IdNumber != null) && s.Profile.IdNumber.ToUpper().Contains(searchString.ToUpper())
                       || (s.Profile.User.Email != null) && s.Profile.User.Email.ToUpper().Contains(searchString.ToUpper())
                       || (s.Profile.User.PhoneNumber != null) && s.Profile.User.PhoneNumber.ToUpper().Contains(searchString.ToUpper())
                       || (s.Source != null) && s.Source.ToUpper().Contains(searchString.ToUpper())
                       || (s.Destination != null) && s.Destination.ToUpper().Contains(searchString.ToUpper())
                       || (s.Reason != null) && s.Reason.ToUpper().Contains(searchString.ToUpper())
                       || (s.CreationTime.ToString().ToUpper().Contains(searchString.ToUpper()))

                       );



                }

            }
            Count = iProfile.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;

            Profile = await PaginatedList<WalletHistory>.CreateAsync(
                iProfile.AsNoTracking(), pageIndex ?? 1, pageSize);
            CurrentPage = pageIndex ?? 1;

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateUserStatus(string uid, int statusType)
        {
            return RedirectToPage();
        }

    }
}
