using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.AdminPage.Pages.Shops
{
    [Authorize(Roles = "mSuperAdmin,SubAdmin,Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public IndexModel(
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


        //public IList<Tenant> Profile { get; set; }
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
        public async Task OnGetAsync(string sortOrder,
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
            //walatapeter@gmail.com
            string uids = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(uids);
            var adminrole = await _userManager.IsInRoleAsync(user, "mSuperAdmin");
            var admini = await _userManager.IsInRoleAsync(user, "Admin");
            string subadminid = "";
            if (adminrole.Equals(true))
            {
                var userad = await _userManager.FindByEmailAsync("MRTOBIEMMANUEL@gmail.com");
                subadminid = userad.Id;
            }
            else if (admini.Equals(true))
            {
                var userad = await _userManager.FindByEmailAsync("MRTOBIEMMANUEL@gmail.com");
                subadminid = userad.Id;
            }
            else
            {
                subadminid = user.Id;
            }
            IQueryable<AdminShopReferral> getids = from s in _context.AdminShopReferrals
                                                   .Where(x => x.MainReferalId == subadminid)
                                                   select s;
            var idlist = getids.Select(x => x.SubReferalId);

            IQueryable<Tenant> iProfile = from s in _context.Tenants
                                       .Include(x=>x.User)
                                       .Include(x=>x.UserProfile)
                                                  .Where(x=> idlist.Contains(x.UserId)).OrderByDescending(x => x.CreationTime)
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
                        || (s.BusinessName != null) && s.BusinessName.ToUpper().Contains(item.ToUpper())
                        || (s.TenentHandle != null) && s.TenentHandle.ToUpper().Contains(item.ToUpper())

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
                       || (s.BusinessName != null) && s.BusinessName.ToUpper().Contains(searchString.ToUpper())
                       || (s.TenentHandle != null) && s.TenentHandle.ToUpper().Contains(searchString.ToUpper())

                       );

                }

            }
            Count = iProfile.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;

            Profile = await PaginatedList<Tenant>.CreateAsync(
                iProfile.AsNoTracking(), pageIndex ?? 1, pageSize);
            CurrentPage = pageIndex ?? 1;

            //Profile = Shops.ToList();
        }
        public async Task<IActionResult> OnPostUpdateUserStatus(string uid, int statusType)
        {
            return RedirectToPage();
        }

    }
}
