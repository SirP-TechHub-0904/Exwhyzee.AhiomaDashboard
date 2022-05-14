using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public EntityStatus Status { get; set; }
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show featured products on home page
        /// </summary>
        public bool FeaturedProductsOnHomaPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the category on search box
        /// </summary>
        public bool ShowOnSearchBox { get; set; }

        /// <summary>
        /// Gets or sets the display order on search box category
        /// </summary>
        public int SearchBoxDisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include this category in the top menu
        /// </summary>
        public bool IncludeInTopMenu { get; set; }
        public bool LimitedToStores { get; set; }
        //public IList<string> Stores { get; set; }
        public string Stores { get; set; }
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the flag
        /// </summary>
        public string Flag { get; set; }
        public string Icon { get; set; }
        public string ImageUrl { get; set; }
        public string ImagePath { get; set; }

        public string HomeBarner { get; set; }
        public string VerticalBannerUrl { get; set; }
        public string HorizontalBannerUrl { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; } 
        public ICollection<Product> Product { get; set; }

    }
}
