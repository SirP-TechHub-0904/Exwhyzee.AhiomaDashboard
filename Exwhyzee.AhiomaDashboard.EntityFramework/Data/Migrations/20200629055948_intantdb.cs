using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations
{
    public partial class intantdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlPath = table.Column<string>(nullable: true),
                    UrlLink = table.Column<string>(nullable: true),
                    AltText = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    Disable = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    DateUtc = table.Column<DateTime>(nullable: false),
                    BannerType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    TenantAddressId = table.Column<long>(nullable: false),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    InvoiceDate = table.Column<DateTime>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FaqQuestions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    FaqType = table.Column<int>(nullable: false),
                    Publish = table.Column<bool>(nullable: false),
                    Answer = table.Column<string>(nullable: true),
                    DateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaqQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    LocalGovernment = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    MarketType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Markets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    OtherNames = table.Column<string>(nullable: true),
                    Roles = table.Column<string>(nullable: true),
                    NextOfKin = table.Column<string>(nullable: true),
                    NextOfKinPhoneNumber = table.Column<string>(nullable: true),
                    FirstTimeLogin = table.Column<bool>(nullable: false),
                    DateRegistered = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastUpdateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebPages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    UniqueTitle = table.Column<string>(nullable: true),
                    Publish = table.Column<bool>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    DateUtc = table.Column<DateTime>(nullable: false),
                    PageType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalGoverments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LGAName = table.Column<string>(nullable: true),
                    StatesId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalGoverments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalGoverments_States_StatesId",
                        column: x => x.StatesId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenentHandle = table.Column<string>(nullable: true),
                    BusinessName = table.Column<string>(nullable: true),
                    BusinessDescription = table.Column<string>(nullable: true),
                    LogoUri = table.Column<string>(nullable: true),
                    BannerUri = table.Column<string>(nullable: true),
                    DoYouHaveOtherBranches = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    UserProfileId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreationUserId = table.Column<string>(nullable: true),
                    MarketId = table.Column<long>(nullable: false),
                    DeliveryType = table.Column<int>(nullable: false),
                    TenantStatus = table.Column<int>(nullable: false),
                    PaymentType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenants_Markets_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tenants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tenants_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    LocalGovernment = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    Latitiude = table.Column<string>(nullable: true),
                    Zipcode = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    UserProfileId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAddresses_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserProfileSocialMedias",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    UserProfileId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileSocialMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfileSocialMedias_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserReferees",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    ContactAddress = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    CreatedDateUtc = table.Column<DateTime>(nullable: false),
                    UpdateDateUtc = table.Column<DateTime>(nullable: false),
                    UserProfileId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReferees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReferees_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    MetaTitle = table.Column<string>(nullable: true),
                    ShowOnHomePage = table.Column<bool>(nullable: false),
                    FeaturedProductsOnHomaPage = table.Column<bool>(nullable: false),
                    ShowOnSearchBox = table.Column<bool>(nullable: false),
                    SearchBoxDisplayOrder = table.Column<int>(nullable: false),
                    IncludeInTopMenu = table.Column<bool>(nullable: false),
                    LimitedToStores = table.Column<bool>(nullable: false),
                    Stores = table.Column<string>(nullable: true),
                    Published = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Flag = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    ImagePath = table.Column<string>(nullable: true),
                    VerticalBannerUrl = table.Column<string>(nullable: true),
                    HorizontalBannerUrl = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantAddresses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    LocalGovernment = table.Column<string>(nullable: true),
                    MarketId = table.Column<long>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    Latitiude = table.Column<string>(nullable: true),
                    Zipcode = table.Column<string>(nullable: true),
                    TenantId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantAddresses_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantSettings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(nullable: false),
                    SiteType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSettings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantSocialMedias",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true),
                    TenantId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSocialMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSocialMedias_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreCategories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<long>(nullable: true),
                    CategoryId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CategoryId = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    MetaTitle = table.Column<string>(nullable: true),
                    ShowOnHomePage = table.Column<bool>(nullable: false),
                    FeaturedProductsOnHomaPage = table.Column<bool>(nullable: false),
                    ShowOnSearchBox = table.Column<bool>(nullable: false),
                    SearchBoxDisplayOrder = table.Column<int>(nullable: false),
                    IncludeInTopMenu = table.Column<bool>(nullable: false),
                    LimitedToStores = table.Column<bool>(nullable: false),
                    Stores = table.Column<string>(nullable: true),
                    Published = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Flag = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    UserProfileId = table.Column<long>(nullable: false),
                    DateOfTransaction = table.Column<DateTime>(nullable: false),
                    AmountPaid = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TenantAddressId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_TenantAddresses_TenantAddressId",
                        column: x => x.TenantAddressId,
                        principalTable: "TenantAddresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ProductCode = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ReOrderLevel = table.Column<int>(nullable: false),
                    SendNotification = table.Column<int>(nullable: false),
                    CategoryId = table.Column<long>(nullable: false),
                    ManufacturerId = table.Column<long>(nullable: false),
                    SubCategoryId = table.Column<long>(nullable: true),
                    VisibleIndividually = table.Column<bool>(nullable: false),
                    TenantId = table.Column<long>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true),
                    FullDescription = table.Column<string>(nullable: true),
                    AdminComment = table.Column<string>(nullable: true),
                    ShowOnHomePage = table.Column<bool>(nullable: false),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    MetaTitle = table.Column<string>(nullable: true),
                    AllowCustomerReviews = table.Column<bool>(nullable: false),
                    ApprovedRatingSum = table.Column<int>(nullable: false),
                    NotApprovedRatingSum = table.Column<int>(nullable: false),
                    ApprovedTotalReviews = table.Column<int>(nullable: false),
                    NotApprovedTotalReviews = table.Column<int>(nullable: false),
                    ManufacturerPartNumber = table.Column<string>(nullable: true),
                    RequireOtherProducts = table.Column<bool>(nullable: false),
                    RequiredProductIds = table.Column<string>(nullable: true),
                    AutomaticallyAddRequiredProducts = table.Column<bool>(nullable: false),
                    HasUserAgreement = table.Column<bool>(nullable: false),
                    UserAgreementText = table.Column<string>(nullable: true),
                    NotifyAdminForQuantityBelow = table.Column<int>(nullable: false),
                    DisableBuyButton = table.Column<bool>(nullable: false),
                    CallForPrice = table.Column<bool>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    OldPrice = table.Column<decimal>(nullable: false),
                    ProductCost = table.Column<decimal>(nullable: false),
                    MarkAsNew = table.Column<bool>(nullable: false),
                    MarkAsNewStartDateTimeUtc = table.Column<DateTime>(nullable: true),
                    MarkAsNewEndDateTimeUtc = table.Column<DateTime>(nullable: true),
                    Weight = table.Column<decimal>(nullable: false),
                    Length = table.Column<decimal>(nullable: false),
                    Width = table.Column<decimal>(nullable: false),
                    Height = table.Column<decimal>(nullable: false),
                    AvailableStartDateTimeUtc = table.Column<DateTime>(nullable: true),
                    AvailableEndDateTimeUtc = table.Column<DateTime>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    DisplayOrderCategory = table.Column<int>(nullable: false),
                    DisplayOrderManufacturer = table.Column<int>(nullable: false),
                    Published = table.Column<bool>(nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(nullable: false),
                    Sold = table.Column<int>(nullable: false),
                    Viewed = table.Column<long>(nullable: false),
                    OnSale = table.Column<int>(nullable: false),
                    Flag = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Products_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductPictures",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(nullable: true),
                    PictureUrl = table.Column<string>(nullable: true),
                    PictureName = table.Column<string>(nullable: true),
                    PicturePath = table.Column<string>(nullable: true),
                    PictureUrlThumbnail = table.Column<string>(nullable: true),
                    PicturePathThumbnail = table.Column<string>(nullable: true),
                    CreatedDateTimeUtc = table.Column<DateTime>(nullable: true),
                    imgExtention = table.Column<string>(nullable: true),
                    AltAttribute = table.Column<string>(nullable: true),
                    TitleAttribute = table.Column<string>(nullable: true),
                    IsNew = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPictures_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitCostPrice = table.Column<decimal>(nullable: false),
                    SerialNumber = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    UnitSellingPrice = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    QuantitySold = table.Column<int>(nullable: false),
                    QuantityMoved = table.Column<int>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    BatchId = table.Column<long>(nullable: false),
                    DateEntered = table.Column<DateTime>(nullable: false),
                    ManufacturedDate = table.Column<DateTime>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchases_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(nullable: false),
                    PurchaseId = table.Column<long>(nullable: false),
                    PricePerUnit = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sales_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId",
                table: "Categories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalGoverments_StatesId",
                table: "LocalGoverments",
                column: "StatesId");

            migrationBuilder.CreateIndex(
                name: "IX_Markets_UserId",
                table: "Markets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TenantAddressId",
                table: "Orders",
                column: "TenantAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserProfileId",
                table: "Orders",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPictures_ProductId",
                table: "ProductPictures",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ManufacturerId",
                table: "Products",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SubCategoryId",
                table: "Products",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId",
                table: "Products",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_BatchId",
                table: "Purchases",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ProductId",
                table: "Purchases",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_OrderId",
                table: "Sales",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_PurchaseId",
                table: "Sales",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreCategories_CategoryId",
                table: "StoreCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAddresses_TenantId",
                table: "TenantAddresses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_MarketId",
                table: "Tenants",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_UserId",
                table: "Tenants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_UserProfileId",
                table: "Tenants",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSettings_TenantId",
                table: "TenantSettings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSocialMedias_TenantId",
                table: "TenantSocialMedias",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserProfileId",
                table: "UserAddresses",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileSocialMedias_UserProfileId",
                table: "UserProfileSocialMedias",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReferees_UserProfileId",
                table: "UserReferees",
                column: "UserProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.DropTable(
                name: "FaqQuestions");

            migrationBuilder.DropTable(
                name: "LocalGoverments");

            migrationBuilder.DropTable(
                name: "ProductPictures");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "StoreCategories");

            migrationBuilder.DropTable(
                name: "TenantSettings");

            migrationBuilder.DropTable(
                name: "TenantSocialMedias");

            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "UserProfileSocialMedias");

            migrationBuilder.DropTable(
                name: "UserReferees");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "WebPages");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "TenantAddresses");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "Markets");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
