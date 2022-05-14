﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    public class SuccessModel : PageModel
    {
        [BindProperty]
        public string CustomerRef { get; set; }
        public void OnGet(string customerRef)
        {
            CustomerRef = customerRef;
        }
    }
}