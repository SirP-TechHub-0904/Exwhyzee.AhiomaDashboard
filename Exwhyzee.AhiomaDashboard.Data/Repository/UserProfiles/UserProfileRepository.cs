using Exwhyzee.AhiomaDashboard.Data.Dtos;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles
{
    public class UserProfileRepository : IUserProfileRepository
    {

        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserProfileRepository(AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.UserProfiles.FindAsync(id);
            _context.UserProfiles.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<IQueryable<UserProfile>> GetAsyncAll()
        {
            IQueryable<UserProfile> data = from s in _context.UserProfiles
                                    .Include(x => x.User).Where(x => x.User.Email != "jinmcever@gmail.com").OrderByDescending(x => x.DateRegistered)
                                           select s;

            return data;
        }
        public async Task<List<UserProfile>> GetAsyncAllActive()
        {
            var data = await _context.UserProfiles.Include(x => x.User).Where(x => x.User.Email != "jinmcever@gmail.com" && x.Status == Enums.AccountStatus.Active).OrderByDescending(x => x.DateRegistered).ToListAsync();
            return data;
        }
        public async Task<IQueryable<UserProfile>> GetAsyncAllByRole(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            var selecteduser = users.Select(x => x.Id);
            var data = _context.UserProfiles.Include(x => x.User).Where(x => x.User.Email != "jinmcever@gmail.com" && selecteduser.Contains(x.UserId)).OrderByDescending(x => x.DateRegistered);
            return data;
        }

        public async Task<UserProfile> GetById(long? id)
        {
            var data = await _context.UserProfiles.Include(x => x.User).Include(x => x.UserReferees).Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<UserProfile> GetByUserId(string id)
        {
            var data = await _context.UserProfiles.Include(x => x.User).Include(x => x.UserReferees).Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == id);
            return data;
        }

        public async Task<long> Insert(UserProfile model)
        {
            _context.UserProfiles.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(UserProfile model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<string> CreateAccount(Wallet wallet, UserProfile profile, UserAddress address, UserReferee referee, AdminReferral referralsystem)
        {
            if (referralsystem != null)
            {
                if (referralsystem.SubReferalId != null)
                {
                    _context.AdminReferrals.Add(referralsystem);
                }
            }
            _context.Wallets.Add(wallet);
            _context.UserProfiles.Add(profile);
            _context.UserAddresses.Add(address);
            _context.UserReferees.Add(referee);
            await _context.SaveChangesAsync();
            try
            {
                var profileupdate = await _context.UserProfiles.FindAsync(profile.Id);
                profileupdate.IdNumber = profileupdate.Id.ToString("0000000");
                _context.Attach(profileupdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception d) { }
            return wallet.UserId;
        }

        public async Task<string> CreateAccountTenant(Wallet wallet, UserProfile profile, Tenant tenant, TenantAddress tenantAddress, AdminShopReferral adminref, string Facebook, string Instagram, string LinkedIn, string WhatsappNumber)
        {
            _context.Wallets.Add(wallet);
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
            tenant.UserProfileId = profile.Id;
            if (adminref.MainReferalId != null)
            {
                adminref.SubReferalId = profile.UserId;
                _context.AdminShopReferrals.Add(adminref);
            }
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
            //
            tenantAddress.TenantId = tenant.Id;
            _context.TenantAddresses.Add(tenantAddress);
            try
            {
                TenantSocialMedia fbook = new TenantSocialMedia();
                fbook.Name = "Facebook";
                fbook.Uri = Facebook;
                fbook.TenantId = tenant.Id;
                _context.TenantSocialMedias.Add(fbook);
                //
                TenantSocialMedia istan = new TenantSocialMedia();
                istan.Name = "Instagram";
                istan.Uri = Instagram;
                istan.TenantId = tenant.Id;

                _context.TenantSocialMedias.Add(istan);

                //
                TenantSocialMedia linkd = new TenantSocialMedia();
                linkd.Name = "LinkedIn";
                linkd.Uri = LinkedIn;
                linkd.TenantId = tenant.Id;

                _context.TenantSocialMedias.Add(linkd);
                //
                TenantSocialMedia whatsapp = new TenantSocialMedia();
                whatsapp.Name = "Whatsapp Number";
                whatsapp.Uri = WhatsappNumber;
                whatsapp.TenantId = tenant.Id;

                _context.TenantSocialMedias.Add(whatsapp);
                await _context.SaveChangesAsync();

                var profileupdate = await _context.UserProfiles.FindAsync(profile.Id);
                profileupdate.IdNumber = profileupdate.Id.ToString("0000000");
                _context.Attach(profileupdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception c)
            {

            }
            await _context.SaveChangesAsync();
            return wallet.UserId;
        }

        public async Task<List<LocalGoverment>> GetLGA(string id)
        {
            var states = await _context.States.FirstOrDefaultAsync(x => x.StateName == id);
            return await _context.LocalGoverments.Where(x => x.StatesId == states.Id).ToListAsync();
        }


        public async Task<List<State>> GetStates()
        {
            return await _context.States.ToListAsync();
        }

        public async Task<List<UserListModelDto>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var data = users.Select(x => new UserListModelDto
            {
                UserId = x.Id,
                FullName = GetProfile(x.Id)
            }).ToList();
            return data;
        }

        public async Task<List<UserListModelDto>> GetUsersInRole(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);

            var data = users.Select(x => new UserListModelDto
            {
                UserId = x.Id,
                FullName = GetProfile(x.Id),
                IdNumber = GetProfileIdNumber(x.Id)

            }).ToList();
            return data;
        }

        protected string GetProfile(string id)
        {
            var result = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
            if (result == null)
            {
                return "---";
            }
            return result.Fullname;
        }
        protected string GetProfileIdNumber(string id)
        {
            var result = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
            if (result == null)
            {
                return "---";
            }
            return result.IdNumber;
        }

        public async Task<string> FetchUserRoles(string UserId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(UserId);
                var userroles = await _userManager.GetRolesAsync(user);
                List<string> roles = userroles.ToList();

                string rolestoreturn = string.Join(", ", roles);

                return rolestoreturn;
            }
            catch (Exception c)
            {
                return "null";
            }
        }

        public async Task<List<Tenant>> GetAsyncStores(string uid)
        {
            var user = await _userManager.FindByIdAsync(uid);
            var userSadmin = await _userManager.IsInRoleAsync(user, "mSuperAdmin");
            var userAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var stores = await _context.Tenants.Include(x => x.User).Include(x => x.Market).Include(x => x.TenantAddresses).ToListAsync();
            if (userSadmin == true)
            {
                stores = stores.ToList();
                return stores;
            }
            else if (userAdmin == true)
            {
                return stores;
            }
            stores = stores.Where(x => x.CreationUserId == uid).ToList();

            return stores;
        }

        public async Task<UserReferee> GetUserAddress(long id)
        {
            //var address = await _context.UserAddresses.Include
            throw new NotImplementedException();

        }

        public Task<UserReferee> GetUserReferee(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CreateAccountLogistic(Wallet wallet, UserProfile profile, LogisticProfile logisticProfile, UserAddress address, string Facebook, string Instagram, string LinkedIn, string WhatsappNumber)
        {
            _context.Wallets.Add(wallet);
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
            logisticProfile.UserProfileId = profile.Id;
            _context.LogisticProfiles.Add(logisticProfile);
            await _context.SaveChangesAsync();
            //


            try
            {


                var profileupdate = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == wallet.UserId);
                profileupdate.IdNumber = profileupdate.Id.ToString("0000000");
                _context.Attach(profileupdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception c)
            {

            }
            return wallet.UserId;
        }
    }
}
