using Pharmacy.Data.Data;
using Pharmacy.Data.Data.Models;

namespace Pharmacy.Core
{
	public class Seeder
	{
		PharmacyDbContext context;
		public Seeder(PharmacyDbContext context)
		{
			this.context = context;
		}

		public async Task SeedCategories(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');
				var category = new Category
				{
					CategoryName = parts[0],
					CategoryDescription = parts[1],
					IsDeleted = false
				};
				await context.Categories.AddAsync(category);
			}
			await context.SaveChangesAsync();
		}

		public async Task SeedManufacturers(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');
				var manufacturer = new Manufacturer
				{
					ManufacturerName = parts[0],
					Website = parts[1],
					Email = parts[2],
					Phone = parts[3],
					IsDeleted = false
				};
				await context.Manufacturers.AddAsync(manufacturer);
			}
			await context.SaveChangesAsync();
		}

		public async Task SeedDoctors(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');
				var doctor = new Doctor
				{
					DoctorName = parts[0],
					Specialty = parts[1],
					Email = parts[2],
					Phone = parts[3],
					IsDeleted = false
				};
				await context.Doctors.AddAsync(doctor);
			}
			await context.SaveChangesAsync();
		}

		public async Task SeedPatients(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');
				var patient = new Patient
				{
					PatientName = parts[0],
					DateOfBirth = DateTime.Parse(parts[1]),
					Email = parts[2],
					Phone = parts[3],
					IsDeleted = false
				};
				await context.Patients.AddAsync(patient);
			}
			await context.SaveChangesAsync();
		}

		public async Task SeedMedicines(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');
				var medicine = new Medicine
				{
					MedicineName = parts[0],
					Description = parts[1],
					CategoryId = int.Parse(parts[2]),
					RecommendedDosage = parts[3],
					IsDeleted = false
				};
				await context.Medicines.AddAsync(medicine);
			}
			await context.SaveChangesAsync();
		}
		public async Task SeedManufacturerMedicine(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');

				var mm = new ManufacturerMedicine
				{
					ManufacturerId = int.Parse(parts[0]),
					MedicineId = int.Parse(parts[1]),
					ManufacturerPrice = decimal.Parse(parts[2]),
					MadeQuantity = int.Parse(parts[3]),
					IsDeleted = false
				};

				await context.ManufacturerMedicines.AddAsync(mm);
			}
			await context.SaveChangesAsync();
		}

		public async Task SeedPharmacyMedicine(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');

				var pm = new PharmacyMedicine
				{
					ManufacturerMedicineId = int.Parse(parts[0]),
					StockQuantity = int.Parse(parts[1]),
					PharmacyPrice = decimal.Parse(parts[2]),
					IsDeleted = false
				};

				await context.PharmacyMedicines.AddAsync(pm);
			}
			await context.SaveChangesAsync();
		}

		public async Task SeedPrescriptions(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');

				var prescription = new Prescription
				{
					PatientId = int.Parse(parts[0]),
					DoctorId = int.Parse(parts[1]),
					PrescribedAt = DateTime.Parse(parts[2]),
					Id = parts[3],
					IsDeleted = false
				};

				await context.Prescriptions.AddAsync(prescription);
			}
			await context.SaveChangesAsync();
		}

		public async Task SeedOrders(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');

				var order = new Order
				{
					ManufacturerId = int.Parse(parts[0]),
					OrderDate = DateTime.Parse(parts[1]),
					Id = parts[2],
					IsDeleted = false
				};

				await context.Orders.AddAsync(order);
			}
			await context.SaveChangesAsync();
		}

		public async Task SeedSales(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');

				var sale = new Sale
				{
					PrescriptionId = parts[0],
					SaleDate = DateTime.Parse(parts[1]),
					IsDeleted = false
				};

				await context.Sales.AddAsync(sale);
			}
			await context.SaveChangesAsync();
		}
		public async Task SeedPrescriptionMedicines(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');

				var pm = new PrescriptionMedicine
				{
					PrescriptionId = parts[0],
					MedicineId = int.Parse(parts[1]),
					Dosage = parts[2],
					PrescribedQuantity = int.Parse(parts[3]),
					IsDeleted = false
				};

				await context.PrescriptionMedicines.AddAsync(pm);
			}
			await context.SaveChangesAsync();
		}
		public async Task SeedOrderMedicines(string filePath)
		{
			var lines = await File.ReadAllLinesAsync(filePath);
			foreach (var line in lines)
			{
				var parts = line.Split(',');

				var orderMedicine = new OrderMedicine
				{
					OrderId = parts[0],
					MedicineId = int.Parse(parts[1]),
					BoughtQuantity = int.Parse(parts[2])
				};

				context.OrderMedicines.Add(orderMedicine);
			}

			await context.SaveChangesAsync();
		}
	}
}
