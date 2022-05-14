using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    [AllowAnonymous]

    public class MarketDetailsModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        public MarketDetailsModel(
            AhiomaDbContext context)
        {

            _context = context;
        }
        //asp-route-id="@data.Id" asp-route-name="@data.Name.Replace(" ", "-")"
        //    asp-route-mktstate="@data.State.Replace(" ", "-")"
        //    asp-route-mktaddress="@data.Address.Replace(" ", "-")" 
        [BindProperty]
        public string CustomerRef { get; set; }
        public Market Market { get; set; }
        public IList<Tenant> Tenants { get; set; }
        public async Task OnGetAsync(string customerRef, long id, string name, string mktstate, string mktaddress)
        {
            CustomerRef = customerRef;
            Market = await _context.Markets.FirstOrDefaultAsync(x => x.Id == id);
            Tenants = await _context.Tenants.Include(x=>x.TenantAddresses).Include(x=>x.Categories).Where(x => x.MarketId == id).ToListAsync();
        }


    }
}
