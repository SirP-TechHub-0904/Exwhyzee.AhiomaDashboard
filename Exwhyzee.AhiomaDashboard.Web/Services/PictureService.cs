using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
//using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Services
{
    public class PictureService : IPictureService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly AhiomaDbContext _context;
        private readonly IProductRepository _product;
        private readonly IEmailSendService _emailSender;


        public PictureService(
          IWebHostEnvironment hostingEnvironment, AhiomaDbContext context, IEmailSendService emailSender, IProductRepository product)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _emailSender = emailSender;
            _product = product;

        }
        protected virtual string GetPictureLocalPath(string fileName)
        {

            var fileDbPath = $"/Products/".Trim();

            string filePath = $"{_hostingEnvironment.WebRootPath}{fileDbPath}".Trim();

            if (!(Directory.Exists(filePath)))
                Directory.CreateDirectory(filePath);

            return Path.Combine(filePath, fileName);
        }

        protected virtual string GetPictureLocalPathThumbnail(string fileName)
        {

            var fileDbPath = $"/Products/Thumbnail/".Trim();

            string filePath = $"{_hostingEnvironment.WebRootPath}{fileDbPath}".Trim();

            if (!(Directory.Exists(filePath)))
                Directory.CreateDirectory(filePath);

            return Path.Combine(filePath, fileName);
        }


        protected virtual byte[] LoadPictureFromFile(string fileName)
        {
            //string lastPart = GetFileExtensionFromMimeType(mimeType);
            //string fileName = string.Format("{0}_0.{1}", pictureId, lastPart);
            var filePath = GetPictureLocalPath(fileName);
            if (!File.Exists(filePath))
                return new byte[0];
            return File.ReadAllBytes(filePath);
        }

        protected virtual Task SaveThumb(string thumbFilePath, string thumbFileName, byte[] binary)
        {
            File.WriteAllBytes(thumbFilePath, binary ?? new byte[0]);
            return Task.CompletedTask;
        }
        public virtual void SavePictureInFile(string pictureId, byte[] pictureBinary, string mimeType)
        {
            var lastPart = mimeType;
            var fileName = string.Format("{0}_0.{1}", pictureId, lastPart);
            File.WriteAllBytes(GetPictureLocalPath(fileName), pictureBinary);
        }
        public virtual async Task<bool> InsertPicture(byte[] pictureBinary, string mimeType, long ProductId, string path, string extension)
        {
            try
            {

                string pictureNameThumbnail = "";
                string pictureName = "";
                var ProductUpdate = _context.Products.Find(ProductId);
                var now = DateTime.Now;
                string des = ProductUpdate.Name;

                des = des.Replace(",", " ").Replace(".", " ").Replace("+", " ").Replace("-", " ").Replace(";", " ").Replace(":", " ").Replace(">", " ").Replace("<", " ").Replace("?", " ").Replace("\"", " ").Replace("'", " ").Replace("|", " ").Replace("/", " ").Replace("%", " ");
                des = des.Replace(" ", "-").Replace("--", "-").Replace("-", "-");
                des = des.Replace(" ", "-").Replace("--", "-").Replace("-", "-");
                string nameproduct = des.Replace(" ", "-").Replace("--", "-").Replace("-", "-");


                var uniqueFileName = $"{now.Millisecond}{now.Minute}{now.Second}{now.Day}-".Trim() + nameproduct;
                var uniqueFileNameThumbnail = $"{now.Millisecond}{now.Minute}{now.Second}{now.Day}-".Trim() + nameproduct + "(1)";


                // pictureBinary = ValidatePicture(pictureBinary, mimeType);


                var lastPart = extension;
                var fileName = string.Format("{0}{1}", uniqueFileName, lastPart);
                ////
                //

                Image returnImageMain = null;
                using (MemoryStream ms = new MemoryStream(pictureBinary))
                {
                    returnImageMain = Image.FromStream(ms);

                    Bitmap sourceImage = new Bitmap(returnImageMain);
                    Bitmap objBitmap = new Bitmap(500, 500);
                    
                        objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                        Graphics objGraphics = Graphics.FromImage(objBitmap);
                        
                            // Set the graphic format for better result cropping   
                            objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                            objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            objGraphics.DrawImage(sourceImage, 0, 0, 500, 500);

                            //var fileNameThumb = string.Format("{0}_0.{1}", "majorimage", lastPart);
                            // File.WriteAllBytes(GetPictureLocalPathThumbnail(fileNameThumb), pictureBinary);
                            // Save the file path, note we use png format to support png file  

                            var fileDbPathth = $"/Products/".Trim();

                            string filePathth = $"{_hostingEnvironment.WebRootPath}{fileDbPathth}".Trim();

                            if (!(Directory.Exists(filePathth)))
                                Directory.CreateDirectory(filePathth);


                            string datafilename = filePathth + uniqueFileName + lastPart;



                           objBitmap.Save(datafilename);

                            pictureName = fileDbPathth + uniqueFileName + lastPart;


                            //using (img = Image.FromStream(streamBitmap))
                            //{
                            //    img.Save(path);
                            //}


                        
                    
            }

                //File.WriteAllBytes(GetPictureLocalPath(fileName), pictureBinary);

            //
            Image returnImage = null;
            using (MemoryStream ms = new MemoryStream(pictureBinary))
            {
                returnImage = Image.FromStream(ms);

                Bitmap sourceImage = new Bitmap(returnImage);
                    Bitmap objBitmap = new Bitmap(175, 175);
                
                    objBitmap.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
                        Graphics objGraphics = Graphics.FromImage(objBitmap);
                    
                        // Set the graphic format for better result cropping   
                        objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        objGraphics.DrawImage(sourceImage, 0, 0, 175, 175);

                        //var fileNameThumb = string.Format("{0}_0.{1}", "majorimage", lastPart);
                        // File.WriteAllBytes(GetPictureLocalPathThumbnail(fileNameThumb), pictureBinary);
                        // Save the file path, note we use png format to support png file  

                        var fileDbPathth = $"/Products/Thumbnail/".Trim();

                        string filePathth = $"{_hostingEnvironment.WebRootPath}{fileDbPathth}".Trim();

                        if (!(Directory.Exists(filePathth)))
                            Directory.CreateDirectory(filePathth);


                        string datafilename = filePathth + uniqueFileNameThumbnail + lastPart;



                        objBitmap.Save(datafilename);

                        pictureNameThumbnail = fileDbPathth + uniqueFileNameThumbnail + lastPart;

                    
                
            }


            var img = new ProductPicture()
            {
                ProductId = ProductUpdate.Id,
                PictureUrl = pictureName,
                PicturePath = "https://" + path + pictureName,
                //
                PictureUrlThumbnail = pictureNameThumbnail,
                PicturePathThumbnail = "https://" + path + pictureNameThumbnail,

                CreatedDateTimeUtc = DateTime.UtcNow.AddHours(1),

            };
            var saveImageToDb = await _product.InsertImg(img);

            return true;
        }catch(Exception c)
            {
               // await _emailSender.SendToOne("onwukaemeka41@gmail.com", "error", "error", c.ToString());

                return false;
            }
}
        //public virtual byte[] ValidatePicture(byte[] byteArray, string mimeType)
        //{
        //    try
        //    {
        //        var format = EncodedImageFormat(mimeType);
        //        using (var ms = new MemoryStream(byteArray))
        //        {
        //            using (var image = SKBitmap.Decode(byteArray))
        //            {
        //                if (image.Width >= image.Height)
        //                {
        //                    //horizontal rectangle or square
        //                    if (image.Width > 500 && image.Height > 500)
        //                        byteArray = ApplyResize(image, format,500);
        //                }
        //                else if (image.Width < image.Height)
        //                {
        //                    //vertical rectangle
        //                    if (image.Width > 500)
        //                        byteArray = ApplyResize(image, format, 500);
        //                }
        //                return byteArray;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return byteArray;
        //    }
        //}
        ////protected SKEncodedImageFormat EncodedImageFormat(string mimetype)
        //{
        //    SKEncodedImageFormat defaultFormat = SKEncodedImageFormat.Jpeg;
        //    if (string.IsNullOrEmpty(mimetype))
        //        return defaultFormat;

        //    mimetype = mimetype.ToLower();

        //    if (mimetype.Contains("jpeg") || mimetype.Contains("jpg") || mimetype.Contains("pjpeg"))
        //        return defaultFormat;

        //    if (mimetype.Contains("png"))
        //        return SKEncodedImageFormat.Png;

        //    if (mimetype.Contains("webp"))
        //        return SKEncodedImageFormat.Webp;

        //    if (mimetype.Contains("webp"))
        //        return SKEncodedImageFormat.Webp;

        //    if (mimetype.Contains("gif"))
        //        return SKEncodedImageFormat.Gif;

        //    //if mime type is BMP format then happens error with convert picture
        //    if (mimetype.Contains("bmp"))
        //        return SKEncodedImageFormat.Png;

        //    if (mimetype.Contains("ico"))
        //        return SKEncodedImageFormat.Ico;

        //    return defaultFormat;

        //}
        
        //protected byte[] ApplyResize(SKBitmap image, SKEncodedImageFormat format, int targetSize)
        //{
        //    if (image == null)
        //        throw new ArgumentNullException("image");

        //    if (targetSize <= 0)
        //    {
        //        targetSize = 500;
        //    }
        //    float width, height;
        //    if (image.Height > image.Width)
        //    {
        //        // portrait
        //        width = image.Width * (targetSize / (float)image.Height);
        //        height = targetSize;
        //    }
        //    else
        //    {
        //        // landscape or square
        //        width = targetSize;
        //        height = image.Height * (targetSize / (float)image.Width);
        //    }

        //    if ((int)width == 0 || (int)height == 0)
        //    {
        //        width = image.Width;
        //        height = image.Height;
        //    }
        //    try
        //    {
        //        using (var resized = image.Resize(new SKImageInfo((int)width, (int)height), SKFilterQuality.Medium))
        //        {
        //            using (var resimage = SKImage.FromBitmap(resized))
        //            {
        //                return resimage.Encode(format, 300).ToArray();
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return image.Bytes;
        //    }

        //}


    }
}
