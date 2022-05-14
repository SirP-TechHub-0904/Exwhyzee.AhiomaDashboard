using Exwhyzee.AhiomaDashboard.Data.Flutter.Balance;
using Exwhyzee.AhiomaDashboard.Data.Flutter.BankInfo;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Bill;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Model;
using Exwhyzee.AhiomaDashboard.Data.Flutter.TransactionGetAsync;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Transfer;
using Exwhyzee.AhiomaDashboard.Data.Flutter.TransferGetAsync;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Verify;
using Exwhyzee.AhiomaDashboard.Data.Paystack.Model.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Flutter
{
   public interface IFlutterTransactionService
    {
        Task<FlutterResponse> InitializeTransaction(string tx_ref, string amount, string currency, string redirect_url, string payment_options,
           int consumer_id, string consumer_mac, string email, string phonenumber, string name, string title, string description, string logo, string from);

        Task<FlutterTransactionVerify> VerifyTransaction(string tx_ref);
        Task<BankVerify> GetBanks();
        Task<Balance> GetBalance();
        Task<ResponseRequestBill> CreateBill(RequestBill model);

        

        Task<FetchTransfer> GetAllTransfer(string page, string status);
        Task<GetAllTransaction> GetAllTransactions(string from, string to, int page, string customerEmail, string status, string tx_ref, string customerName);
        Task<BankInformation> AccountInfomation(string account_number, string account_bank);
        Task<string> Transfer(string account_bank, string account_number, int amount, string narration, string currency, string reference, string callback_url, string debit_currency, string uid, string from);
        Task<string> MajorTransfer(long id);

        Task<string> GetBills();
    }
}
