﻿using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class EmailVerifyViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        //private readonly IEmailSender _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;

        public EmailVerifyViewComponent(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            ITenantRepository tenant,
            IUserProfileRepository account
            /*IEmailSender emailSender*/)
        {
            _userManager = userManager;
            //_emailSender = emailSender;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tenant = tenant;
        }

        public string UserInfo{ get; set; }
        public string LoggedInUser { get; set; }
        public async Task<IViewComponentResult> InvokeAsync(string uid)
        {
            string newid = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(newid);
            var superrole = await _userManager.IsInRoleAsync(user, "mSuperAdmin");

            if (superrole.Equals(true))
            {
                LoggedInUser = uid;
            }
            else
            {
                LoggedInUser = user.Id;
            }
                
            var userdata = await _userManager.FindByIdAsync(LoggedInUser);
            var profile = await _account.GetByUserId(userdata.Id);
            if (userdata.EmailConfirmed != true)
            {
                TempData["UserVerify"] = "false";
            }
            ViewBag.User = userdata.EmailConfirmed;
            ViewBag.UserActivated = profile.Status;
            return View();
        }
    }
}
