using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.OneNairaOneView.Pages.Dashboard
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
      
        public async Task OnGetAsync()
        {



            IQueryable<Transaction> itransact = from s in _context.Transactions.Where(x => x.Description.Contains("1 View 1 Naira Comm."))
                                            .OrderByDescending(x => x.DateOfTransaction)
                                                select s;

            Amount = itransact.Sum(x => x.Amount);


            IQueryable<UserProfile> nProfile = from s in _context.UserProfiles.Where(x => x.User.Email != "jinmcever@gmail.com")
                                  .Where(x => x.ActivateForAdvert == true).OrderByDescending(x => x.DateRegistered)
                                               select s;
            AllUsers = nProfile.Count();
        }



    }
}
