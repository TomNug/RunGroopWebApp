using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunGroopWebApp.Migrations
{
    /// <inheritdoc />
    public partial class URLsAndPublicIdsForCloudinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Races",
                newName: "ImageURL");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Clubs",
                newName: "ImageURL");

            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Races",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Clubs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Clubs");

            migrationBuilder.RenameColumn(
                name: "ImageURL",
                table: "Races",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "ImageURL",
                table: "Clubs",
                newName: "Image");
        }
    }
}
