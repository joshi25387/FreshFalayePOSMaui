using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshFalaye.Pos.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddedWeighingScaleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WeighingScaleBudRate",
                table: "LocalSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WeighingScaleComPortName",
                table: "LocalSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeighingScaleBudRate",
                table: "LocalSettings");

            migrationBuilder.DropColumn(
                name: "WeighingScaleComPortName",
                table: "LocalSettings");
        }
    }
}
