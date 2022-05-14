using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class ProductPicture
    {
        public long Id { get; set; }
        public long? ProductId { get; set; }
        public string PictureUrl { get; set; }
        public string PictureName { get; set; }
        public string PicturePath { get; set; }

        public string PictureUrlThumbnail { get; set; }
        public string PicturePathThumbnail { get; set; }
        public DateTime? CreatedDateTimeUtc { get; set; }
        public string imgExtention { get; set; }
        public string AltAttribute { get; set; }

        /// <summary>
        /// Gets or sets the "title" attribute for "img" HTML element. If empty, then a default rule will be used (e.g. product name)
        /// </summary>
        public string TitleAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the picture is new
        /// </summary>
        public bool IsNew { get; set; }
        public bool IsDefault { get; set; }
        public string ThumbnailImageSize { get; set; }
        public string ImageSize { get; set; }
    }


    public class ProductPictureDto
    {
        public long Id { get; set; }
        public long? ProductId { get; set; }
        public string PictureUrl { get; set; }
        public string PictureName { get; set; }
        public string PicturePath { get; set; }

        public string PictureUrlThumbnail { get; set; }
        public string PicturePathThumbnail { get; set; }
        public DateTime? CreatedDateTimeUtc { get; set; }
        public string imgExtention { get; set; }
        public string AltAttribute { get; set; }

        /// <summary>
        /// Gets or sets the "title" attribute for "img" HTML element. If empty, then a default rule will be used (e.g. product name)
        /// </summary>
        public string TitleAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the picture is new
        /// </summary>
        public bool IsNew { get; set; }
        public bool IsDefault { get; set; }
        public decimal ThumbnailImageSize { get; set; }
        public decimal ImageSize { get; set; }
    }

}
