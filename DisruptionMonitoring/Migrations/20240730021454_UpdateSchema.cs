using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisruptionMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Articles",
                newName: "Lng");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Articles",
                newName: "Lat");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Articles",
                newName: "Created_At");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lng",
                table: "Articles",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "Lat",
                table: "Articles",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "Created_At",
                table: "Articles",
                newName: "CreatedAt");
        }
    }
}
