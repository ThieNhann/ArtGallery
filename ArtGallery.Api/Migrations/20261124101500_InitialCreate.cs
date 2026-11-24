using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArtGallery.Api.Migrations;

/// <inheritdoc />
public partial class _20261124101500_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Carts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Carts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Orders",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                Total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                PaymentStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                ShippingAddress = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                ContactName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                ContactEmail = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                ContactPhone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                ReservedUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Orders", x => x.Id);
                table.CheckConstraint("CK_Orders_Total", "\"Total\" > 0");
            });

        migrationBuilder.CreateTable(
            name: "SiteSettings",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ShopName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                ArtistName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                ArtistBio = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                ArtistPhotoUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                LogoUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                PrimaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                ContactEmail = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                BankAccountInfo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                ReturnPolicy = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                FlatShippingFee = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                ReservationMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 20)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SiteSettings", x => x.Id);
                table.CheckConstraint("CK_SiteSettings_FlatShippingFee", "\"FlatShippingFee\" >= 0");
                table.CheckConstraint("CK_SiteSettings_Id", "\"Id\" = 1");
                table.CheckConstraint("CK_SiteSettings_ReservationMinutes", "\"ReservationMinutes\" > 0");
            });

        migrationBuilder.CreateTable(
            name: "Paintings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                Price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                WidthCm = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                HeightCm = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                Medium = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Year = table.Column<int>(type: "integer", nullable: true),
                IsFramed = table.Column<bool>(type: "boolean", nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Paintings", x => x.Id);
                table.CheckConstraint("CK_Paintings_Dimensions", "\"WidthCm\" > 0 AND \"HeightCm\" > 0");
                table.CheckConstraint("CK_Paintings_Price", "\"Price\" > 0");
                table.CheckConstraint("CK_Paintings_Year", "\"Year\" IS NULL OR \"Year\" > 0");
                table.ForeignKey(
                    name: "FK_Paintings_Categories_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "CartItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CartId = table.Column<Guid>(type: "uuid", nullable: false),
                PaintingId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CartItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_CartItems_Carts_CartId",
                    column: x => x.CartId,
                    principalTable: "Carts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CartItems_Paintings_PaintingId",
                    column: x => x.PaintingId,
                    principalTable: "Paintings",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "OrderItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                PaintingId = table.Column<Guid>(type: "uuid", nullable: true),
                PriceAtPurchase = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderItems", x => x.Id);
                table.CheckConstraint("CK_OrderItems_PriceAtPurchase", "\"PriceAtPurchase\" > 0");
                table.ForeignKey(
                    name: "FK_OrderItems_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_OrderItems_Paintings_PaintingId",
                    column: x => x.PaintingId,
                    principalTable: "Paintings",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "PaintingImages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                PaintingId = table.Column<Guid>(type: "uuid", nullable: false),
                StorageKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                ThumbUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                MediumUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                FullUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                Width = table.Column<int>(type: "integer", nullable: false),
                Height = table.Column<int>(type: "integer", nullable: false),
                SortOrder = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PaintingImages", x => x.Id);
                table.CheckConstraint("CK_PaintingImages_Dimensions", "\"Width\" > 0 AND \"Height\" > 0");
                table.ForeignKey(
                    name: "FK_PaintingImages_Paintings_PaintingId",
                    column: x => x.PaintingId,
                    principalTable: "Paintings",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CartItems_CartId_PaintingId",
            table: "CartItems",
            columns: new[] { "CartId", "PaintingId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CartItems_PaintingId",
            table: "CartItems",
            column: "PaintingId");

        migrationBuilder.CreateIndex(
            name: "IX_Carts_UserId",
            table: "Carts",
            column: "UserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Categories_Slug",
            table: "Categories",
            column: "Slug",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OrderItems_OrderId_PaintingId",
            table: "OrderItems",
            columns: new[] { "OrderId", "PaintingId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OrderItems_PaintingId",
            table: "OrderItems",
            column: "PaintingId");

        migrationBuilder.CreateIndex(
            name: "IX_PaintingImages_PaintingId_SortOrder",
            table: "PaintingImages",
            columns: new[] { "PaintingId", "SortOrder" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Paintings_CategoryId",
            table: "Paintings",
            column: "CategoryId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CartItems");

        migrationBuilder.DropTable(
            name: "OrderItems");

        migrationBuilder.DropTable(
            name: "PaintingImages");

        migrationBuilder.DropTable(
            name: "SiteSettings");

        migrationBuilder.DropTable(
            name: "Carts");

        migrationBuilder.DropTable(
            name: "Orders");

        migrationBuilder.DropTable(
            name: "Paintings");

        migrationBuilder.DropTable(
            name: "Categories");
    }
}
