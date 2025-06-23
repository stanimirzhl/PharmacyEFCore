using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmacy.Data.Migrations
{
	/// <inheritdoc />
	public partial class LastMig : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Sales",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Prescriptions",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "PrescriptionMedicines",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "PharmacyMedicines",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Patients",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "OrderMedicines",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Medicines",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Manufacturers",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "ManufacturerMedicines",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Doctors",
				type: "bit",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Categories",
				type: "bit",
				nullable: false,
				defaultValue: false);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Sales");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Prescriptions");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "PrescriptionMedicines");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "PharmacyMedicines");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Patients");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "OrderMedicines");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Medicines");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Manufacturers");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "ManufacturerMedicines");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Doctors");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Categories");
		}
	}
}
