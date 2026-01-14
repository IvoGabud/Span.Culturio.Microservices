using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Span.Culturio.CultureObjects.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CultureObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ZipCode = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    City = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AdminUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CultureObjects", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CultureObjects",
                columns: new[] { "Id", "Address", "AdminUserId", "City", "ContactEmail", "Name", "ZipCode" },
                values: new object[,]
                {
                    { 1, "Rooseveltov trg 5", 1, "Zagreb", "kontakt@mimara.hr", "Muzej Mimara", 10000 },
                    { 2, "Trg Republike Hrvatske 15", 1, "Zagreb", "info@hnk.hr", "Hrvatsko narodno kazalište", 10000 },
                    { 3, "Avenija Dubrovnik 17", 1, "Zagreb", "kontakt@msu.hr", "Muzej suvremene umjetnosti", 10000 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CultureObjects");
        }
    }
}
