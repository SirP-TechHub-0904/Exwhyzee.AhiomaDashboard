using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Transactions
{
    public interface ITransactionRepository
    {
        Task<long> Insert(Transaction model);
        Task<long> CreateCreditTransactionAndUpdateWallet(Transaction model, string UserId, string tranxRef);
        Task<long> CreateDebitTransactionAndUpdateWallet(Transaction model, string UserId, string tranxRef);
        Task<Transaction> GetById(long? id);
        Task Delete(long? id);
        Task<Transaction> Update(Transaction model);
        Task<List<Transaction>> GetAsyncAll();
        Task<Transaction> CreateTransaction(Transaction transaction);
        Task<List<Transaction>> GetAsyncAllById(string UserId);
        Task<List<Transaction>> GetAsyncAllByUserId(string UserId);
    }
}
