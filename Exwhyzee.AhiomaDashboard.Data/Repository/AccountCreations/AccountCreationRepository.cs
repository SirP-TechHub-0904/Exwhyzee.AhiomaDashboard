using Exwhyzee.Ahioma.Data.Repository.UserProfiles;
using Exwhyzee.Ahioma.EntityFramework.Data;
using Exwhyzee.Ahioma.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.Ahioma.Data.Repository.AccountCreations
{
    public class AccountCreationRepository : IAccountCreationRepository
    {
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _profile;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountCreationRepository(AhiomaDbContext context, UserManager<IdentityUser> userManager,
            UserProfileRepository profile)
        {
            _context = context;
            _userManager = userManager;
            _profile = profile;
        }
        public async Task<string> CreateAccount(Wallet wallet, UserProfile profile)
        {
            _context.Wallets.Add(wallet);
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return wallet.UserId;
        }

        public async Task<string> CreateAccountTenant(Wallet wallet, UserProfile profile, Market market)
        {
            _context.Wallets.Add(wallet);
            _context.UserProfiles.Add(profile);
            _context.Markets.Add(market);
            await _context.SaveChangesAsync();
            return wallet.UserId;
        }

        public async Task<List<LocalGoverment>> GetLGA(long id)
        {
            return await _context.LocalGoverments.Where(x=>x.StatesId == id).ToListAsync();
        }

       
        public async Task<List<State>> GetStates()
        {
            return await _context.States.ToListAsync();
        }

        public async Task<List<UserListModel>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var data = users.Select(x => new UserListModel
            {
                UserId = x.Id,
                FullName = GetProfile(x.Id).Fullname
            }).ToList();
            return data;
        }

        public async Task<List<UserListModel>> GetUsersInRole(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);

            var data = users.Select(x => new UserListModel
            {
                UserId = x.Id,
                FullName = GetProfile(x.Id).Fullname
            }).ToList();
            return data;
        }

        protected UserProfile GetProfile(string id)
        {
            var result = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
            return result;
        }
    }
}
