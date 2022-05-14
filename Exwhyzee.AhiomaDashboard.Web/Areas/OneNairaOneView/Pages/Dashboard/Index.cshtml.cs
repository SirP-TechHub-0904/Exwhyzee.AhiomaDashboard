using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.OneNairaOneView.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin,StatusManager")]
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
        [BindProperty]
        public string UserId { get; set; }

        public decimal Amount { get; set; }
        public int AllUsers { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public DateTime StartDate { get; set; }
        public int NewAddUsers { get; set; }
        public decimal TotalSumCredited { get; set; }
        public decimal TotalSumDebited { get; set; }
        public int TotalAddUsers { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }

        public int TodayTotalAddUsers { get; set; }
        public decimal TodayCredit { get; set; }
        public decimal TodayDebit { get; set; }
        public decimal Balance { get; set; }
        public decimal Overall { get; set; }
        public async Task<IActionResult> OnGetAsync(string sortOrder,
          string currentFilter, string searchString, int? pageIndex, long id, DateTime? DateOne)
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

            #region date
            if (DateOne != null)
            {
                DateOne = DateOne.Value.Date;
            }
            else
            {
                DateOne = DateTime.UtcNow.AddHours(1).Date;
            }

            StartDate = DateOne.Value;

            #endregion

            IQueryable<Transaction> iitransact = from s in _context.Transactions
                                                .Include(x => x.UserProfile)
                                                .Include(x => x.UserProfile.User)
                                    .Where(x => x.Description.Contains("1 View 1 Naira Comm."))
                                    .Where(x=>x.TransactionType == Enums.TransactionTypeEnum.Debit)
                                                   .OrderByDescending(x => x.DateOfTransaction)
                                                 select s;

            IQueryable<Transaction> tcredit = from s in _context.Transactions
                                              .Include(x => x.UserProfile)
                                              .Include(x => x.UserProfile.User)
                                  .Where(x => x.UserProfile.User.Email == "ahiomaadvert@gmail.com")
                                  .Where(x => x.TransactionType == Enums.TransactionTypeEnum.Credit)
                                                 .OrderByDescending(x => x.DateOfTransaction)
                                                 select s;

            var itransact = iitransact.Where(x => x.DateOfTransaction.Date == DateOne);

            ///
            IQueryable<UserProfile> nProfile = from s in _context.UserProfiles.Where(x => x.User.Email != "jinmcever@gmail.com")
                                .Where(x => x.ActivateForAdvert == true).OrderByDescending(x => x.DateRegistered)
                                               select s;
            NewAddUsers = nProfile.Where(x => x.ActivateForAdvert == true && x.AddForAdvert.Date == DateOne).Count();

            TodayCredit = await tcredit.Where(x => x.DateOfTransaction.Date == DateOne).SumAsync(x => x.Amount);
            TodayDebit = await itransact.Where(x => x.DateOfTransaction.Date == DateOne).SumAsync(x => x.Amount);

            Overall = await itransact.SumAsync(x => x.Amount);
            Credit = await tcredit.SumAsync(x => x.Amount);
            Debit = await itransact.Where(x => x.TransactionType == TransactionTypeEnum.TransferDebit).SumAsync(x => x.Amount);
            return Page();
        }


    }
}
