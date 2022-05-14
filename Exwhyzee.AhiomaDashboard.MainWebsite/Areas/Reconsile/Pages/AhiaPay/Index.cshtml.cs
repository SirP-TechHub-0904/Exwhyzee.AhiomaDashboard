using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Reconsile.Pages.AhiaPay
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin")]

    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public IndexModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }


       // public PaginatedList<WalletDto> Wallets { get; set; }
        public IList<WalletDto> Wallets { get; set; }

        public async Task OnGetAsync(string currentFilter, string searchString, int? pageIndex)
        {
            CurrentFilter = searchString;

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            //IQueryable<Wallet> WalletIQ = from s in _context.Wallets

            //                                select s;
            var WalletIQ = await _context.Wallets.ToListAsync();

            Wallets = WalletIQ.Select(x => new WalletDto
            {
                UserId = x.UserId,
                DateUpdated = x.LastUpdateTime,
                Fullname = NameWallet(x.UserId),
                IdNumber = IDNameWallet(x.UserId),
                WithdrawBalance = x.WithdrawBalance,
                LedgerBalance = x.Balance

            }).ToList();

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    IWallets = IWallets.Where(s => (s.IdNumber != null) && s.IdNumber.ToUpper().Contains(searchString.ToUpper())
            //           || (s.WithdrawBalance.ToString().ToUpper().Contains(searchString.ToUpper())
            //           || (s.LedgerBalance.ToString().ToUpper().Contains(searchString.ToUpper())
            //           || (s.DateUpdated != null) && s.DateUpdated.ToString().ToUpper().Contains(searchString.ToUpper()))));
                      
            //}
            //int pageSize = 50;
            //Wallets = await PaginatedList<WalletDto>.CreateAsync(
            //    IWallets, pageIndex ?? 1, pageSize);
        }

        private string NameWallet(string id)
        {
            try
            {
                var userprofile = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
                return userprofile.Fullname;
            }
            catch (Exception k)
            {
                return "";
            }
        }
        private string IDNameWallet(string id)
        {
            try
            {
                var userprofile = _context.UserProfiles.FirstOrDefault(x => x.UserId == id);
                return userprofile.IdNumber;
            }
            catch (Exception k)
            {
                return "";
            }
        }
    }
}
