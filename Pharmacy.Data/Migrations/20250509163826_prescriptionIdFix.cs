using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.Data.Migrations
{
    /// <inheritdoc />
    public partial class prescriptionIdFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop FKs referencing Prescriptions.Id
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionMedicines_Prescriptions_PrescriptionId",
                table: "PrescriptionMedicines");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Prescriptions_PrescriptionId",
                table: "Sales");

            // 2. Drop PK from Prescriptions
            migrationBuilder.DropPrimaryKey(
                name: "PK_Prescriptions",
                table: "Prescriptions");

            // 3. Drop the old Id column
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Prescriptions");

            // 4. Add new string Id column
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Prescriptions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // 5. Add PK again
            migrationBuilder.AddPrimaryKey(
                name: "PK_Prescriptions",
                table: "Prescriptions",
                column: "Id");

            // 6. Alter PrescriptionId in foreign tables to match new type
            migrationBuilder.AlterColumn<string>(
                name: "PrescriptionId",
                table: "Sales",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PrescriptionId",
                table: "PrescriptionMedicines",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // 7. Re-add FKs
            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionMedicines_Prescriptions_PrescriptionId",
                table: "PrescriptionMedicines",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Prescriptions_PrescriptionId",
                table: "Sales",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");
        }

    }
}
