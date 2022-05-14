using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Products
{

       public class ProductDto
    {

        public long Id { get; set; }


        public string Name { get; set; }


        public string ProductCode { get; set; }


        public EntityStatus Status { get; set; }

        public int ReOrderLevel { get; set; }

        public SendNotification SendNotification { get; set; }



        public long CategoryId { get; set; }

        public long ManufacturerId { get; set; }

        public Category Category { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public Market Market { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }


        /// <summary>
        /// Gets or sets the product type identifier
        /// </summary>
        public long? SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
        public bool VisibleIndividually { get; set; }

        /// <summary>
        /// Gets or sets the sename
        /// </summary>
        public long? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Gets or sets the full description
        /// </summary>
        public string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }


        /// <summary>
        /// Gets or sets a vendor identifier
        /// </summary>

        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }
        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }
        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product allows customer reviews
        /// </summary>
        public bool AllowCustomerReviews { get; set; }
        /// <summary>
        /// Gets or sets the rating sum (approved reviews)
        /// </summary>
        public int ApprovedRatingSum { get; set; }
        /// <summary>
        /// Gets or sets the rating sum (not approved reviews)
        /// </summary>
        public int NotApprovedRatingSum { get; set; }
        /// <summary>
        /// Gets or sets the total rating votes (approved reviews)
        /// </summary>
        public int ApprovedTotalReviews { get; set; }
        /// <summary>
        /// Gets or sets the total rating votes (not approved reviews)
        /// </summary>
        public int NotApprovedTotalReviews { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is subject to ACL
        /// </summary>

        public string ManufacturerPartNumber { get; set; }
        /// <summary>
        /// Gets or sets the Global Trade Item Number (GTIN). These identifiers include UPC (in North America), EAN (in Europe), JAN (in Japan), and ISBN (for books).
        /// </summary>

        public bool RequireOtherProducts { get; set; }
        /// <summary>
        /// Gets or sets a required product identifiers (comma separated)
        /// </summary>
        public string RequiredProductIds { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether required products are automatically added to the cart
        /// </summary>
        public bool AutomaticallyAddRequiredProducts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is download
        /// </summary>

        public bool HasUserAgreement { get; set; }
        /// <summary>
        /// Gets or sets the text of license agreement
        /// </summary>
        public string UserAgreementText { get; set; }


        public int NotifyAdminForQuantityBelow { get; set; }
        /// <summary>
        /// Gets or sets a value backorder mode identifier
        /// </summary>

        public bool DisableBuyButton { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to disable "Add to wishlist" button
        /// </summary>
        public int Quantity { get; set; }

        public bool CallForPrice { get; set; }
        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Gets or sets the old price
        /// </summary>
        public decimal OldPrice { get; set; }


        /// <summary>
        /// Gets or sets the product cost
        /// </summary>
        public decimal ProductCost { get; set; }


        public bool MarkAsNew { get; set; }
        /// <summary>
        /// Gets or sets the start date and time of the new product (set product as "New" from date). Leave empty to ignore this property
        /// </summary>
        public DateTime? MarkAsNewStartDateTimeUtc { get; set; }
        /// <summary>
        /// Gets or sets the end date and time of the new product (set product as "New" to date). Leave empty to ignore this property
        /// </summary>
        public DateTime? MarkAsNewEndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the weight
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public decimal Length { get; set; }
        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public decimal Width { get; set; }
        /// <summary>
        /// Gets or sets the height
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the available start date and time
        /// </summary>
        public DateTime? AvailableStartDateTimeUtc { get; set; }
        /// <summary>
        /// Gets or sets the available end date and time
        /// </summary>
        public DateTime? AvailableEndDateTimeUtc { get; set; }


        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a display order for category page.
        /// This value is used when sorting products on category page
        /// </summary>
        public int DisplayOrderCategory { get; set; }

        /// <summary>
        /// Gets or sets a display order for manufacturer page.
        /// This value is used when sorting products on manufacturer page
        /// </summary>
        public int DisplayOrderManufacturer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the date and time of product creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the date and time of product update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the sold
        /// </summary>
        public int Sold { get; set; }

        /// <summary>
        /// Gets or sets the viewed
        /// </summary>
        public Int64 Viewed { get; set; }

        /// <summary>
        /// Gets or sets the onsale
        /// </summary>
        public int OnSale { get; set; }

        /// <summary>
        /// Gets or sets the flag
        /// </summary>
        public string ImageThumbnail { get; set; }


        public virtual ICollection<ProductPicture> ProductPictures
        {
            get; set;
        }

    }

}
