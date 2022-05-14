using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.IO;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.VehicleLogistic
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Logistic,mSuperAdmin")]

    public class CreateModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;
        private readonly IHostingEnvironment _hostingEnv;
        public CreateModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, IUserProfileRepository account, IHostingEnvironment hostingEnv)
        {
            _context = context;
            _account = account;
            _hostingEnv = hostingEnv;
        }
        [BindProperty]
        public long LogisticId { get; set; }
        public List<SelectListItem> StateListing { get; set; }

        public async Task<IActionResult> OnGet(long id)
        {
            LogisticId = id;
            var state = await _account.GetStates();
            StateListing = state.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.StateName,
                                      Text = a.StateName
                                  }).ToList();
            return Page();
        }

        [BindProperty]
        public LogisticVehicle LogisticVehicle { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            #region Image(s)
            int imgCount = 0;
            if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count > 0)
            {
                var newFileName = string.Empty;
                var filePath = string.Empty;
                string pathdb = string.Empty;
                var file = HttpContext.Request.Form.Files.FirstOrDefault();

                if (file.Length > 0)
                {
                    filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    imgCount++;
                    var now = DateTime.Now;

                    var fileExtension = Path.GetExtension(filePath);

                    newFileName = $"{now.Minute}{now.Month}{now.Day}-".Trim() + file.FileName;

                    // if you wish to save file path to db use this filepath variable + newFileName
                    var fileDbPathName = $"/Logistic/".Trim();

                    filePath = $"{_hostingEnv.WebRootPath}{fileDbPathName}".Trim();

                    if (!(Directory.Exists(filePath)))
                        Directory.CreateDirectory(filePath);

                    var fileName = "";
                    fileName = filePath + $"{newFileName}".Trim();


                    // copy the file to the desired location from the tempMemoryLocation of IFile and flush temp memory
                    using (FileStream fs = System.IO.File.Create(fileName))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }



                    #region Save Image Propertie to Db

                    LogisticVehicle.VehicleImage = $"{fileDbPathName}{newFileName}";

                };

                #endregion

            }

            #endregion
            LogisticVehicle.VehicleStatus = Enums.VehicleEnum.Disabled;
            _context.LogisticVehicle.Add(LogisticVehicle);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Dashboard/Vehicles", new { area = "Logistic"});
        }
    }
}
