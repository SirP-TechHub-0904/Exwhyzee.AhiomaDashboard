using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Pages.Shared.ViewComponents
{
    public class UserLast15TransactionsViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
       
        private readonly IUserProfileRepository _account;
        private readonly AhiomaDbContext _context;


        public UserLast15TransactionsViewComponent(
            UserManager<IdentityUser> userManager,
            IUserProfileRepository account
, AhiomaDbContext context)
        {
            _userManager = userManager;
            _account = account;
            _context = context;
        }

        public string UserInfo{ get; set; }

        public async Task<IViewComponentResult> InvokeAsync(string uid)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.IdNumber == uid);

            IQueryable<WalletHistory> productIQ = from s in _context.WalletHistories
                                                  .Where(x=>x.UserId == profile.UserId)
                                            .OrderByDescending(x => x.CreationTime)
                                            .Take(15)
                                            select s;
            ViewBag.list = productIQ;
            return View();
        }
    }
}
