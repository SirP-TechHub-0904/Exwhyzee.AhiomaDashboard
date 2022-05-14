using Exwhyzee.AhiomaDashboard.Data.Repository.Batches;
using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.Manufacturers;
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.Data.Repository.PayStack;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.Data.Repository.TenantAddresses;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.TenantSettings;
using Exwhyzee.AhiomaDashboard.Data.Repository.TenantSocialMedias;
using Exwhyzee.AhiomaDashboard.Data.Repository.Transactions;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserAddresses;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfileSocialMedias;
using Exwhyzee.AhiomaDashboard.Data.Repository.UtilityBill;
using Exwhyzee.AhiomaDashboard.Data.Repository.Wallets;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Exwhyzee.AhiomaDashboard.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<ITenantRepository, TenantRepository>();
            services.AddTransient<IBatchRepository, BatchRepository>();
            services.AddTransient<IManufacturerRepository, ManufacturerRepository>();
            services.AddTransient<IMarketRepository, MarketRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ITenantAddressRepository, TenantAddressRepository>();
            services.AddTransient<ITenantSettingRepository, TenantSettingRepository>();
            services.AddTransient<ITenantSocialMediaRepository, TenantSocialMediaRepository>();
            services.AddTransient<IUserAddressRepository, UserAddressRepository>();
            services.AddTransient<IUserProfileRepository, UserProfileRepository>();
            services.AddTransient<IUserProfileSocialMediaRepository, UserProfileSocialMediaRepository>();
            services.AddTransient<IWalletRepository, WalletRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IPaystackTransactionService, PaystackTransactionService>();
            services.AddTransient<IFlutterTransactionService, FlutterTransactionService>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IBillRepository, BillRepository>();
            return services;
        }
    }
    
}
