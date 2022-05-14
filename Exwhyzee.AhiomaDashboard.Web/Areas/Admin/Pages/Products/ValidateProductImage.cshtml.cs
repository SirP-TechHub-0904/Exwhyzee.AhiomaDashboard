using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Exwhyzee.Enums;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Http;
using Exwhyzee.AhiomaDashboard.Web.Services;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Admin,SOA,Store,mSuperAdmin,Customer,Editor")]

    public class ValidateProductImageModel : PageModel
    {

        private readonly IProductRepository _product;
        private readonly IPictureService _pictureservice;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICategoryRepository _category;
        private readonly IUserLogging _log;
        private readonly IEmailSendService _emailSender;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IUserProfileRepository _account;
        public ValidateProductImageModel(IPictureService pictureservice, IEmailSendService emailSender, IUserLogging log, IProductRepository product, IUserProfileRepository account, UserManager<IdentityUser> userManager, IHostingEnvironment hostingEnv, AhiomaDbContext context, ICategoryRepository category)
        {
            _context = context;
            _product = product;
            _pictureservice = pictureservice;
            _category = category;
            _hostingEnv = hostingEnv;
            _emailSender = emailSender;
            _userManager = userManager;
            _account = account;
            _log = log;
        }


        //public async Task<IActionResult> OnGet()
        //{
        //    IQueryable<Product> pic = from s in _context.Products
        //                                        .Where(ValidImage())
        //                                     select s;

        //    return Page();
        //}



        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://aka.ms/RazorPagesCRUD.
        //public async Task<IActionResult> OnPostAsync()
        //{


        //    return Page();
        //}

        //public string ValidImage(long id)
        //{
        //    string imgpath = "noimage";
        //    try
        //    {
        //        var pic = _context.ProductPictures.FirstOrDefault(x => x.ProductId == id);


        //        string webRootPath = _hostingEnv.WebRootPath;
        //        var fullPaththum = webRootPath + pic.PictureUrlThumbnail;
        //        if (System.IO.File.Exists(fullPaththum))
        //        {
        //            imgpath = pic.PictureUrlThumbnail;

        //        }

        //        return imgpath;
        //    }
        //    catch (Exception c)
        //    {
        //        return imgpath;
        //    }
        //}
    }
}
