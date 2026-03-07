using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Master.Migrations
{
    /// <inheritdoc />
    public partial class JPStatusUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "JobPosts");

            migrationBuilder.AddColumn<int>(
                name: "JPStatus",
                table: "JobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JPStatus",
                table: "JobPosts");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "JobPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
