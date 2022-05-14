using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    public class SiteModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public SiteModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
