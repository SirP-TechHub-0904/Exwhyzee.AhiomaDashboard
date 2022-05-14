using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageProductApiController : ControllerBase
    {

       
        private readonly IPictureService _pictureservice;

        public ManageProductApiController(IPictureService pictureservice)
        {
           
            _pictureservice = pictureservice;
        }

        [HttpPost]
        [Consumes("application/json")]
        public IActionResult PostJson(IEnumerable<int> values) =>
           Ok(new { Consumes = "application/json", Values = values });

        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult PostForm([FromForm] IEnumerable<int> values) =>
            Ok(new { Consumes = "application/x-www-form-urlencoded", Values = values });


        [HttpPost]
        [Route("Pageing")]
        public async Task<IActionResult> Pageing()
        {
            return Ok("sucess");
        }
            [HttpPost]
        [Route("ThreeImageUpload")]
        public async Task<IActionResult> ThreeImageUpload(ImageUpload objfile)
        {

            if (objfile.fileFront != null)
            {
                try
                {
                    await _pictureservice.InsertPicture(objfile.fileFront, "image/jpeg", objfile.ProductId, "www.ahioma.com", "jpeg");

                }
                catch (Exception ex)
                {

                }
            }
            return Ok("sucess");
        }
    }

   
    public class ImageUpload
    {
        public byte[] fileFront { get; set; }
        public byte[] fileBack { get; set; }
        public byte[] fileSide { get; set; }
        public long ProductId { get; set; }
    }
}
