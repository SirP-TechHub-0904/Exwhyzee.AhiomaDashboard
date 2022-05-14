using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    public class BlogModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string customerRef, long? id, long tid, string des, string name, int? pageIndex)
        {


            return Page();
        }
    }

    //public class Blog
    //{
    //    public int Id { get; set; }
    //    public string Title { get; set; }
    //    public string ShortDescription { get; set; }
    //    public string Description { get; set; }
    //    public bool Publish { get; set; }
    //    public DateTime Date { get; set; }
    //    public string ImageUrl { get; set; }
    //    public int CategoryId { get; set; }
    //    public Category Category { get; set; }
    //}

    //public class BlogCategory
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string ShortDescription { get; set; }
    //    public bool Publish { get; set; }
    //    public DateTime Date { get; set; }
    //    public ICollection<ModelBlogDto> Blogs { get; set; }
    //}
}
