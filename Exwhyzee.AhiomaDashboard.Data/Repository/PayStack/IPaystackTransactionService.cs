using Exwhyzee.AhiomaDashboard.Data.Paystack;
using Exwhyzee.AhiomaDashboard.Data.Paystack.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.PayStack
{
    public interface IPaystackTransactionService
    {
        Task<PaymentInitalizationResponse> InitializeTransaction(string secretKey, string email, int amount, long transactionId, string firstName = null,
            string lastName = null, string callbackUrl = null, string reference = null, bool makeReferenceUnique = false);

        Task<TransactionResponseModel> VerifyTransaction(string reference, string secretKey);

        // HttpClient CreateClient(string secretKey);
    }
}
