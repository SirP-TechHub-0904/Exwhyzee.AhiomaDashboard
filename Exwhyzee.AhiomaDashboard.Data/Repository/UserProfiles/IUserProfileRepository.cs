using Exwhyzee.AhiomaDashboard.Data.Dtos;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles
{
   public interface IUserProfileRepository
    {
        Task<long> Insert(UserProfile model);
        Task<UserProfile> GetById(long? id);
        Task<UserProfile> GetByUserId(string id);
        Task<UserReferee> GetUserAddress(long id);
        Task<UserReferee> GetUserReferee(long id);
        Task Delete(long? id);
        Task Update(UserProfile model);
        Task<IQueryable<UserProfile>> GetAsyncAll();
        Task<IQueryable<UserProfile>> GetAsyncAllByRole(string role);
        Task<List<UserProfile>> GetAsyncAllActive();
        Task<List<Tenant>> GetAsyncStores(string uid);
        Task<string> CreateAccount(Wallet wallet, UserProfile profile, UserAddress address, UserReferee referee, AdminReferral referralsystem);
        Task<string> CreateAccountTenant(Wallet wallet, UserProfile profile, Tenant tenant, TenantAddress tenantAddress, AdminShopReferral adminref, string Facebook, string Instagram, string LinkedIn, string WhatsappNumber);
        Task<string> CreateAccountLogistic(Wallet wallet, UserProfile profile, LogisticProfile logisticProfile, UserAddress address, string Facebook, string Instagram, string LinkedIn, string WhatsappNumber);

        Task<List<State>> GetStates();
        Task<string> FetchUserRoles(string UserId);

        Task<List<UserListModelDto>> GetUsers();

        Task<List<UserListModelDto>> GetUsersInRole(string role);
        Task<List<LocalGoverment>> GetLGA(string id);
    }
}
