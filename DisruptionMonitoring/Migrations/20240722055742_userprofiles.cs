using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisruptionMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class userprofiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInterestedTopic");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "UserProfiles");

            migrationBuilder.CreateTable(
                name: "UserProfilesCategoryKeywords",
                columns: table => new
                {
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    CategoryKeywordId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfilesCategoryKeywords", x => new { x.UserProfileId, x.CategoryKeywordId });
                    table.ForeignKey(
                        name: "FK_UserProfilesCategoryKeywords_CategoryKeywords_CategoryKeywordId",
                        column: x => x.CategoryKeywordId,
                        principalTable: "CategoryKeywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProfilesCategoryKeywords_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfilesCategoryKeywords_CategoryKeywordId",
                table: "UserProfilesCategoryKeywords",
                column: "CategoryKeywordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfilesCategoryKeywords");

            migrationBuilder.AddColumn<int>(
                name: "LocationName",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupplierName",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserInterestedTopic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryKeywordsId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserProfileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInterestedTopic", x => x.Id);
                });
        }
    }
}
