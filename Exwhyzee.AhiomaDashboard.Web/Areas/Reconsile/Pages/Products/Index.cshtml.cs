using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Reconsile.Pages.Products
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

      
        public PaginatedList<Product> Product { get;set; }

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
            IQueryable<Product> ProductIQ = from s in _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.SubCategory)
                .Include(p => p.Tenant)
                                            select s;
            
            if (!String.IsNullOrEmpty(searchString))
            {
                ProductIQ = ProductIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString.ToUpper())
                       || (s.FullDescription != null) && s.FullDescription.ToUpper().Contains(searchString.ToUpper())
                       || (s.ShortDescription != null) && s.ShortDescription.ToUpper().Contains(searchString.ToUpper())
                       || (s.MetaKeywords != null) && s.MetaKeywords.ToUpper().Contains(searchString.ToUpper())
                       || (s.Price.ToString() != null) && s.Price.ToString().ToUpper().Contains(searchString.ToUpper())
                        || (s.Manufacturer.Name != null) && s.Manufacturer.Name.ToUpper().Contains(searchString.ToUpper())
                        || (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(searchString.ToUpper())
                        || (s.Tenant.BusinessDescription != null) && s.Tenant.BusinessDescription.ToUpper().Contains(searchString.ToUpper())
                        || (s.Tenant.Market.Name != null) && s.Tenant.Market.Name.ToUpper().Contains(searchString.ToUpper())
                        || (s.ShortDescription != null) && s.ShortDescription.ToUpper().Contains(searchString.ToUpper())
                       || (s.Category.Name != null) && s.Category.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            Product = await PaginatedList<Product>.CreateAsync(
                ProductIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
