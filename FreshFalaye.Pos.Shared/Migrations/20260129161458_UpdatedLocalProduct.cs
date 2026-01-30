using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshFalaye.Pos.Shared.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLocalProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseWeighingScale",
                table: "LocalProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseWeighingScale",
                table: "LocalProducts");
        }
    }
}
