using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.Transactions
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]
    public class IndexModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserProfileRepository _account;

        public IndexModel(AhiomaDbContext context,
            UserManager<IdentityUser> userManager
, IUserProfileRepository account)
        {
            _context = context;
            _userManager = userManager;
            _account = account;
        }
        public PaginatedList<Transaction> Transaction { get; set; }
        public int CountString(string searchString)
        {
            int result = 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();

                if (searchString == "")
                    return 0;

                while (searchString.Contains("  "))
                    searchString = searchString.Replace("  ", " ");

                foreach (string y in searchString.Split(' '))

                    result++;

            }
            return result;
        }

        public int Count { get; set; }

      //  public int PageSize { get; set; }
        public int AllTransaction { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int? CurrentPage { get; set; }
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;
        public async Task<IActionResult> OnGetAsync(string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {


            CurrentFilter = searchString;
            CurrentSort = sortOrder;


            CurrentFilter = searchString;
            CurrentSort = sortOrder;

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            //Transaction = await _context.Transactions.OrderByDescending(x => x.DateOfTransaction).Where(x => x.TransactionShowEnum != Enums.TransactionShowEnum.Off).ToListAsync();

            IQueryable<Transaction> iTransaction = from s in _context.Transactions
                                                    .Include(x=>x.UserProfile)
                                           .OrderByDescending(x => x.DateOfTransaction).Where(x => x.TransactionShowEnum != Enums.TransactionShowEnum.Off)
                                                    select s;

            //var iTransaction = xiTransaction.Select(x => new TransactionListDto
            //{
            //    Id = x.Id,
            //    DateOfTransaction = x.DateOfTransaction,
            //    Fullname = x.UserProfile.Fullname,
            //    IdNumber = x.UserProfile.IdNumber,
            //    Amount = x.Amount,
            //    TransactionType = x.TransactionType,
            //    Status = x.Status,
            //    Description = x.Description,
            //    TrackCode = x.TrackCode
            //});


            AllTransaction = iTransaction.Count();

            if (!String.IsNullOrEmpty(searchString))
            {

              
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');

                    IQueryable<Transaction> listProduct = Enumerable.Empty<Transaction>().AsQueryable();
                    List<Transaction> alist = new List<Transaction>();
                    foreach (var item in searchStringCollection)
                    {
                        iTransaction = iTransaction.Where(s => (s.UserProfile.Fullname != null) && s.UserProfile.Fullname.ToUpper().Contains(item.ToUpper())
                        || (s.Amount.ToString().ToUpper().Contains(item.ToUpper())
                        || (s.UserProfile.IdNumber != null) && s.UserProfile.IdNumber.ToUpper().Contains(item.ToUpper())
                        || (s.Description != null) && s.Description.ToUpper().Contains(item.ToUpper())
                        || (s.TrackCode != null) && s.TrackCode.ToUpper().Contains(item.ToUpper())
                        )
                        );
                        var li = iTransaction.ToList();
                        alist.AddRange(li);
                    }

                }
                else
                {
                    iTransaction = iTransaction.Where(s => (s.UserProfile.Fullname != null) && s.UserProfile.Fullname.ToUpper().Contains(searchString.ToUpper())
                        || (s.Amount.ToString().ToUpper().Contains(searchString.ToUpper())
                        || (s.UserProfile.IdNumber != null) && s.UserProfile.IdNumber.ToUpper().Contains(searchString.ToUpper())
                        || (s.Description != null) && s.Description.ToUpper().Contains(searchString.ToUpper())
                        || (s.TrackCode != null) && s.TrackCode.ToUpper().Contains(searchString.ToUpper())
                        )
                        );

                }
            }

          
           Count = iTransaction.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;
            Transaction = await PaginatedList<Transaction>.CreateAsync(
                iTransaction.AsNoTracking().AsQueryable(), pageIndex ?? 1, pageSize);

            CurrentPage = pageIndex ?? 1;


            return Page();
        }

    }

}