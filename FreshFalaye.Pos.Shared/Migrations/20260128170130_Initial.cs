using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshFalaye.Pos.Shared.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalExpenseMaster",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyncId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpenseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RateType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bearer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalExpenseMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalProductGroups",
                columns: table => new
                {
                    ProductGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyncId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalProductGroups", x => x.ProductGroupId);
                });

            migrationBuilder.CreateTable(
                name: "LocalSales",
                columns: table => new
                {
                    LocalSaleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SyncId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SaleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerMobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpenseTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsSynced = table.Column<bool>(type: "bit", nullable: false),
                    IsUploaded = table.Column<bool>(type: "bit", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalSales", x => x.LocalSaleId);
                });

            migrationBuilder.CreateTable(
                name: "LocalSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PosDeviceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastBillSequence = table.Column<int>(type: "int", nullable: false),
                    LastSyncAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalStocks",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalStocks", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "LocalStoreSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GstIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillPrefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    POSCounterNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    POSSalesman = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiptLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiptLine2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiptLine3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiptLine4 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiptLine5 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiptLine6 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalStoreSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalUsers",
                columns: table => new
                {
                    LocalUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PinHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalUsers", x => x.LocalUserId);
                });

            migrationBuilder.CreateTable(
                name: "LocalProducts",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SyncId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DecimalAllowed = table.Column<bool>(type: "bit", nullable: false),
                    Mrp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalProducts", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_LocalProducts_LocalProductGroups_ProductGroupId",
                        column: x => x.ProductGroupId,
                        principalTable: "LocalProductGroups",
                        principalColumn: "ProductGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalSaleExpenses",
                columns: table => new
                {
                    LocalSaleExpenseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocalSaleId = table.Column<long>(type: "bigint", nullable: false),
                    ExpenseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RateType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bearer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalSaleExpenses", x => x.LocalSaleExpenseId);
                    table.ForeignKey(
                        name: "FK_LocalSaleExpenses_LocalSales_LocalSaleId",
                        column: x => x.LocalSaleId,
                        principalTable: "LocalSales",
                        principalColumn: "LocalSaleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalSaleItems",
                columns: table => new
                {
                    LocalSaleItemId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocalSaleId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Mrp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalSaleItems", x => x.LocalSaleItemId);
                    table.ForeignKey(
                        name: "FK_LocalSaleItems_LocalSales_LocalSaleId",
                        column: x => x.LocalSaleId,
                        principalTable: "LocalSales",
                        principalColumn: "LocalSaleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalProducts_ProductGroupId",
                table: "LocalProducts",
                column: "ProductGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalSaleExpenses_LocalSaleId",
                table: "LocalSaleExpenses",
                column: "LocalSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalSaleItems_LocalSaleId",
                table: "LocalSaleItems",
                column: "LocalSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalSales_SyncId",
                table: "LocalSales",
                column: "SyncId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalExpenseMaster");

            migrationBuilder.DropTable(
                name: "LocalProducts");

            migrationBuilder.DropTable(
                name: "LocalSaleExpenses");

            migrationBuilder.DropTable(
                name: "LocalSaleItems");

            migrationBuilder.DropTable(
                name: "LocalSettings");

            migrationBuilder.DropTable(
                name: "LocalStocks");

            migrationBuilder.DropTable(
                name: "LocalStoreSettings");

            migrationBuilder.DropTable(
                name: "LocalUsers");

            migrationBuilder.DropTable(
                name: "LocalProductGroups");

            migrationBuilder.DropTable(
                name: "LocalSales");
        }
    }
}
