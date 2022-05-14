using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    /// <summary>
    /// 
    /// </summary>
    public class ProfileModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _profile;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IMessageRepository _message;

        public ProfileModel(IUserProfileRepository profile, RoleManager<IdentityRole> roleManager, AhiomaDbContext context,
            UserManager<IdentityUser> userManager,
             IHostingEnvironment hostingEnv
, IMessageRepository message)
        {
            _context = context;
            _profile = profile;
            _roleManager = roleManager;
            _userManager = userManager;
            _hostingEnv = hostingEnv;
            _message = message;
        }
        public IList<Tenant> Stores { get; set; }
        public UserProfile Profile { get; set; }
        public string LoggedInUser { get; set; }
        public bool UserBool { get; set; }
        public List<SelectListItem> StateListing { get; set; }


        [BindProperty]
        public UserAddress Address { get; set; }


        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            public string PPhoneNumber { get; set; }

            public string PSurname { get; set; }

            public string PFirstName { get; set; }

            public string POtherNames { get; set; }
            public DateTime PDOB { get; set; }
            public string PSecurityQuestion { get; set; }
            public string PSecurityAnswer { get; set; }
            public string State { get; set; }
            public string LGA { get; set; }


            [Display(Name = "Next of Kin")]
            public string PNextOfKin { get; set; }
            [Display(Name = "Next of Kin Phone Number")]
            public string PNextOfKinPhoneNumber { get; set; }



        }
        [BindProperty]

        public string ReturnUrl { get; set; }
        public async Task OnGetAsync(string returnUrl = null, string status = null)
        {
            if (status != null)
            {
                TempData["error"] = "Scroll Down and Update your delivery address and click on My Cart";
            }

            ReturnUrl = returnUrl;
            LoggedInUser = _userManager.GetUserId(HttpContext.User);

            Profile = await _profile.GetByUserId(LoggedInUser);

            var userdata = await _userManager.FindByIdAsync(LoggedInUser);
            UserBool = userdata.EmailConfirmed;
            try
            {
                string roles = await _profile.FetchUserRoles(userdata.Id);
                TempData["roles"] = roles.Replace("Customer, ", "");
            }

            catch (Exception c) { }

            var state = await _profile.GetStates();
            StateListing = state.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.StateName,
                                      Text = a.StateName
                                  }).ToList();

        }

        public async Task<IActionResult> OnPostUpdateProfile()
        {
            try
            {
                LoggedInUser = _userManager.GetUserId(HttpContext.User);
                var user = await _userManager.FindByIdAsync(LoggedInUser);
                string profileimage = "";
                string IDcardfront = "";
                string IDcardBack = "";
                try
                {


                    #region profile Image(s)
                    int imgCount = 0;
                    if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count > 0)
                    {
                        var newFileName = string.Empty;
                        var newFileNameThumbnail = string.Empty;
                        var filePath = string.Empty;
                        var filePathThumbnail = string.Empty;
                        string pathdb = string.Empty;
                        var files = HttpContext.Request.Form.Files;
                        foreach (var file in files)
                        {
                            try
                            {
                                Image image = Image.FromStream(file.OpenReadStream(), true, true);
                                var newImage = new Bitmap(175, 175);
                            }
                            catch (ArgumentException aex)
                            {
                                //TempData["error"] = "The image was not uploaded is not from a valid source. kindly resave with paint and continue";

                                //return RedirectToPage("./Edit", new { id = id });

                            }
                            if (file.Length > 0)
                            {
                                filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                filePathThumbnail = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                                imgCount++;
                                var now = DateTime.Now;
                                string nameproduct = file.Name + "-" + user.Email.Replace("@", "-").Replace(".", "-");
                                var uniqueFileName = $"{now.Millisecond}{now.Minute}{now.Second}{now.Day}-".Trim() + nameproduct;

                                var fileExtension = Path.GetExtension(filePath);

                                newFileName = uniqueFileName + fileExtension;

                                // if you wish to save file path to db use this filepath variable + newFileName
                                var fileDbPathName = $"/ProfileData/".Trim();

                                filePath = $"{_hostingEnv.WebRootPath}{fileDbPathName}".Trim();

                                if (!(Directory.Exists(filePath)))
                                    Directory.CreateDirectory(filePath);

                                var fileName = "";
                                fileName = filePath + $"{newFileName}".Trim();

                                //using (FileStream fsa = System.IO.File.Create(fileName))
                                //{
                                //    file.CopyTo(fsa);
                                //    fsa.Flush();
                                //}

                                Image image = Image.FromStream(file.OpenReadStream(), true, true);

                                Bitmap sourceImage = new Bitmap(image);
                                using (Bitmap objBitmap = new Bitmap(175, 300))
                                {
                                    objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                                    using (Graphics objGraphics = Graphics.FromImage(objBitmap))
                                    {
                                        // Set the graphic format for better result cropping   
                                        objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                                        objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                        objGraphics.DrawImage(sourceImage, 0, 0, 175, 300);

                                        // Save the file path, note we use png format to support png file   
                                        objBitmap.Save(fileName, ImageFormat.Jpeg);
                                    }
                                }

                                if (file.Name == "filesPhoto")
                                {
                                    profileimage = $"{fileDbPathName}{newFileName}";
                                }
                                else if (file.Name == "filesIDFront")
                                {
                                    IDcardfront = $"{fileDbPathName}{newFileName}";
                                }
                                else if (file.Name == "filesIDBack")
                                {
                                    IDcardBack = $"{fileDbPathName}{newFileName}";
                                }

                                #region Save Image Propertie to Db

                                #endregion

                                if (imgCount >= 5)
                                    break;
                            }
                        }
                    }
                    #endregion


                }
                catch (Exception c)
                {

                }

                //user.PhoneNumber = Input.PPhoneNumber;
                //await _userManager.UpdateAsync(user);

                var profile = await _profile.GetByUserId(user.Id);
                profile.ProfileUrl = profileimage;
                profile.IDCardFront = IDcardfront;
                profile.IDCardBack = IDcardBack;
                profile.Surname = Input.PSurname;
                profile.FirstName = Input.PFirstName;
                profile.OtherNames = Input.POtherNames;
                profile.NextOfKin = Input.PNextOfKin;
                profile.NextOfKinPhoneNumber = Input.PNextOfKinPhoneNumber;
                profile.DOB = Input.PDOB;
                profile.SecurityQuestion = Input.PSecurityQuestion;
                profile.SecurityAnswer = Input.PSecurityAnswer;
                profile.LastUserUpdated = DateTime.UtcNow.AddHours(1);
                await _profile.Update(profile);

                TempData["success"] = "Updated Successfully";

                AddMessageDto sms = new AddMessageDto();
                sms.Content = "Profile Updated by " + profile.Fullname + "<br>https://ahioma.com/UserManager/ManageAccounts/Details?uid=" + user.Id + " <br> Kindly Confirm the account and update profile";
                sms.Recipient = "onwukaemeka41@gmail.comAhioma";
                sms.NotificationType = Enums.NotificationType.Email;
                sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                sms.Retries = 0;
                sms.Title = "Profile Updated by " + profile.Fullname;
                //
                var stss = await _message.AddMessage(sms);

                AddMessageDto smsi = new AddMessageDto();
                smsi.Content = "Profile Updated by " + profile.Fullname + "<br>https://ahioma.com/UserManager/ManageAccounts/Details?uid=" + user.Id + " <br> Kindly Confirm the account and update profile";
                smsi.Recipient = "ahiomacs@gmail.com";
                smsi.NotificationType = Enums.NotificationType.Email;
                smsi.NotificationStatus = Enums.NotificationStatus.NotSent;
                smsi.Retries = 0;
                smsi.Title = "Profile Updated by " + profile.Fullname;
                //
                var stsss = await _message.AddMessage(smsi);
                return RedirectToPage();
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostAddAddress()
        {
            try
            {
                LoggedInUser = _userManager.GetUserId(HttpContext.User);
                var profiledata = await _profile.GetByUserId(LoggedInUser);
                Address.UserId = LoggedInUser;
                Address.UserProfileId = profiledata.Id;
                _context.UserAddresses.Add(Address);

                await _context.SaveChangesAsync();

                TempData["success"] = "Address Successfully Updated";
                return RedirectToPage();
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage();
            }
        }

        [BindProperty]
        public long DeleteId { get; set; }
        public async Task<IActionResult> OnPostRemoveAddress()
        {
            try
            {
                var data = await _context.UserAddresses.FindAsync(DeleteId);
                _context.UserAddresses.Remove(data);
                await _context.SaveChangesAsync();
                return RedirectToPage();
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage();
            }
        }

        [BindProperty]
        public long AddressId { get; set; }
        public async Task<IActionResult> OnPostChangeDefault()
        {
            try
            {

                var data = await _context.UserAddresses.FindAsync(AddressId);
                IQueryable<UserAddress> check = from s in _context.UserAddresses
                                            .Where(x => x.UserId == data.UserId && x.Default == true)

                                                select s;
                foreach (var i in check)
                {
                    i.Default = false;
                    _context.Entry(i).State = EntityState.Modified;

                }
                data.Default = true;

                _context.Entry(data).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToPage();
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage();
            }
        }
    }
}
