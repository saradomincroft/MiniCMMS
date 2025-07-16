using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniCMMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssetLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Assets",
                newName: "SubLocation");

            migrationBuilder.AddColumn<string>(
                name: "MainLocation",
                table: "Assets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainLocation",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "SubLocation",
                table: "Assets",
                newName: "Location");
        }
    }
}
