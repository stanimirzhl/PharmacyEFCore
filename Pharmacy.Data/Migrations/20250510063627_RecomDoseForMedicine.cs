using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.Data.Migrations
{
    /// <inheritdoc />
    public partial class RecomDoseForMedicine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecommendedDosage",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecommendedDosage",
                table: "Medicines");
        }
    }
}
