using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.Data.Migrations
{
    /// <inheritdoc />
    public partial class PmFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyMedicines_Medicines_MedicineId",
                table: "PharmacyMedicines");

            migrationBuilder.AlterColumn<int>(
                name: "MedicineId",
                table: "PharmacyMedicines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ManufacturerMedicineId",
                table: "PharmacyMedicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyMedicines_ManufacturerMedicineId",
                table: "PharmacyMedicines",
                column: "ManufacturerMedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyMedicines_ManufacturerMedicines_ManufacturerMedicineId",
                table: "PharmacyMedicines",
                column: "ManufacturerMedicineId",
                principalTable: "ManufacturerMedicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyMedicines_Medicines_MedicineId",
                table: "PharmacyMedicines",
                column: "MedicineId",
                principalTable: "Medicines",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyMedicines_ManufacturerMedicines_ManufacturerMedicineId",
                table: "PharmacyMedicines");

            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyMedicines_Medicines_MedicineId",
                table: "PharmacyMedicines");

            migrationBuilder.DropIndex(
                name: "IX_PharmacyMedicines_ManufacturerMedicineId",
                table: "PharmacyMedicines");

            migrationBuilder.DropColumn(
                name: "ManufacturerMedicineId",
                table: "PharmacyMedicines");

            migrationBuilder.AlterColumn<int>(
                name: "MedicineId",
                table: "PharmacyMedicines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyMedicines_Medicines_MedicineId",
                table: "PharmacyMedicines",
                column: "MedicineId",
                principalTable: "Medicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
