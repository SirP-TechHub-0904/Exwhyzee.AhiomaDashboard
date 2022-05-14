using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Services
{
    public class UserLogging : IUserLogging
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public UserLogging(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task<string> LogData(string userid, string data1, string data2)
        {
            var info = GetMACAddress();
            string uid = "";
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userid);
            if(user == null)
            {
                var user23 = await _context.Users.FirstOrDefaultAsync(x => x.Id == userid);
                uid = user23.Id;
            }
            else
            {
                uid = user.Id;
            }
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == uid);
            profile.Logs = profile.Logs + "LOG>>> "+DateTime.UtcNow.AddHours(1)+  "<br>" + data1 +" " + data2 + " mac: "+ info +" user info "+userid;
            _context.Attach(profile).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return "user data";
        }
        public string GetMACAddress()
        {
            string mac_src = "";
            string macAddress = "";

            foreach (System.Net.NetworkInformation.NetworkInterface nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                {
                    mac_src += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            while (mac_src.Length < 12)
            {
                mac_src = mac_src.Insert(0, "0");
            }

            for (int i = 0; i < 11; i++)
            {
                if (0 == (i % 2))
                {
                    if (i == 10)
                    {
                        macAddress = macAddress.Insert(macAddress.Length, mac_src.Substring(i, 2));
                    }
                    else
                    {
                        macAddress = macAddress.Insert(macAddress.Length, mac_src.Substring(i, 2)) + "-";
                    }
                }
            }
            return macAddress;
        }

    }
}
