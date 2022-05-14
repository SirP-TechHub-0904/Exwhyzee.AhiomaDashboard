using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Transactions
{
    public class TransactionRepository : ITransactionRepository
    {

        private readonly AhiomaDbContext _context;
        private readonly IMessageRepository _message;

        public TransactionRepository(AhiomaDbContext context, IMessageRepository message)
        {
            _context = context;
            _message = message;
        }
        public Task Delete(long? id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Transaction>> GetAsyncAll()
        {
            var data = await _context.Transactions.ToListAsync();
            return data;
        }

        public async Task<List<Transaction>> GetAsyncAllById(string UserId)
        {
            var data = await _context.Transactions.Where(x => x.UserId == UserId).ToListAsync();
            return data;
        }
        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            var data = new Transaction
            {
                Amount = transaction.Amount,
                DateOfTransaction = transaction.DateOfTransaction,
                Status = transaction.Status,
                TransactionType = transaction.TransactionType,
                UserId = transaction.UserId,
                WalletId = transaction.WalletId,
                TrackCode = transaction.TrackCode,
                TransactionReference = transaction.TransactionReference,
                OrderItemId = transaction.OrderItemId,
                TransactionSection = transaction.TransactionSection,
                PayoutStatus = transaction.PayoutStatus,
                TransactionShowEnum = transaction.TransactionShowEnum,
                Description = transaction.Description

            };
            var profilr = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == transaction.UserId);
            data.UserProfileId = profilr.Id;
            _context.Transactions.Add(data);
            await _context.SaveChangesAsync();
            //AddMessageDto sms = new AddMessageDto();
            //sms.Content = "Transaction Activity from " + profilr.Fullname + " with: " + profilr.IdNumber + " tID: " + data.Id + " type: " + transaction.TransactionType + " desc: " + transaction.Description;
            //sms.Recipient = "onwukaemeka41@gmail.comAhioma";
            //sms.NotificationType = Enums.NotificationType.Email;
            //sms.NotificationStatus = Enums.NotificationStatus.NotSent;
            //sms.Retries = 0;
            //sms.Title = "Transaction Activity from " + profilr.Fullname;
            ////
            //var stss = await _message.AddMessage(sms);
            return data;

        }
        public async Task<List<Transaction>> GetAsyncAllByUserId(string UserId)
        {
            var data = await _context.Transactions.Where(x => x.UserId == UserId).OrderByDescending(x => x.DateOfTransaction).ToListAsync();
            return data;
        }

        public async Task<Transaction> GetById(long? id)
        {
            var data = await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<long> Insert(Transaction model)
        {
            var profilr = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == model.UserId);
            model.UserProfileId = profilr.Id;
            _context.Transactions.Add(model);
            await _context.SaveChangesAsync();
            //AddMessageDto sms = new AddMessageDto();
            //sms.Content = "Transaction Activity from " + profilr.Fullname + " with: " + profilr.IdNumber + " tID: " + model.Id + " type: " + model.TransactionType + " desc: " + model.Description;
            //sms.Recipient = "onwukaemeka41@gmail.comAhioma";
            //sms.NotificationType = Enums.NotificationType.Email;
            //sms.NotificationStatus = Enums.NotificationStatus.NotSent;
            //sms.Retries = 0;
            //sms.Title = "Transaction Activity from " + profilr.Fullname;
            ////
            //var stss = await _message.AddMessage(sms);
            return model.Id;
        }

        public async Task<Transaction> Update(Transaction model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<long> CreateCreditTransactionAndUpdateWallet(Transaction model, string UserId, string tranxRef)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == UserId);
            var profilr = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == UserId);


            model.WalletId = wallet.Id;
            model.Status = Enums.EntityStatus.Success;
            model.TransactionType = Enums.TransactionTypeEnum.Credit;
            model.TransactionReference = tranxRef;
            model.TransactionSection = Enums.TransactionSection.Transaction;
            model.DateOfTransaction = DateTime.UtcNow.AddHours(1);
            model.UserProfileId = profilr.Id;
            _context.Transactions.Add(model);


            wallet.Balance += model.Amount;
            wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
            _context.Attach(wallet).State = EntityState.Modified;
            //wallet histiory
            WalletHistory adcushistory = new WalletHistory();
            adcushistory.Amount = model.Amount;
            adcushistory.CreationTime = DateTime.UtcNow.AddHours(1);
            adcushistory.LedgerBalance = wallet.Balance;
            adcushistory.AvailableBalance = wallet.WithdrawBalance;
            adcushistory.WalletId = wallet.Id;
            adcushistory.UserId = wallet.UserId;
            adcushistory.UserProfileId = profilr.Id;
            adcushistory.TransactionType = "Cr";
            adcushistory.Source = "Reference " + tranxRef;
            _context.WalletHistories.Add(adcushistory);

            await _context.SaveChangesAsync();
            //AddMessageDto sms = new AddMessageDto();
            //sms.Content = "Transaction and Wa Add " + model.Amount + " Activity with: " + profilr.IdNumber + " tID: " + model.Id;
            //sms.Recipient = "onwukaemeka41@gmail.comAhioma";
            //sms.NotificationType = Enums.NotificationType.Email;
            //sms.NotificationStatus = Enums.NotificationStatus.NotSent;
            //sms.Retries = 0;
            //sms.Title = "Notify";
            ////
            //var stss = await _message.AddMessage(sms);
            return model.Id;
        }

        public async Task<long> CreateDebitTransactionAndUpdateWallet(Transaction model, string UserId, string tranxRef)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == UserId);
            var profilr = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == UserId);


            model.WalletId = wallet.Id;
            model.Status = Enums.EntityStatus.Success;
            model.TransactionType = Enums.TransactionTypeEnum.Debit;
            model.TransactionReference = tranxRef;
            model.TransactionSection = Enums.TransactionSection.Transaction;
            model.DateOfTransaction = DateTime.UtcNow.AddHours(1);
            model.UserProfileId = profilr.Id;
            _context.Transactions.Add(model);

            wallet.Balance -= model.Amount;
            wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);
            _context.Attach(wallet).State = EntityState.Modified;

            //wallet histiory
            WalletHistory adcushistory = new WalletHistory();
            adcushistory.Amount = model.Amount;
            adcushistory.CreationTime = DateTime.UtcNow.AddHours(1);
            adcushistory.LedgerBalance = wallet.Balance;
            adcushistory.AvailableBalance = wallet.WithdrawBalance;
            adcushistory.WalletId = wallet.Id;
            adcushistory.UserId = wallet.UserId;
            adcushistory.UserProfileId = profilr.Id;
            adcushistory.TransactionType = "Dr";
            adcushistory.Source = "Reference " + tranxRef;
            _context.WalletHistories.Add(adcushistory);

            await _context.SaveChangesAsync();

            //AddMessageDto sms = new AddMessageDto();
            //sms.Content = "Transaction and Wa minus " + model.Amount + " Activity with: " + profilr.IdNumber + " tID: " + model.Id;
            //sms.Recipient = "onwukaemeka41@gmail.comAhioma";
            //sms.NotificationType = Enums.NotificationType.Email;
            //sms.NotificationStatus = Enums.NotificationStatus.NotSent;
            //sms.Retries = 0;
            //sms.Title = "Notify";
            ////
            //var stss = await _message.AddMessage(sms);
            return model.Id;
        }
    }
}
