using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshFalaye.Pos.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddedSycnState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddDeduct",
                table: "LocalSaleExpenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddDeduct",
                table: "LocalExpenseMaster",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SyncState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastSaleVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncState", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncState");

            migrationBuilder.DropColumn(
                name: "AddDeduct",
                table: "LocalSaleExpenses");

            migrationBuilder.DropColumn(
                name: "AddDeduct",
                table: "LocalExpenseMaster");
        }
    }
}
