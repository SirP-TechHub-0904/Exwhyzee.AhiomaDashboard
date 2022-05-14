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

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.UserManager.Pages.Users
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare")]
    public class NonActiveSOModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public NonActiveSOModel(
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


        public PaginatedList<Tenant> Profile { get; set; }
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

            // Profile = await _context.Tenants.Include(x => x.UserProfile).Include(x => x.User).OrderByDescending(x => x.CreationTime).ToListAsync();
            IQueryable<Tenant> iProfile = from s in _context.Tenants
                                  .Include(x => x.User)
                                  .Include(x => x.UserProfile)
                                  .Where(x => x.TenantStatus != Enums.TenantEnum.Enable)
                                  .OrderByDescending(x => x.CreationTime)
                                          select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    IQueryable<Tenant> listProduct = Enumerable.Empty<Tenant>().AsQueryable();
                    List<Tenant> alist = new List<Tenant>();
                    foreach (var item in searchStringCollection)
                    {
                        iProfile = iProfile.Where(s => (s.UserProfile.Surname != null) && s.UserProfile.Surname.ToUpper().Contains(item.ToUpper())
                        || (s.UserProfile.FirstName != null) && s.UserProfile.FirstName.ToUpper().Contains(item.ToUpper())
                        || (s.UserProfile.OtherNames != null) && s.UserProfile.OtherNames.ToUpper().Contains(item.ToUpper())
                        || (s.UserProfile.IdNumber != null) && s.UserProfile.IdNumber.ToUpper().Contains(item.ToUpper())
                        || (s.User.Email != null) && s.User.Email.ToUpper().Contains(item.ToUpper())
                        || (s.User.PhoneNumber != null) && s.User.PhoneNumber.ToUpper().Contains(item.ToUpper())
                        || (s.UserProfile.Roles != null) && s.UserProfile.Roles.ToUpper().Contains(item.ToUpper())

                        );

                        var li = iProfile.ToList();
                        alist.AddRange(li);
                    }

                }
                else
                {
                    iProfile = iProfile.Where(s => (s.UserProfile.Surname != null) && s.UserProfile.Surname.ToUpper().Contains(searchString.ToUpper())
                       || (s.UserProfile.FirstName != null) && s.UserProfile.FirstName.ToUpper().Contains(searchString.ToUpper())
                       || (s.UserProfile.OtherNames != null) && s.UserProfile.OtherNames.ToUpper().Contains(searchString.ToUpper())
                       || (s.UserProfile.IdNumber != null) && s.UserProfile.IdNumber.ToUpper().Contains(searchString.ToUpper())
                       || (s.User.Email != null) && s.User.Email.ToUpper().Contains(searchString.ToUpper())
                       || (s.User.PhoneNumber != null) && s.User.PhoneNumber.ToUpper().Contains(searchString.ToUpper())
                       || (s.UserProfile.Roles != null) && s.UserProfile.Roles.ToUpper().Contains(searchString.ToUpper())

                       );



                }

            }
            Count = iProfile.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;

            Profile = await PaginatedList<Tenant>.CreateAsync(
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
