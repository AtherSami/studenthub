using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialProject.Migrations
{
    public partial class _99 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "UserModels");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "UserModels",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "UserModels",
                newName: "Attachment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "UserModels",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Attachment",
                table: "UserModels",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "UserModels",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
