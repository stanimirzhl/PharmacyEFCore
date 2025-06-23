using Pharmacy.Core;
using Pharmacy.Data.Data;

namespace PharmacyForm
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static async Task Main()
		{
			ApplicationConfiguration.Initialize();

			using (var context = new PharmacyDbContext())
			{
				bool databaseCreated = await context.Database.EnsureCreatedAsync();

				if (databaseCreated)
				{
					Seeder seeder = new Seeder(context);
					string SeederPath(string fileName) =>
						Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "SeederInfo", fileName);

					await seeder.SeedCategories(SeederPath("categories.txt"));
					await seeder.SeedManufacturers(SeederPath("manufacturers.txt"));
					await seeder.SeedDoctors(SeederPath("doctors.txt"));
					await seeder.SeedPatients(SeederPath("patients.txt"));
					await seeder.SeedMedicines(SeederPath("medicines.txt"));
					await seeder.SeedManufacturerMedicine(SeederPath("manufacturer_medicine.txt"));
					await seeder.SeedPharmacyMedicine(SeederPath("pharmacy_medicine.txt"));
					await seeder.SeedPrescriptions(SeederPath("prescriptions.txt"));
					await seeder.SeedOrders(SeederPath("orders.txt"));
					await seeder.SeedSales(SeederPath("sales.txt"));
					await seeder.SeedOrderMedicines(SeederPath("order_medicine.txt"));
					await seeder.SeedPrescriptionMedicines(SeederPath("prescription_medicine.txt"));
				}
			}

			using (var context2 = new PharmacyDbContext())
			{

				PharmacyController controller = new PharmacyController(context2);


				Application.Run(new Main(controller));
			}

		}
	}
}