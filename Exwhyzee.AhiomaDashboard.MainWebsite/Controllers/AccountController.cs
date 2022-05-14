using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Controllers.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserProfileRepository _account;
        private IConfiguration _config;

        public AccountController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<AccountController> logger, IConfiguration config,
             RoleManager<IdentityRole> roleManager,
            IUserProfileRepository account)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _config = config;
            _account = account;
            _roleManager = roleManager;

        }

        #region Loging
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Token([FromBody]LoginDto login)
        {
            var user = await Authenticate(login);

            if (user == null)
            {
                return BadRequest("Authentication Failed");
            }

            var token = BuildToken(user);

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }

            return Ok(token);
        }

        private string BuildToken(IdentityUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<IdentityUser> Authenticate(LoginDto login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null)
            {
                return null;
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }

        #endregion

        #region Register
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
        {

           // var check = _userManager.Users.Where(x => x.UserName != "mJinmcever").ToList();
            if (ModelState.IsValid)
            {
                var check = _userManager.Users.ToList();

                if (check.Select(x => x.Email).Contains(registerDto.Email))
                {


                    return null;
                }

                if (check.Select(x => x.PhoneNumber).Contains(registerDto.PhoneNumber))
                {

                    return null;
                }
                var user = new IdentityUser { UserName = registerDto.Email, Email = registerDto.Email, PhoneNumber = registerDto.PhoneNumber };
               

                //age

                try
                {


                    var result = await _userManager.CreateAsync(user, registerDto.Password);
                    if (result.Succeeded)
                    {
                        var Role = await _roleManager.FindByIdAsync(registerDto.Role);
                        var CustomerRole = await _roleManager.FindByNameAsync("Customer");
                        if (Role != null)
                        {
                            if (registerDto.Role.Contains("Customer"))
                            {

                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, CustomerRole.Name);
                            }
                            await _userManager.AddToRoleAsync(user, Role.Name);

                        }
                        var Userroles = "";
                        try
                        {
                            Userroles = await _account.FetchUserRoles(user.Id);
                        }
                        catch (Exception c) { }
                        // _logger.LogInformation("User created a new account with password.");
                        //create wallet
                        Wallet wallet = new Wallet();
                        wallet.UserId = user.Id;
                        wallet.Balance = 0;
                        wallet.CreationTime = DateTime.UtcNow.AddHours(1);
                        wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);


                        UserProfile profile = new UserProfile();
                        profile.UserId = user.Id;
                        profile.DateRegistered = DateTime.UtcNow.AddHours(1);
                        profile.Roles = Userroles;
                        profile.FirstName = registerDto.FirstName;
                        profile.OtherNames = registerDto.OtherNames;
                        profile.Surname = registerDto.Surname;
                        profile.Status = Enums.AccountStatus.Active;

                        UserAddress address = new UserAddress();
                        address.UserId = user.Id;
                        address.Address = registerDto.ContactAddress;
                        address.State = registerDto.State;
                        address.LocalGovernment = registerDto.LocalGovernment;

                        UserReferee referee = new UserReferee();
                        referee.UserId = user.Id;
                        referee.FullName = registerDto.RefereeName;
                        referee.ContactAddress = registerDto.RefereeName;


                        string id = await _account.CreateAccount(wallet, profile, address, referee, null);


                        return Ok(registerDto);

                        // return RedirectToPage("./Manage/Verify", new { id = user.Id });
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return BadRequest("Account setup failed.");
        }

       

        #endregion
    }


}