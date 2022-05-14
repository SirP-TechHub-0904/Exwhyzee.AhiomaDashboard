using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Drawing.Printing;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.Users
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare,SubAdmin")]

    public class EditModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public EditModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public UserProfile UserProfile { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Phone { get; set; }
        [BindProperty]
        public bool ConfirmPhone { get; set; }
        [BindProperty]
        public bool ConfirmEmail { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           
            UserProfile = await _context.UserProfiles.Include(x=>x.User).FirstOrDefaultAsync(m => m.Id == id);

            if (UserProfile == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
           
            try
            {
                var user = await _userManager.FindByIdAsync(UserProfile.UserId);
                try
                {
                    var code = await _userManager.GenerateChangeEmailTokenAsync(user, Email);
                    var chmail = await _userManager.ChangeEmailAsync(user, Email, code);
                }catch(Exception c)
                {
                    TempData["mail"] = "unable to change mail";
                }
                try
                {
                    var codep = await _userManager.GenerateChangePhoneNumberTokenAsync(user, Email);
                    var chmail = await _userManager.ChangePhoneNumberAsync(user, Phone, codep);
                }
                catch (Exception c)
                {
                    TempData["mail"] = "unable to change phone number";
                }
                user.EmailConfirmed = ConfirmEmail;
                user.PhoneNumberConfirmed = ConfirmPhone;
                await _userManager.UpdateAsync(user);
            }
            catch (Exception s)
            {

            }
            //var userinfo = await _context.UserProfiles.FirstOrDefaultAsync(x => x.Id == UserProfile.Id);


            //_context.Attach(UserProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProfileExists(UserProfile.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserProfileExists(long id)
        {
            return _context.UserProfiles.Any(e => e.Id == id);
        }
    }
}
