using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    public class DataModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

//#region image upload

//string profileimage = "";
//string IDcardfront = "";
//string IDcardBack = "";
//try
//{
//    var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

//    var url = location.AbsoluteUri;
//    var urlPath = location.Authority;
//    #region product Image(s)
//    int imgCount = 0;
//    if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count > 0)
//    {
//        var newFileName = string.Empty;
//        var newFileNameThumbnail = string.Empty;
//        var filePath = string.Empty;
//        var filePathThumbnail = string.Empty;
//        string pathdb = string.Empty;
//        var files = HttpContext.Request.Form.Files;
//        foreach (var file in files)
//        {
//            try
//            {
//                Image image = Image.FromStream(file.OpenReadStream(), true, true);
//                var newImage = new Bitmap(175, 175);
//            }
//            catch (ArgumentException aex)
//            {
//                //TempData["error"] = "The image was not uploaded is not from a valid source. kindly resave with paint and continue";

//                //return RedirectToPage("./Edit", new { id = id });

//            }
//            if (file.Length > 0)
//            {
//                filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
//                filePathThumbnail = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
//                imgCount++;
//                var now = DateTime.Now;
//                string nameproduct = file.Name + "-" + user.Email.Replace("@", "-").Replace(".", "-");
//                var uniqueFileName = $"{now.Millisecond}{now.Minute}{now.Second}{now.Day}-".Trim() + nameproduct;

//                var fileExtension = Path.GetExtension(filePath);

//                newFileName = uniqueFileName + fileExtension;

//                // if you wish to save file path to db use this filepath variable + newFileName
//                var fileDbPathName = $"/ProfileData/".Trim();

//                filePath = $"{_hostingEnv.WebRootPath}{fileDbPathName}".Trim();

//                if (!(Directory.Exists(filePath)))
//                    Directory.CreateDirectory(filePath);

//                var fileName = "";
//                fileName = filePath + $"{newFileName}".Trim();

//                //using (FileStream fsa = System.IO.File.Create(fileName))
//                //{
//                //    file.CopyTo(fsa);
//                //    fsa.Flush();
//                //}

//                Image image = Image.FromStream(file.OpenReadStream(), true, true);

//                Bitmap sourceImage = new Bitmap(image);
//                using (Bitmap objBitmap = new Bitmap(175, 175))
//                {
//                    objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
//                    using (Graphics objGraphics = Graphics.FromImage(objBitmap))
//                    {
//                        // Set the graphic format for better result cropping   
//                        objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

//                        objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
//                        objGraphics.DrawImage(sourceImage, 0, 0, 175, 175);

//                        // Save the file path, note we use png format to support png file   
//                        objBitmap.Save(fileName, ImageFormat.Jpeg);
//                    }
//                }

//                if (file.Name == "filesPhoto")
//                {
//                    profileimage = $"{fileDbPathName}{newFileName}";
//                }
//                else if (file.Name == "filesIDFront")
//                {
//                    IDcardfront = $"{fileDbPathName}{newFileName}";
//                }
//                else if (file.Name == "filesIDBack")
//                {
//                    IDcardBack = $"{fileDbPathName}{newFileName}";
//                }

//                #region Save Image Propertie to Db

//                #endregion

//                if (imgCount >= 5)
//                    break;
//            }
//        }
//    }
//    #endregion


//}
//catch (Exception c)
//{

//}
//profile.ProfileUrl = profileimage;
//profile.IDCardFront = IDcardfront;
//profile.IDCardBack = IDcardBack;
//#endregion








//#region login old

//@page
//@model LoginModel

//@{
//    ViewData["Title"] = "Log in";
//    Layout = "/Pages/Shared/_LoginLayout.cshtml";
//}
//<div id="reg-ahioma" class="position-absolute top-0 right-0 text-right mt-5 mb-15 mb-lg-0 flex-column-auto justify-content-center py-5 px-10">
//    @*<span class="font-weight-bold text-dark-50">Dont have an account yet?</span>
//    <a asp-page="./Register" asp-route-customerRef="" asp-route-returnUrl="@Model.ReturnUrl" class="font-weight-bold ml-2" id="kt_login_signup">Register!</a>*@
//</div>


//<form id="account" class="col-sm-6 form" method="post">
//    <input asp-for="CustomerRef" type="hidden" />
//    <div class="panel">
//        <div class="panel-header">
//            <h2 class="panel-title">Sign in</h2>
//        </div>
//        <div class="panel-body">


//            <div asp-validation-summary="All" class="text-danger"></div>
//            <div class="form-group">
//                <label asp-for="Input.Email"></label>
//                <input asp-for="Input.Email" class="form-control" />
//                <span asp-validation-for="Input.Email" class="text-danger"></span>
//            </div>
//            <div class="form-group">
//                <label asp-for="Input.Password"></label>
//                <input asp-for="Input.Password" class="form-control" />
//                <span asp-validation-for="Input.Password" class="text-danger"></span>
//            </div>
//            <div class="form-group">
//                <div class="">
//                    <label>
//                        Remember Me
//                    </label>
//                    <input asp-for="Input.RememberMe" />
//                </div>
//            </div>
//            <div class="form-group">
//                <button type="submit" class="btn btn-primary">Log in</button>
//            </div>


//            <div class="hidden-md hidden-lg form-group">
//                <p>
//                    Dont have an account yet? <a asp-page="./Register" asp-route-customerRef="" asp-route-returnUrl="@Model.ReturnUrl">Register!</a>
//                </p>

//            </div>
//            <div class="form-group">
//                <p>
//                    <a asp-area="Identity" asp-page="/Account/ForgotPassword">Forgot your password?</a>
//                </p>

//            </div>
//            <div style="text-align:center;display:none;" id="browserWarning">
//                <btn class="btn btn-light-danger">Notice:for 100% effeciency, Please use Chrome or Safari.</btn><br><br>
//            </div>
//        </div>


//    </div>
//</form>



//@section Scripts {
//    <partial name="_ValidationScriptsPartial" />
//}



//#endregion