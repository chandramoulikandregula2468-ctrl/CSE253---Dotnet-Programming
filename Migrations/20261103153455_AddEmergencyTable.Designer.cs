
using Microsoft.EntityFrameworkCore.Migrations;

namespace TouristSafetySystem.Migrations
{
    public partial class AddEmergencyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Emergencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),

                    TouristId = table.Column<string>(type: "TEXT", nullable: false),

                    EmergencyType = table.Column<string>(type: "TEXT", nullable: false),

                    Description = table.Column<string>(type: "TEXT", nullable: false),

                    ReportedAt = table.Column<DateTime>(type: "TEXT", nullable: false),

                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emergencies", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emergencies");
        }
    }
}
