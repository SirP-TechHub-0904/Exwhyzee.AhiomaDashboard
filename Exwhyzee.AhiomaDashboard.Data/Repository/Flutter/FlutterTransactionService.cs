using Exwhyzee.AhiomaDashboard.Data.Flutter.Balance;
using Exwhyzee.AhiomaDashboard.Data.Flutter.BankInfo;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Bill;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Model;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Transfer;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Verify;
using Exwhyzee.AhiomaDashboard.Data.Paystack;
using Exwhyzee.AhiomaDashboard.Data.Paystack.Model.Response;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Flutter
{
    public class FlutterTransactionService : IFlutterTransactionService
    {
        private readonly IMessageRepository _message;
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITransactionRepository _transactionAppService;
        public FlutterTransactionService(IMessageRepository message, EntityFramework.Data.AhiomaDbContext context, UserManager<IdentityUser> userManager, ITransactionRepository transactionAppService)
        {
            _message = message;
            _context = context;
            _userManager = userManager;
            _transactionAppService = transactionAppService;
        }

        public static string GenerateToken()
        {

            // Token will be good for 20 minutes
            DateTime Expiry = DateTime.UtcNow.AddMinutes(20);

            string ApiKey = "FLWPUBK_TEST-f4bb871959b328ba5c04417f81263902-X";
            string ApiSecret = "FLWSECK_TEST-2e538587a52c4836a1e5860199817f71-X";

            int ts = (int)(Expiry - new DateTime(1970, 1, 1)).TotalSeconds;

            // Create Security key  using private key above:
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiSecret));

            // length should be >256b
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Finally create a Token
            var header = new JwtHeader(credentials);

            //Zoom Required Payload
            var payload = new JwtPayload
        {
            { "iss", ApiKey},
            { "exp", ts },
        };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(secToken);

            return tokenString;
        }

        public async Task<BankInformation> AccountInfomation(string account_number, string account_bank)
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/accounts/resolve";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

                var lad = new RequestAccountInfo
                {
                    account_bank = account_bank,
                    account_number = account_number
                };

                request.AddJsonBody(lad);
                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<BankInformation>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<Data.Flutter.TransactionGetAsync.GetAllTransaction> GetAllTransactions(string from, string to, int page, string customerEmail, string status, string tx_ref, string customerName)
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transactions";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

                request.AddParameter("from", from);
                request.AddParameter("to", to);
                request.AddParameter("page", page);
                request.AddParameter("customer_email", customerEmail);
                request.AddParameter("status", status);
                request.AddParameter("tx_ref", tx_ref);
                request.AddParameter("customer_fullname", customerName);
                request.AddParameter("currency", "NGN");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<Data.Flutter.TransactionGetAsync.GetAllTransaction>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<Data.Flutter.TransferGetAsync.FetchTransfer> GetAllTransfer(string page, string status)
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transfers";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");
                request.AddParameter("page", page);
                request.AddParameter("status", status);

                //var lad = new Data.Flutter.TransferGetAsync.TransferValue
                //{
                //    page = page,
                //    status = status
                //};

                //request.AddJsonBody(lad);

                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<Data.Flutter.TransferGetAsync.FetchTransfer>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<Balance> GetBalance()
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/balances";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<Balance>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<BankVerify> GetBanks()
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/banks/NG";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<BankVerify>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<FlutterResponse> InitializeTransaction(string tx_ref, string amount, string currency, string redirect_url, string payment_options, int consumer_id, string consumer_mac, string email, string phonenumber, string name, string title, string description, string logo, string from)
        {
            var token = GenerateToken();
            ///v2/accounts/128273138/users/{userId}/meetings
            /////https://api.zoom.us/v2/users
            ///
            string apiurl = "https://api.flutterwave.com/v3/payments";
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            var client = new RestClient(apiurl);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

            var lad = new TransactionResponseModel
            {
                tx_ref = tx_ref,
                amount = amount,
                currency = currency,
                redirect_url = redirect_url,
                payment_options = payment_options,
                meta = new Meta
                {
                    consumer_id = consumer_id,
                    consumer_mac = consumer_mac
                },
                customer = new Data.Flutter.Model.Customer
                {
                    email = email,
                    phonenumber = phonenumber,
                    name = name
                },
                customizations = new Customizations
                {
                    title = title,
                    description = description,
                    logo = logo
                }

            };

            request.AddJsonBody(lad);

            IRestResponse response = client.Execute(request);
            var contents = response.Content.ToString();
            //var json = await response.Content.ReadAsStringAsync();

            var mainresponse = JsonConvert.DeserializeObject<FlutterResponse>(contents);
            return mainresponse;
            //if (mainresponse.status == true)
            //{
            //    return Redirect(response.data.authorization_url);
            //}

        }

        public async Task<string> Transfer(string account_bank, string account_number, int amount, string narration, string currency, string reference, string callback_url, string debit_currency, string uid, string from)
        {
            string outputstring = "";
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transfers";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

                var lad = new TModel
                {
                    account_bank = account_bank,
                    account_number = account_number,
                    amount = amount,
                    narration = narration,
                    currency = currency,
                    reference = reference,
                    callback_url = callback_url,
                    debit_currency = debit_currency
                };

                request.AddJsonBody(lad);

                Thread.Sleep(2000);
                var Walletd = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == uid);
                if (amount < 500)
                {
                    return "Minimum Amount is N500.";

                }
                if (amount > Walletd.WithdrawBalance)
                {
                    return "Insufficient Amount";

                }


                //  IRestResponse response = client.Execute(request);

                // var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                // var mainresponse = JsonConvert.DeserializeObject<TransferModel>(contents);
                //return mainresponse;
                var mainresponse = "success";
                var user = await _userManager.FindByIdAsync(uid);
                var Profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                var Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
                var guid = Guid.NewGuid().ToString();
                if (mainresponse != null)
                {

                    if (mainresponse == "success")
                    {
                        TransferQueue qu = new TransferQueue();
                        qu.account_bank = lad.account_bank;
                        qu.account_number = lad.account_number;
                        qu.amount = lad.amount;
                        qu.callback_url = lad.callback_url;
                        qu.currency = lad.currency;
                        qu.Date = DateTime.UtcNow.AddHours(1);
                        qu.debit_currency = lad.debit_currency;
                        qu.fullname = Profile.Fullname;
                        qu.IDNUmber = Profile.IdNumber;
                        qu.narration = lad.narration;
                        qu.reference = lad.reference;
                        qu.uid = uid;
                        qu.QueueStatus = QueueStatus.Pending;

                        _context.TransferQueues.Add(qu);
                        await _context.SaveChangesAsync();

                        AhiaPayTransfer transfer = new AhiaPayTransfer();
                        transfer.Amount = amount;
                        transfer.DateOfTransfer = DateTime.UtcNow.AddHours(1);
                        transfer.Description = "AhiaPay Transfer to bank";
                        transfer.Sender = "Ahioma";
                        transfer.Status = Enums.TransferEnum.Processed;
                        transfer.TransferReference = guid;
                        transfer.UserId = user.Id;
                        _context.AhiaPayTransfers.Add(transfer);
                        await _context.SaveChangesAsync();


                        Wallet.WithdrawBalance -= amount;
                        Wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                        _context.Entry(Wallet).State = EntityState.Modified;
                        //wallet histiory
                        WalletHistory history = new WalletHistory();
                        history.Amount = amount;
                        history.CreationTime = DateTime.UtcNow.AddHours(1);
                        history.LedgerBalance = Wallet.Balance;
                        history.AvailableBalance = Wallet.WithdrawBalance;
                        history.WalletId = Wallet.Id;
                        history.UserId = Wallet.UserId;
                        history.UserProfileId = Profile.Id;
                        history.TransactionType = "Dr";
                        history.Source = "AhiaPay Transfer to Bank";
                        history.Destination = "AhiaPay Trx to Bank";
                        history.From = from;
                        _context.WalletHistories.Add(history);
                        await _context.SaveChangesAsync();

                        var newtransaction = await _transactionAppService.CreateTransaction(new Transaction
                        {
                            Amount = amount,
                            DateOfTransaction = DateTime.UtcNow.AddHours(1),
                            Status = EntityStatus.Processed,
                            TransactionType = TransactionTypeEnum.Debit,
                            Note = "AhiaPay Transfer to Bank",
                            UserId = user.Id,
                            TrackCode = guid,
                            WalletId = Wallet.Id,
                            TransactionShowEnum = TransactionShowEnum.Off,
                            TransactionSection = TransactionSection.Order,
                            Description = "AhiaPay Transfer to Bank",
                            From = from
                        });

                        var quen = await _context.TransferQueues.FirstOrDefaultAsync(x => x.Id == qu.Id);
                        quen.TransactionId = newtransaction.Id;
                        _context.Entry(quen).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        AddMessageDto smrs = new AddMessageDto();
                        smrs.Content = "T Banki. Processed with: " + Profile.IdNumber + " tID: " + newtransaction.Id + " amt: " + amount;
                        smrs.Recipient = "08165680904";
                        smrs.NotificationType = Enums.NotificationType.SMS;
                        smrs.NotificationStatus = Enums.NotificationStatus.NotSent;
                        smrs.Retries = 0;
                        smrs.Title = "Notify";
                        //
                        var stgss = await _message.AddMessage(smrs);

                        AddMessageDto emails = new AddMessageDto();
                        emails.Content = "T Banki. Processed with: " + Profile.IdNumber + " tID: " + newtransaction.Id + " amt: " + amount; ;
                        emails.Recipient = "onwukaemeka41@gmail.comAhioma";
                        emails.NotificationType = Enums.NotificationType.Email;
                        emails.NotificationStatus = Enums.NotificationStatus.NotSent;
                        emails.Retries = 0;
                        emails.Title = "xxxx Transfer to Bank alert";

                        var stsl = await _message.AddMessage(emails);

                        outputstring = "Transfer Processed Successfully";

                        //string email = user.Email;
                        //string phone = user.PhoneNumber;
                        //string Title = "Hi " + Profile.Surname;
                        //string SMS = "Your Ahia Pay Transfer.";
                        //string Subject = "Ahia Pay Bank Transfer";
                        //string Message = string.Format("<h4>Transfer to Your Bank from Ahia Pay was Successful. Account credited Immediately with {0}", amount);

                        ////await _emailSender.SendToOne(email, Subject, Title, Message);

                        string mSmsContent = "";
                        try
                        {
                            mSmsContent = await _message.GetMessage(Enums.ContentType.TransferToBankSms);
                        }
                        catch (Exception c) { }

                        //update content Data
                        mSmsContent = mSmsContent.Replace("|Amount|", amount.ToString());
                        string mmm = "";

                        mSmsContent = mSmsContent.Replace("|Desc|", "Transfer to Your Bank has been Processed Successfully.");
                        mSmsContent = mSmsContent.Replace("|Date|", newtransaction.DateOfTransaction.ToString("dd/MM/yyyy hh:mm tt"));

                        mSmsContent = mSmsContent.Replace("|||", "\r\n");
                        //sms
                        AddMessageDto sms = new AddMessageDto();
                        sms.Content = mSmsContent;
                        sms.Recipient = user.PhoneNumber.Replace(" ", "");
                        sms.NotificationType = Enums.NotificationType.SMS;
                        sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                        sms.Retries = 0;

                        sms.Title = mmm;
                        //
                        var stss = await _message.AddMessage(sms);

                        ////email
                        string mEmailContent = "";
                        try
                        {
                            mEmailContent = await _message.GetMessage(Enums.ContentType.TransferToBankEmail);
                        }
                        catch (Exception c) { }


                        ////update content Data
                        mEmailContent = mEmailContent.Replace("|Amount|", amount.ToString());
                        mEmailContent = mEmailContent.Replace("|Date|", newtransaction.DateOfTransaction.ToString("dd/MM/yyyy hh:mm tt"));
                        mEmailContent = mEmailContent.Replace("|Surname|", Profile.Surname);
                        mEmailContent = mEmailContent.Replace("|Description|", "Transfer to Your Bank has been Processed Successfully.");


                        AddMessageDto email = new AddMessageDto();
                        email.Content = mEmailContent;
                        email.Recipient = user.Email.Replace(" ", "");
                        email.NotificationType = Enums.NotificationType.Email;
                        email.NotificationStatus = Enums.NotificationStatus.NotSent;
                        email.Retries = 0;
                        email.Title = "Transfer of N" + amount + " to Bank";

                        var sts = await _message.AddMessage(email);



                        //await _emailSender.SMSToOne(phone, SMS);

                        return outputstring;

                    }
                    else
                    {
                        AhiaPayTransfer transfer = new AhiaPayTransfer();
                        transfer.Amount = amount;
                        transfer.DateOfTransfer = DateTime.UtcNow.AddHours(1);
                        transfer.Description = "AhiaPay Transfer";
                        transfer.Sender = "Ahioma";
                        transfer.Status = Enums.TransferEnum.Fail;
                        transfer.TransferReference = "";
                        transfer.UserId = user.Id;
                        _context.AhiaPayTransfers.Add(transfer);
                        await _context.SaveChangesAsync();
                        outputstring = "Transfer Not Successful, Kindly Update Account and try again";
                        return outputstring;
                    }
                }
                else
                {
                    AhiaPayTransfer transfer = new AhiaPayTransfer();
                    transfer.Amount = amount;
                    transfer.DateOfTransfer = DateTime.UtcNow.AddHours(1);
                    transfer.Description = "AhiaPay Transfer";
                    transfer.Sender = "Ahioma";
                    transfer.Status = Enums.TransferEnum.Fail;
                    transfer.TransferReference = "";
                    transfer.UserId = user.Id;
                    _context.AhiaPayTransfers.Add(transfer);
                    await _context.SaveChangesAsync();
                    outputstring = "Not Successful. Try again";
                    return outputstring;

                }
            }
            catch (Exception g)
            {
                return null;
            }
        }


        public async Task<string> MajorTransfer(long id)
        {
            string outputstring = "";
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transfers";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

                var ilad = await _context.TransferQueues.FirstOrDefaultAsync(x => x.Id == id);

                var lad = new TModel
                {
                    account_bank = ilad.account_bank,
                    account_number = ilad.account_number,
                    amount = ilad.amount,
                    narration = ilad.narration,
                    currency = ilad.currency,
                    reference = ilad.reference,
                    callback_url = ilad.callback_url,
                    debit_currency = ilad.debit_currency
                };

                request.AddJsonBody(lad);

                Thread.Sleep(2000);


                IRestResponse response = client.Execute(request);

                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<TransferModel>(contents);
                //return mainresponse;
              
                if (mainresponse != null)
                {

                    if (mainresponse.status == "success")
                    {
                    //    var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == ilad.Id);
                    //    transaction.Status = EntityStatus.Success;
                    //    _context.Entry(transaction).State = EntityState.Modified;
                    //    await _context.SaveChangesAsync();
                        return "success "+ mainresponse.data.id;
                    }

                }
                return null;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<FlutterTransactionVerify> VerifyTransaction(string tx_ref)
        {
            //          try{

            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transactions/" + tx_ref + "/verify";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<FlutterTransactionVerify>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }

        }


        public async Task<FlutterTransactionVerify> GetTransfer(string tx_ref)
        {
            //          try{

            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/transactions/" + tx_ref + "/verify";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<FlutterTransactionVerify>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }

        }

        public async Task<string> GetBills()
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/bill-categories";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");


                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                //var mainresponse = JsonConvert.DeserializeObject<FlutterTransactionVerify>(contents);
                //return mainresponse;
                return "";
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public async Task<ResponseRequestBill> CreateBill(RequestBill model)
        {
            try
            {
                string apiurl = $"https://api.flutterwave.com/v3/accounts/resolve";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", "FLWSECK-3ebc176be7413ec684592804c5cd98b7-X");

                var lad = new RequestBill
                {
                    country = "NG",
                    customer = model.customer,
                    amount = model.amount,
                    recurrence = "ONCE",
                    type = model.type,
                    reference = model.reference
                };

                request.AddJsonBody(lad);
                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                //var json = await response.Content.ReadAsStringAsync();

                var mainresponse = JsonConvert.DeserializeObject<ResponseRequestBill>(contents);
                return mainresponse;
            }
            catch (Exception g)
            {
                return null;
            }
        }
    }
}
