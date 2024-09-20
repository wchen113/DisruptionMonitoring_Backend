using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisruptionMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class UpdateArticlesSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfilesCategoryKeywords_Suppliers_SupplierId",
                table: "UserProfilesCategoryKeywords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfilesCategoryKeywords",
                table: "UserProfilesCategoryKeywords");

            migrationBuilder.DropIndex(
                name: "IX_UserProfilesCategoryKeywords_SupplierId",
                table: "UserProfilesCategoryKeywords");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "UserProfilesCategoryKeywords");

            migrationBuilder.DropColumn(
                name: "Created_At",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "Lng",
                table: "Articles",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "Lat",
                table: "Articles",
                newName: "Latitude");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "UserProfilesCategoryKeywords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfilesCategoryKeywords",
                table: "UserProfilesCategoryKeywords",
                columns: new[] { "UserProfileId", "CategoryKeywordId" });

            migrationBuilder.CreateTable(
                name: "UserProfilesLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfilesLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfilesLocation_Cities_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProfilesLocation_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfilesSupplier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfilesSupplier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfilesSupplier_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProfilesSupplier_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfilesLocation_LocationId",
                table: "UserProfilesLocation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfilesLocation_UserProfileId",
                table: "UserProfilesLocation",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfilesSupplier_SupplierId",
                table: "UserProfilesSupplier",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfilesSupplier_UserProfileId",
                table: "UserProfilesSupplier",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfilesLocation");

            migrationBuilder.DropTable(
                name: "UserProfilesSupplier");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfilesCategoryKeywords",
                table: "UserProfilesCategoryKeywords");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "UserProfilesCategoryKeywords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Articles",
                newName: "Lng");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Articles",
                newName: "Lat");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "UserProfilesCategoryKeywords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Created_At",
                table: "Articles",
                type: "date",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfilesCategoryKeywords",
                table: "UserProfilesCategoryKeywords",
                columns: new[] { "UserProfileId", "CategoryKeywordId", "SupplierId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfilesCategoryKeywords_SupplierId",
                table: "UserProfilesCategoryKeywords",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfilesCategoryKeywords_Suppliers_SupplierId",
                table: "UserProfilesCategoryKeywords",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
