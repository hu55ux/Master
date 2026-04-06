using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Master.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MasterRatingAdding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MasterRatings",
                columns: table => new
                {
                    MasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterRatings", x => new { x.MasterId, x.CustomerId });
                    table.ForeignKey(
                        name: "FK_MasterRatings_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MasterRatings_AspNetUsers_MasterId",
                        column: x => x.MasterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MasterRatings_CustomerId",
                table: "MasterRatings",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MasterRatings");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "AspNetUsers");
        }
    }
}
