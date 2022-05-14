using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
   
    public class CheckModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public CheckModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task OnGetAsync()
        {
            try
            {


                var profileupdate = await _context.UserProfiles.ToListAsync();
                foreach(var i in profileupdate) { 
                
                    i.IdNumber = i.Id.ToString("0000000");
                    _context.Attach(i).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                TempData["succ"] = "complete";
            }
            catch (Exception s)
            {

            }
        }
    }
}
