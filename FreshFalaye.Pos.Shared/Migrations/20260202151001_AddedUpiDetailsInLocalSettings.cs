using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshFalaye.Pos.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddedUpiDetailsInLocalSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpiId",
                table: "LocalStoreSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpiMerchantName",
                table: "LocalStoreSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpiId",
                table: "LocalStoreSettings");

            migrationBuilder.DropColumn(
                name: "UpiMerchantName",
                table: "LocalStoreSettings");
        }
    }
}
