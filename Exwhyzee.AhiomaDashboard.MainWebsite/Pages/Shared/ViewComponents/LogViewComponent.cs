using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class LogViewComponent : ViewComponent
    {
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LogViewComponent(AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            //string url = HttpContext.Current.Request.Url.AbsoluteUri;
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var item = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                string ip = HttpContext.Connection.RemoteIpAddress.ToString();
                string mc = GetMacAddress();
                item.Note = item.Note + "<br>log:" + DateTime.UtcNow + " <ip> " + ip + " <mc> " + mc;
                _context.Attach(item).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            return View();
        }
        private string GetMacAddress()
        {
            string macAddresses = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddresses;
        }

        //public String getDeviceIMEI()
        //{
        //    String deviceUniqueIdentifier = null;
        //    TelephonyManager tm = (TelephonyManager)this.getSystemService(Context.TELEPHONY_SERVICE);
        //    if (null != tm)
        //    {
        //        deviceUniqueIdentifier = tm.getDeviceId();
        //    }
        //    if (null == deviceUniqueIdentifier || 0 == deviceUniqueIdentifier.length())
        //    {
        //        deviceUniqueIdentifier = Settings.Secure.getString(this.getContentResolver(), Settings.Secure.ANDROID_ID);
        //    }
        //    return deviceUniqueIdentifier;
        //}


    }

}
