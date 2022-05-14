using System;
using System.Collections.Generic;
using System.Text;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data
{
    public class AhiomaDbContext : IdentityDbContext
    {
        
        public AhiomaDbContext(DbContextOptions<AhiomaDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<UtilityParameter> UtilityParameters { get; set; }
        public DbSet<TransferQueue> TransferQueues { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantSetting> TenantSettings { get; set; }
        public DbSet<TenantAddress> TenantAddresses { get; set; }
        public DbSet<TenantSocialMedia> TenantSocialMedias { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserProfileSocialMedia> UserProfileSocialMedias { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<LocalGoverment> LocalGoverments { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<ProductPicture> ProductPictures { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<UserReferee> UserReferees { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<StoreCategory> StoreCategories { get; set; }
        public DbSet<WebPages> WebPages { get; set; }
        public DbSet<FaqQuestion> FaqQuestions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<SoaRequest> SoaRequests { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductCart> ProductCarts { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<SavedItem> SavedItems { get; set; }
        public DbSet<AhiaPayTransfer> AhiaPayTransfers { get; set; }
        public DbSet<LogisticProfile> LogisticProfiles { get; set; }
        public DbSet<LogisticVehicle> LogisticVehicle { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductUploadShop> ProductUploadShops { get; set; }
        public DbSet<TrackOrder> TrackOrders { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<RequestPhoneEmailChange> RequestPhoneEmailChanges { get; set; }
        public DbSet<WalletHistory> WalletHistories { get; set; }
        public DbSet<Messaging> Messagings { get; set; }
        public DbSet<MailProduct> MailProducts { get; set; }
        public DbSet<MailContent> MailContents { get; set; }
        public DbSet<AdminReferral> AdminReferrals { get; set; }
        public DbSet<AdminShopReferral> AdminShopReferrals { get; set; }
        public DbSet<ProductCheck> ProductChecks { get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<PromoProduct> PromoProducts { get; set; }
        public DbSet<PromoCategory> PromoCategories { get; set; }
        public DbSet<BulkMailList> BulkMailLists { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }

    }
}
