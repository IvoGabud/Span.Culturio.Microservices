using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Span.Culturio.Packages.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ValidDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackageCultureObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    CultureObjectId = table.Column<int>(type: "int", nullable: false),
                    AvailableVisits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageCultureObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageCultureObjects_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Packages",
                columns: new[] { "Id", "Name", "ValidDays" },
                values: new object[,]
                {
                    { 1, "Osnovni paket", 30 },
                    { 2, "Premium paket", 90 },
                    { 3, "Godišnji paket", 365 }
                });

            migrationBuilder.InsertData(
                table: "PackageCultureObjects",
                columns: new[] { "Id", "AvailableVisits", "CultureObjectId", "PackageId" },
                values: new object[,]
                {
                    { 1, 5, 1, 1 },
                    { 2, 3, 2, 1 },
                    { 3, 2, 3, 1 },
                    { 4, 10, 1, 2 },
                    { 5, 8, 2, 2 },
                    { 6, 6, 3, 2 },
                    { 7, 30, 1, 3 },
                    { 8, 25, 2, 3 },
                    { 9, 20, 3, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageCultureObjects_PackageId",
                table: "PackageCultureObjects",
                column: "PackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageCultureObjects");

            migrationBuilder.DropTable(
                name: "Packages");
        }
    }
}
