using Exwhyzee.Ahioma.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.Ahioma.Data.Repository.AccountCreations
{
   public interface IAccountCreationRepository
    {
        Task<string> CreateAccount(Wallet wallet, UserProfile profile);
        Task<string> CreateAccountTenant(Wallet wallet, UserProfile profile, Market market);

        Task<List<State>> GetStates();

        Task<List<UserListModel>> GetUsers();
        
        Task<List<UserListModel>> GetUsersInRole(string role);
        Task<List<LocalGoverment>> GetLGA(long id);
    }
}
