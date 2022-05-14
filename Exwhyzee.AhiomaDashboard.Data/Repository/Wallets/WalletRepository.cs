using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Wallets
{
    public class WalletRepository : IWalletRepository
    {
        private readonly IMessageRepository _message;

        private readonly AhiomaDbContext _context;

        public WalletRepository(AhiomaDbContext context, IMessageRepository message)
        {
            _context = context;
            _message = message;
        }

        public async Task<long> CreateAhiaPay(AhiapayDto ahiapay)
        {
            try
            {
                var receiverwalletid = await GetWallet(ahiapay.ReceiverId);
                receiverwalletid.WithdrawBalance += ahiapay.Amount;
                receiverwalletid.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                //
                var profilere = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == receiverwalletid.UserId);



                var senderwalletid = await GetWallet(ahiapay.Sender);
                senderwalletid.WithdrawBalance -= ahiapay.Amount;
                senderwalletid.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                //
                var profilesend = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == senderwalletid.UserId);

                var transactionDataSender = new Transaction
                {
                    WalletId = ahiapay.Senderwalletid,
                    UserId = ahiapay.Sender,
                    Amount = ahiapay.Amount,
                    TrackCode = ahiapay.TransactionCode,
                    TransactionType = Enums.TransactionTypeEnum.TransferDebit,
                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
                    Status = Enums.EntityStatus.Success,
                    TransactionSection = Enums.TransactionSection.Transfer,
                    TransactionReference = ahiapay.Sender,
                    Description = ahiapay.Note + " (AhiaPay Trx to " + profilere.FirstName + ")",
                    From = ahiapay.From,
                    UserProfileId = profilesend.Id
                };
                var transactionDataReceiver = new Transaction
                {
                    WalletId = ahiapay.Receiverwalletid,
                    UserId = ahiapay.ReceiverId,
                    Amount = ahiapay.Amount,
                    TrackCode = ahiapay.TransactionCode,
                    TransactionType = Enums.TransactionTypeEnum.TransferCredit,
                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
                    Status = Enums.EntityStatus.Success,
                    TransactionSection = Enums.TransactionSection.Transfer,
                    TransactionReference = ahiapay.Sender,
                    Description = ahiapay.Note + " (AhiaPay Trx fr " + profilesend.Fullname + ")",
                    From = ahiapay.From,
                    UserProfileId = profilere.Id

                };
               
                WalletHistory history = new WalletHistory();
                history.Amount = ahiapay.Amount;
                history.CreationTime = DateTime.UtcNow.AddHours(1);
                history.LedgerBalance = senderwalletid.Balance;
                history.AvailableBalance = senderwalletid.WithdrawBalance;
                history.WalletId = senderwalletid.Id;
                history.UserId = senderwalletid.UserId;
                history.UserProfileId = profilesend.Id;
                history.Destination = ahiapay.Note;
                history.TransactionType = "Dr";
                history.From = ahiapay.From;

                history.Source = "AhiaPay Trx to "+ profilere.Fullname;
                _context.WalletHistories.Add(history);


               
                WalletHistory history2 = new WalletHistory();
                history2.Amount = ahiapay.Amount;
                history2.CreationTime = DateTime.UtcNow.AddHours(1);
                history2.LedgerBalance = receiverwalletid.Balance;
                history2.AvailableBalance = receiverwalletid.WithdrawBalance;
                history2.WalletId = receiverwalletid.Id;
                history2.UserId = receiverwalletid.UserId;
                history2.UserProfileId = profilere.Id;
                history2.Destination = ahiapay.Note;
                history2.TransactionType = "Cr";
                history2.From = ahiapay.From;
                history2.Source = "AhiaPay Trx frm "+profilesend.Fullname;
                _context.WalletHistories.Add(history2);


                _context.Transactions.Add(transactionDataReceiver);
                 _context.Transactions.Add(transactionDataSender);
                _context.Entry(senderwalletid).State = EntityState.Modified;
               
                await _context.SaveChangesAsync();
                _context.Entry(receiverwalletid).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();

                //AddMessageDto sms = new AddMessageDto();
                //sms.Content = "Wallet Activity: AhiaPay transfer to " +profilere.Fullname +" from "+profilesend.Fullname;
                //sms.Recipient = "onwukaemeka41@gmail.comAhioma";
                //sms.NotificationType = Enums.NotificationType.Email;
                //sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                //sms.Retries = 0;
                //sms.Title = "Notify";
                ////
                //var stss = await _message.AddMessage(sms);
                try
                {
                    
                    return 1;
                }catch(Exception c)
                {
                    return 0;
                }
              

            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        public TransactionTypeEnum Stat { get; set; }
        public async Task<long> CreateAhiaPayAdmin(AhiapayAdminDto ahiapay)
        {
            try
            {
                var receiverwalletid = await GetWallet(ahiapay.ReceiverId);
                var profilere = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == receiverwalletid.UserId);


                if (ahiapay.AhiaPayType == "Dr")
                {
                    Stat = TransactionTypeEnum.TransferDebit;

                }
                else if (ahiapay.AhiaPayType == "Cr")
                {
                    Stat = TransactionTypeEnum.TransferCredit;
                }
                var transactionDataReceiver = new Transaction
                {
                    WalletId = ahiapay.Receiverwalletid,
                    UserId = ahiapay.ReceiverId,
                    Amount = ahiapay.Amount,
                    TrackCode = ahiapay.TransactionCode,
                    UserProfileId = profilere.Id,
                    TransactionType = Stat,
                    DateOfTransaction = DateTime.UtcNow.AddHours(1),
                    Status = Enums.EntityStatus.Success,
                    TransactionSection = Enums.TransactionSection.Transfer,
                    TransactionReference = "Ahioma",
                    Description = ahiapay.Note
                };




               
                if (ahiapay.AhiaPayType == "Dr")
                {
                    receiverwalletid.WithdrawBalance -= ahiapay.Amount;

                }
                else if (ahiapay.AhiaPayType == "Cr")
                {
                    receiverwalletid.WithdrawBalance += ahiapay.Amount;

                }
                receiverwalletid.LastUpdateTime = DateTime.UtcNow.AddHours(1);
                //

                WalletHistory history2 = new WalletHistory();
                history2.Amount = ahiapay.Amount;
                history2.CreationTime = DateTime.UtcNow.AddHours(1);
                history2.LedgerBalance = receiverwalletid.Balance;
                history2.AvailableBalance = receiverwalletid.WithdrawBalance;
                history2.WalletId = receiverwalletid.Id;
                history2.UserId = receiverwalletid.UserId;
                history2.UserProfileId = profilere.Id;
                if (ahiapay.AhiaPayType == "Dr")
                {
                    history2.TransactionType = "Cr";

                }
                else if (ahiapay.AhiaPayType == "Cr")
                {
                    history2.TransactionType = "Cr";

                }

                history2.Source = "AhiaPay";
                _context.WalletHistories.Add(history2);


                _context.Transactions.Add(transactionDataReceiver);

                _context.Entry(receiverwalletid).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                //AddMessageDto sms = new AddMessageDto();
                //sms.Content = "Wallet Activity: " + profilere.IdNumber;
                //sms.Recipient = "08165680904";
                //sms.NotificationType = Enums.NotificationType.SMS;
                //sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                //sms.Retries = 0;
                //sms.Title = "Notify";
                ////
                //var stss = await _message.AddMessage(sms);
                try
                {

                    return 1;
                }
                catch (Exception c)
                {
                    return 0;
                }


            }
            catch (Exception ex)
            {
            }
            return 0;
        }


        public async Task Delete(long? id)
        {
            var data = await _context.Wallets.FindAsync(id);
             _context.Wallets.Remove(data);
            //await _context.SaveChangesAsync();
        }

        public async Task<List<Wallet>> GetAsyncAll()
        {
            var data = await _context.Wallets.ToListAsync();
            return data;
        }

        public async Task<Wallet> GetById(long? id)
        {
            var data = await _context.Wallets.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<Wallet> GetWallet(string userId)
        {
            
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var data = await _context.Wallets.AsNoTracking().FirstOrDefaultAsync(x=>x.UserId == userId);

           

            return data;
        }

        public async Task<long> Insert(Wallet model)
        {
            _context.Wallets.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(Wallet model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


    }
}
