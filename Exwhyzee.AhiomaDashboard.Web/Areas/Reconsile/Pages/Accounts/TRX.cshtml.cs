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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.Web.Services;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Reconsile.Pages.Accounts
{
    [Authorize(Roles = "mSuperAdmin")]
    public class TRXModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public TRXModel(
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

       // public IQueryable<UserProfile> Profile { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                IQueryable<Transaction> xiTransaction = from s in _context.Transactions
                                             .Where(x => x.UserProfileId == null)
                                                        select s;

                var df = xiTransaction.Count();
                foreach (var i in xiTransaction)
                {
                    var user = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == i.UserId);
                    i.UserProfileId = user.Id;
                    _context.Entry(i).State = EntityState.Modified;

                }
                await _context.SaveChangesAsync();
            }catch(Exception d)
            {

            }

            
        }

    


    }
}
