using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisruptionMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class userprofiles2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfilesCategoryKeywords",
                table: "UserProfilesCategoryKeywords");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "UserProfilesCategoryKeywords",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfilesCategoryKeywords",
                table: "UserProfilesCategoryKeywords",
                columns: new[] { "UserProfileId", "CategoryKeywordId" });
        }
    }
}
