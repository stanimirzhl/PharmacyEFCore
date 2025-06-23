using Pharmacy.Core;
using Pharmacy.Data.Data;
using System.Globalization;
using System.Text.RegularExpressions;

var context = new PharmacyDbContext();
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();

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

var controller = new PharmacyController(context);


while (true)
{
	Console.WriteLine("=== PHARMACY SYSTEM MENU ===");
	Console.WriteLine("1. Add to table");
	Console.WriteLine("2. Delete data from table");
	Console.WriteLine("3. Update data from table");
	Console.WriteLine("4. Get data from table");
	Console.WriteLine("5. Get Patients By Medicine Name");
	Console.WriteLine("6. Get Total Sales By Year");
	Console.WriteLine("7. Get Last Prescription By Patient");
	Console.WriteLine("8. Get All Orders By Manufacturer");
	Console.WriteLine("9. Get Prescriptions By Medicine Name");
	Console.WriteLine("10. Get Unordered Medicines");
	Console.WriteLine("11. Get Low On Stock Medicines In Pharmacy");
	Console.WriteLine("12. Get Old Patients");
	Console.WriteLine("13. Get Orders In The Last 30 Days");
	Console.WriteLine("14. Get All Manufacturers With Email Ending In '.bg'");
	Console.WriteLine("0. Exit");
	Console.Write("Select an option: ");
	Console.WriteLine();
	int choice;
	string stringifiedChoice = Console.ReadLine();
	while (!int.TryParse(stringifiedChoice, out choice))
	{
		Console.WriteLine("Invalid type. Try again with a number");
		stringifiedChoice = Console.ReadLine();
	}

	switch (choice)
	{
		case 1:
			#region AddToTables
			string[] tables = { "categories", "manufacturers", "doctors", "patients", "medicines", "manufacturer medicine", "pharmacy medicine", "prescriptions", "orders", "sales" };
			Console.WriteLine($"Choose table to add: {string.Join(", ", tables)}");
			string table = Console.ReadLine();
			switch (table)
			{
				case "categories":
					try
					{
						Console.Write("New category for medicine:");
						string name = Console.ReadLine();
						Console.Write("Description of the new category:");
						string description = Console.ReadLine();
						await controller.AddCategory(name, description);

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "manufacturers":
					try
					{
						Console.Write("What is the name of the new manufacturer? :");
						string name = Console.ReadLine();
						Console.Write("Their email address:");
						string email = Console.ReadLine();
						Console.WriteLine("They probably have a website too? :");
						string website = Console.ReadLine();
						Console.WriteLine("And for last their phone number:");
						string phone = Console.ReadLine();

						await controller.AddManufacturer(name, email, website, phone);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "doctors":
					try
					{
						Console.Write("The name of the newly recruited doctor:");
						string name = Console.ReadLine();
						Console.Write("He specializes in? :");
						string specialty = Console.ReadLine();
						Console.Write("Email address of the doctor:");
						string email = Console.ReadLine();
						Console.Write("And his phone number:");
						string phone = Console.ReadLine();

						await controller.AddDoctor(name, specialty, email, phone);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "patients":
					try
					{
						Console.Write("Name of the newly registrated patient? :");
						string name = Console.ReadLine();
						Console.Write("His email address:");
						string email = Console.ReadLine();
						Console.Write("His phone number:");
						string phone = Console.ReadLine();
						Console.Write("His date of birth, preferably in the format yyyy-MM-dd(example: 2001-09-11):");
						string date = Console.ReadLine();
						DateTime birth;
						while (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out birth))
						{
							Console.WriteLine("Invalid format, please keep this one yyyy-MM-dd.");
							date = Console.ReadLine();
						}
						await controller.AddPatient(name, email, phone, birth);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "medicines":
					try
					{
						Console.WriteLine($"To add new medicine you need to add to specific category first, here is a list of all available ones: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllCategories())}");
						Console.Write("Category of the medicine:");
						string category = Console.ReadLine();
						int categoryId = await controller.GetCategoryId(category);
						Console.Write("Name of the new medicine:");
						string name = Console.ReadLine();
						Console.Write("What does the medicine cure, brief description:");
						string description = Console.ReadLine();
						Console.WriteLine("What is the recommended dosage for this medicine, the format should be number followed by mg (e.g., 100mg)?: ");
						string dosage = Console.ReadLine();
						if (!Regex.IsMatch(dosage, @"^\d+mg$"))
						{
							Console.WriteLine("Dosage wasn't in the correct format, try again with the above-mentioned one!");
							break;
						}
						await controller.AddMedicine(name, description, categoryId, dosage);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "manufacturer medicine":
					Console.Write("Do you want to refill existing medicine supply for manufacturer? Y/N: ");
					string result = Console.ReadLine().ToLower();
					if (result == "yes" || result == "y")
					{
						try
						{
							Console.WriteLine($"Choose manufacturer whose medicine stock supply you wish to refill, here is a list of all available ones: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllManufacturers())}");
							Console.Write("Manufacturer: ");
							string manufacturer = Console.ReadLine();
							int manufacturerId = await controller.GetManufacturerId(manufacturer);
							Console.WriteLine($"Medicine which stock is going to be refilled: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicinesByManufacturer(manufacturerId))}");
							Console.Write("Medicine: ");
							string medicine = Console.ReadLine();
							int medicineId = await controller.GetMedicineId(medicine);
							Console.Write("Quantity to be refilled: ");
							if (!int.TryParse(Console.ReadLine(), out int quantity))
							{
								Console.WriteLine("Invalid type for quantity, please try again.");
								break;
							}
							if (quantity <= 0)
							{
								Console.WriteLine("Quantity must be greater than 0, try again!");
								break;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
						break;
					}
					try
					{
						Console.WriteLine($"Choose manufacturer that will make the new medicine, here is a list of all available ones: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllManufacturers())}");
						Console.Write("Manufacturer:");
						string manufacturer = Console.ReadLine();
						int manufacturerId = await controller.GetManufacturerId(manufacturer);
						Console.Write($"Choose from the available medicines: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicines())}");
						string medicine = Console.ReadLine();
						int medicineId = await controller.GetMedicineId(medicine);
						Console.WriteLine("How much will the manufacturer charge?: ");
						if (!decimal.TryParse(Console.ReadLine(), out decimal price))
						{
							Console.WriteLine("Invalid type for price, please try again.");
							break;
						}
						Console.WriteLine("In what quantity will the medicine is going to be made?: ");
						if (!int.TryParse(Console.ReadLine(), out int quantity))
						{
							Console.WriteLine("Invalid type for quantity, please try again.");
							break;
						}
						if (quantity <= 0)
						{
							Console.WriteLine("Quantity must be greater than 0, try again!");
							break;
						}

						await controller.AddManufacturerMedicine(manufacturerId, medicineId, price, quantity);

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "pharmacy medicine":
					int manufacturerId2 = 0;
					int quantity2 = 0;
					int medicineId2 = 0;
					try
					{
						Console.WriteLine($"Choose a manufacturer you wish to order the medicine from: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllManufacturers())}");
						Console.Write("Manufacturer: ");
						string manufacturer = Console.ReadLine();
						manufacturerId2 = await controller.GetManufacturerId(manufacturer);
						Console.WriteLine($"Here are the medicines they sell: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicinesByManufacturer(manufacturerId2))}");
						Console.Write("Choose the one you want to buy: ");
						string medicine = Console.ReadLine();
						medicineId2 = await controller.GetMedicineId(medicine);
						Console.WriteLine("How much will it cost?: ");
						if (!decimal.TryParse(Console.ReadLine(), out decimal price))
						{
							Console.WriteLine("Invalid type for price, please try again.");
							break;
						}
						Console.WriteLine("The amount of medicine that is going to be purchased: ");
						if (!int.TryParse(Console.ReadLine(), out quantity2))
						{
							Console.WriteLine("Invalid type for quantity, please try again.");
							break;
						}
						if (quantity2 <= 0)
						{
							Console.WriteLine("Quantity must be greater than 0, try again!");
							break;
						}
						await controller.AddOrder(manufacturerId2);
						string orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId2);
						await controller.AddOrderMedicine(orderId, medicineId2, quantity2);
						await controller.AddPharmacyMedicine(manufacturerId2, medicineId2, quantity2, price);
					}
					catch (ArgumentNullException ex)
					{
						Console.WriteLine(ex.Message);
						Console.WriteLine("Do you wish to place new order to the manufacturer to refill the medicine? Y/N: ");
						string answer = Console.ReadLine().ToLower();
						if (answer == "y" || answer == "yes")
						{
							try
							{
								quantity2 = quantity2 < 100 ? quantity2 = 100 : quantity2;
								await controller.AddOrder(manufacturerId2);
								string orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId2);
								await controller.AddOrderMedicine(orderId, medicineId2, quantity2);
								Console.WriteLine("Order successfully placed!");
							}
							catch (Exception inexistant)
							{
								Console.WriteLine(inexistant.Message);
								string orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId2);
								await controller.DeleteOrder(orderId);
							}
						}

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "prescriptions":
					List<string> pharmacyMedicines;
					int medicineIdToParse = 0;
					int quantityToParse = 0;
					try
					{
						pharmacyMedicines = await controller.GetAllMedicinesInPharmacy("dose");
						Console.WriteLine($"Only a doctor can write you a prescription choose one: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllDoctors())}");
						Console.Write("Your doctor: ");
						string doctor = Console.ReadLine();
						int doctorId = await controller.GetDoctorId(doctor);
						Console.WriteLine("Choose a patient who's getting medicine prescribed to: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllPatients())}");
						Console.Write("Patient: ");
						string patient = Console.ReadLine();
						int patientId = await controller.GetPatientId(patient);
						Console.Write("Date of the prescription, preferably in the format yyyy-MM-dd: ");
						if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
						{
							Console.WriteLine("Wrong format, coorect one is yyyy-MM-dd, try again!");
							break;
						}
						Console.WriteLine($"Write the name of the medicines {patient} is getting prescribed, here is a list of all medicines in the pharmacy to help you: " + "\n" + $"{string.Join(Environment.NewLine + "-", pharmacyMedicines)}");
						Console.WriteLine("Write medicine name, quantity and dosage for the patient until the word 'That's all', write them on one line separated by line a dash (e.g., example-example), Dont forget the format for dosage is number + mg e.g. 100mg");
						string prescriptionId = null;
						while (true)
						{
							string medicine = Console.ReadLine();
							if (medicine.ToLower() == "that's all")
							{
								break;
							}
							Console.Write("Prescription details: ");
							string[] medicineInfo = medicine.Split('-');
							if (medicineInfo.Length != 3)
							{
								Console.WriteLine("Something is missing, try again.");
								continue;
							}
							try
							{
								string medicineName = medicineInfo[0];
								medicineIdToParse = await controller.GetMedicineId(medicineName);
								if (!int.TryParse(medicineInfo[1], out quantityToParse))
								{
									Console.WriteLine("Quantity wasn't in the correct format, try again with a number!");
									continue;
								}
								if (quantityToParse <= 0)
								{
									Console.WriteLine("Quantity must be greater than 0, try again!");
									continue;
								}
								string dosage = medicineInfo[2];
								if (!Regex.IsMatch(dosage, @"^\d+mg$"))
								{
									Console.WriteLine("Dosage wasn't in the correct format, try again with this one (number + mg, e.g. 100mg)!");
									continue;
								}
								if (prescriptionId == null)
								{
									await controller.AddPrescription(doctorId, patientId, date);
									prescriptionId = await controller.GetLastPrescriptionId();
								}
								await controller.AddPrescriptionMedicine(prescriptionId, medicineIdToParse, dosage, quantityToParse);
							}
							catch (ArgumentNullException ex)
							{
								Console.WriteLine(ex.Message);
								int manufacturerId = int.Parse(ex.Data["manufacturerId"].ToString());
								var manufacturer = await controller.GetManufacturerNameById(manufacturerId);
								Console.WriteLine($"It seems like the stock of the medicine with manufacturer: {manufacturer} has ran out, do you wish to restock it? Y/N: ");
								string answer = Console.ReadLine().ToLower();
								if (answer == "y" || answer == "yes")
								{
									try
									{
										quantityToParse = quantityToParse < 100 ? quantityToParse = 100 : quantityToParse;
										await controller.AddOrder(manufacturerId);
										string orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId);
										await controller.AddOrderMedicine(orderId, medicineIdToParse, quantityToParse);
										Console.WriteLine("Order successfully placed!");
									}
									catch (Exception inexistant)
									{
										Console.WriteLine(inexistant.Message);
										string orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId);
										await controller.DeleteOrder(orderId);
									}
								}
							}
							catch (ArgumentOutOfRangeException ex)
							{
								Console.WriteLine(ex.Message);
								int manufacturerId = int.Parse(ex.Data["manufacturerId"].ToString());
								var manufacturer = await controller.GetManufacturerNameById(manufacturerId);
								Console.WriteLine($"It seems like the stock of the medicine with manufacturer: {manufacturer} is not enough, do you wish to restock it? Y/N: ");
								string answer = Console.ReadLine().ToLower();
								if (answer == "y" || answer == "yes")
								{
									try
									{
										quantityToParse = quantityToParse < 100 ? quantityToParse = 100 : quantityToParse;
										await controller.AddOrder(manufacturerId);
										string orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId);
										await controller.AddOrderMedicine(orderId, medicineIdToParse, quantityToParse);
										Console.WriteLine("Order successfully placed!");
									}
									catch (Exception inexistant)
									{
										Console.WriteLine(inexistant.Message);
										string orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId);
										await controller.DeleteOrder(orderId);
									}
								}
							}
						}
						if (prescriptionId == null)
						{
							Console.WriteLine("No items found, prescription terminated!");
						}
						else
						{
							Console.WriteLine("Prescription successfully placed!");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "orders":
					List<string> allMedicinesByManufacturer;
					try
					{
						Console.WriteLine("Here is a list of all medicines currently in the pharmacy: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicinesInPharmacy())}");
						Console.WriteLine($"Choose a manufacturer you wish to order the medicine from: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllManufacturers())}");
						Console.Write("Manufacturer: ");
						string manufacturer = Console.ReadLine();
						int manufacturerId = await controller.GetManufacturerId(manufacturer);
						allMedicinesByManufacturer = await controller.GetAllMedicinesByManufacturer(manufacturerId);
						Console.WriteLine($"Here is a list of all medicines produced by and their current instock availability {manufacturer}: " + "\n" + $"{string.Join(Environment.NewLine + "-", allMedicinesByManufacturer)}");
						Console.WriteLine("Write medicine name and quantity you wish to order separated by slash e.g. MedicineName-Quantity, until the word 'Order'");
						string orderId = null;
						while (true)
						{
							string command = Console.ReadLine();
							if (command.ToLower() == "order")
							{
								break;
							}
							Console.Write("Order details: ");
							string[] medicineInfo = command.Split('-');
							if (medicineInfo.Length != 2)
							{
								Console.WriteLine("Something is missing, try again.");
								continue;
							}
							int medicineId = 0;
							int quantity = 0;
							try
							{
								string medicineName = medicineInfo[0];
								medicineId = await controller.GetMedicineId(medicineName);
								if (!int.TryParse(medicineInfo[1], out quantity))
								{
									Console.WriteLine("Quantity wasn't in the correct format, try again with a number!");
									continue;
								}
								if (quantity <= 0)
								{
									Console.WriteLine("Quantity must be greater than 0, try again!");
									continue;
								}
								if (orderId == null)
								{
									await controller.AddOrder(manufacturerId);
									orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId);
								}

								await controller.AddOrderMedicine(orderId, medicineId, quantity);
							}
							catch (InvalidOperationException ex)
							{
								Console.WriteLine(ex.Message);
								Console.WriteLine("Do you wish to add this new medicine to the pharmacy's stock supply? Y/N: ");
								string option = Console.ReadLine().ToLower();
								if (option == "yes" || option == "y")
								{
									Console.Write($"How much will you charge for 1 stock of {medicineInfo[0]}?: ");
									if (!decimal.TryParse(Console.ReadLine(), out decimal price))
									{
										Console.WriteLine("Invalid type for price, please try again.");
										break;
									}
									await controller.AddPharmacyMedicine(manufacturerId, medicineId, quantity, price);
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}
						}
						if (orderId == null)
						{
							Console.WriteLine("No items were found, order wasn't sent.");
						}
						else
						{
							Console.WriteLine("Order successfully placed!");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "sales":
					try
					{
						Console.WriteLine("Here is presented a list of prescription identificators, choose the one you want to buy: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllPrescriptionIds())}");
						Console.Write("Write the randomized part of the prescription Id and date you will buy, e.g. RandomPart-Date: ");
						string random = Console.ReadLine();
						string prescriptionId = await controller.GetPrescriptionId(random);
						await controller.AddSale(prescriptionId);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				default:
					Console.WriteLine("Seems like you didn't pay enough attention, try again next time with valid table name..");
					break;
			}
			#endregion
			break;
		case 2:
			#region DeleteDataFromTables
			string[] tables2 = { "categories", "manufacturers", "doctors", "patients", "medicines", "manufacturer medicine", "pharmacy medicine", "prescriptions", "orders", "sales" };
			Console.WriteLine($"Choose a table to remove data from: {string.Join(", ", tables2)}");
			string table2 = Console.ReadLine();
			switch (table2)
			{
				case "categories":
					try
					{
						Console.WriteLine($"Here is a list of all categories: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllCategories())}");
						Console.Write("Category to remove: ");
						string category = Console.ReadLine();
						int categoryId = await controller.GetCategoryId(category);
						await controller.DeleteCategory(categoryId);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "manufacturers":
					try
					{
						Console.WriteLine($"Here is a list of all categories: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllManufacturers())}");
						Console.Write("Category to remove: ");
						string manufacturer = Console.ReadLine();
						int manufacturerId = await controller.GetCategoryId(manufacturer);
						await controller.DeleteManufacturer(manufacturerId);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "doctors":
					try
					{
						Console.WriteLine($"Here is a list of all doctors: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllDoctors())}");
						Console.Write("Doctor to remove: ");
						string doctor = Console.ReadLine();
						int doctorId = await controller.GetDoctorId(doctor);
						await controller.DeleteDoctor(doctorId);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "patients":
					try
					{
						Console.WriteLine($"Here is a list of all patients: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllPatients())}");
						Console.Write("Patient to remove: ");
						string patient = Console.ReadLine();
						int patientId = await controller.GetPatientId(patient);
						await controller.DeletePatient(patientId);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "medicines":
					try
					{
						Console.WriteLine($"Here is a list of all medicines: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicines())}");
						Console.Write("Medicine to remove: ");
						string medicine = Console.ReadLine();
						int medicineId = await controller.GetPatientId(medicine);
						await controller.DeleteMedicine(medicineId);
						foreach (var id in await controller.CheckQuantityManufacturer(medicineId))
						{
							await controller.DeleteManufacturerMedicine(id, medicineId);
						}
						foreach (var id in await controller.CheckQuantityPharmacy(medicineId))
						{
							await controller.DeletePharmacyMedicine(id, medicineId);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "manufacturer medicine":
					try
					{
						Console.WriteLine($"Here is a list of all manufacturers: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllManufacturers())}");
						Console.Write("Manufacturer: ");
						string manufacturer = Console.ReadLine();
						int manufacturerId = await controller.GetPatientId(manufacturer);
						Console.WriteLine($"Here is a list of all medicines produced by {manufacturer}: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicinesByManufacturer(manufacturerId))}");
						Console.Write("Medicine to remove: ");
						string medicine = Console.ReadLine();
						int medicineId = await controller.GetMedicineId(medicine);
						await controller.DeleteManufacturerMedicine(manufacturerId, medicineId);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "pharmacy medicine":
					try
					{
						Console.WriteLine("Here is a list with all medicines and their manufacturers currently available in the pharmacy: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicinesInPharmacy("manufacturer"))}");
						Console.Write("Medicine and it's manufacturer to remove, (e.g. MedicineName-ManufacturerName): ");
						string[] strings = Console.ReadLine().Split('-', StringSplitOptions.RemoveEmptyEntries);
						if (strings.Length != 2)
						{
							Console.WriteLine("Something is missing, try again.");
							break;
						}
						int manufacturerId = await controller.GetManufacturerId(strings[1]);
						int medicineId = await controller.GetMedicineId(strings[0]);
						await controller.DeletePharmacyMedicine(manufacturerId, medicineId);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "prescriptions":
					try
					{
						Console.WriteLine($"Here is a list of all prescriptions: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllPrescriptionIds())}");
						Console.Write("Prescription to remove: ");
						string prescription = Console.ReadLine();
						string prescriptionId = await controller.GetPrescriptionId(prescription);
						await controller.DeletePrescription(prescriptionId);
						var (lastPrescriptionId, medicineCollection) = await controller.GetLastDeletedPrescriptionIdAndCollection();

						medicineCollection.ForEach(async x => await controller.DeletePrescriptionMedicine(lastPrescriptionId));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "orders":
					try
					{
						Console.WriteLine($"Here is a list of all orders: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllOrders())}");
						Console.Write("Order to remove, write the Id in the following format e.g. Date-Random: ");
						string order = Console.ReadLine();
						string orderId = await controller.GetOrderId(order);
						await controller.DeleteOrder(orderId);
						var (lastOrderId, orderMedicines) = await controller.GetLastDeletedOrderIdAndCollection();

						orderMedicines.ForEach(async x => await controller.DeleteOrderMedicine(lastOrderId));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "sales":
					Console.WriteLine("Sorry but any data from the sales table is not available for edit or deletion");
					break;
				default:
					Console.WriteLine("Seems like you didn't pay enough attention, try again next time with valid table name..");
					break;
			}
			#endregion
			break;
		case 3:
			#region UpdateDataInTables
			string[] tables3 = { "categories", "manufacturers", "doctors", "patients", "medicines", "manufacturer medicine", "pharmacy medicine", "prescriptions", "orders", "sales" };
			Console.WriteLine($"Choose a table to update data from: {string.Join(", ", tables3)}");
			string table3 = Console.ReadLine();
			switch (table3)
			{
				case "categories":
					try
					{
						Console.WriteLine($"Here is a list of all categories: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllCategories())}");
						Console.Write("Category to update: ");
						string category = Console.ReadLine();
						int categoryId = await controller.GetCategoryId(category);
						Console.Write("Write the new information in the format Name-Description, e.g. PainKillers-They are for pain aches: ");
						string[] strings = Console.ReadLine().Split('-', StringSplitOptions.RemoveEmptyEntries);
						if (strings.Length > 2)
						{
							Console.WriteLine("There is a additional data, please keep the format so try again!");
							break;
						}
						if (strings.Length == 0)
						{
							Console.WriteLine("Nothing has been updated!");
							break;
						}
						await controller.UpdateCategory(categoryId, strings);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "manufacturers":
					try
					{
						Console.WriteLine($"Here is a list of all manufacturers: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllManufacturers())}");
						Console.Write("Manufacturer to update: ");
						string manufacturer = Console.ReadLine();
						int manufacturerId = await controller.GetManufacturerId(manufacturer);
						Console.Write("Write the new information in the format Name-Email-Website-Phone, e.g. Bayer-bayer@bmail.com-bayer.com-+73729472: ");
						string[] strings = Console.ReadLine().Split('-', StringSplitOptions.RemoveEmptyEntries);
						if (strings.Length > 4)
						{
							Console.WriteLine("There is a additional data, please keep the format so try again!");
							break;
						}
						if (strings.Length == 0)
						{
							Console.WriteLine("Nothing has been updated!");
							break;
						}
						await controller.UpdateManufacturer(manufacturerId, strings);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "doctors":
					try
					{
						Console.WriteLine($"Here is a list of all doctors: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllDoctors())}");
						Console.Write("Manufacturer to update: ");
						string doctor = Console.ReadLine();
						int doctorId = await controller.GetDoctorId(doctor);
						Console.Write("Write the new information in the format Name-Email-Phone-Specialty, e.g. Dr. Smith-smith.@email.com-+73729472-Specializes in working with animals: ");
						string[] strings = Console.ReadLine().Split('-', StringSplitOptions.RemoveEmptyEntries);
						if (strings.Length > 4)
						{
							Console.WriteLine("There is a additional data, please keep the format so try again!");
							break;
						}
						if (strings.Length == 0)
						{
							Console.WriteLine("Nothing has been updated!");
							break;
						}
						await controller.UpdateManufacturer(doctorId, strings);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "patients":
					try
					{
						Console.WriteLine($"Here is a list of all patients: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllPatients())}");
						Console.Write("Patient to update: ");
						string patient = Console.ReadLine();
						int patientId = await controller.GetPatientId(patient);
						Console.Write("Write the new information in the format Name-Email-Phone-Date Of Birth, e.g. Bauman-bauman234@gmail.com-+73729472-11/09/2001: ");
						string[] strings = Console.ReadLine().Split('-', StringSplitOptions.RemoveEmptyEntries);
						if (strings.Length > 4)
						{
							Console.WriteLine("There is a additional data, please keep the format so try again!");
							break;
						}
						if (strings.Length == 0)
						{
							Console.WriteLine("Nothing has been updated!");
							break;
						}
						if (!DateTime.TryParseExact(strings[3], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
						{
							Console.WriteLine("Birth date wasn't in the correct format, try again with dd/MM/yyyy!");
							break;
						}
						await controller.UpdatePatient(patientId, strings);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "medicines":
					try
					{
						Console.WriteLine($"Here is a list of all medicines: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicines())}");
						Console.WriteLine($"Here is a list of all categories: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllCategories())}");
						Console.Write("Medicine to update: ");
						string medicine = Console.ReadLine();
						int medicineId = await controller.GetMedicineId(medicine);
						Console.Write("Write the new information in the format Name-Description-Dosage-CategoryName, e.g. Ibuprofen-They are for pain aches-100mg-Painkillers: ");
						string[] strings = Console.ReadLine().Split('-', StringSplitOptions.RemoveEmptyEntries);
						if (strings.Length > 4)
						{
							Console.WriteLine("There is a additional data, please keep the format so try again!");
							break;
						}
						if (strings.Length == 0)
						{
							Console.WriteLine("Nothing has been updated!");
							break;
						}
						if (!Regex.IsMatch(strings[2], @"^\d+mg$"))
						{
							Console.WriteLine("Dosage wasn't in the correct format, try again with this one e.g. 100mg!");
							break;
						}
						int categoryId = await controller.GetCategoryId(strings[3]);

						await controller.UpdateMedicine(medicineId, strings);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "manufacturer medicine":
					try
					{
						Console.WriteLine($"Here is a list of all manufacturers: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllManufacturers())}");
						Console.Write("Manufacturer whose medicine is going to be updated: ");
						string manufacturer = Console.ReadLine();
						int manufacturerId = await controller.GetManufacturerId(manufacturer);
						Console.WriteLine($"Here is a list of all medicines produced by {manufacturer}: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicinesByManufacturer(manufacturerId))}");
						Console.Write("Medicine to update: ");
						string medicine = Console.ReadLine();
						int medicineId = await controller.GetMedicineId(medicine);
						Console.Write("Write the new information in the format Price-Quantity, e.g. 10.99-100: ");
						string[] strings = Console.ReadLine().Split('-', StringSplitOptions.RemoveEmptyEntries);
						if (strings.Length > 2)
						{
							Console.WriteLine("There is a additional data, please keep the format so try again!");
							break;
						}
						if (strings.Length == 0)
						{
							Console.WriteLine("Nothing has been updated!");
							break;
						}
						if (!decimal.TryParse(strings[0], out decimal price))
						{
							Console.WriteLine("Price wasn't in the correct format, try again with a number!");
							break;
						}
						if (price <= 0)
						{
							Console.WriteLine("You cannot sell medicines for free, please provide valid price!");
							break;
						}
						if (!int.TryParse(strings[1], out int quantity))
						{
							Console.WriteLine("Quantity wasn't in the correct format, try again with a number!");
							break;
						}
						if (quantity <= 0)
						{
							Console.WriteLine("Quantity must be greater than 0, try again!");
							break;
						}
						strings[3] = manufacturerId.ToString();
						strings[4] = medicineId.ToString();

						await controller.UpdateManufacturerMedicine(strings);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "pharmacy medicine":
				case "prescriptions":
				case "orders":
				case "sales":
					Console.WriteLine("Sorry but any data from this table is not available for edit or deletion");
					break;
				default:
					Console.WriteLine("Seems like you didn't pay enough attention, try again next time with valid table name..");
					break;
			}
			#endregion
			break;
		case 4:
			#region GetDataFromTables
			string[] tables4 = { "categories", "manufacturers", "doctors", "patients", "medicines", "manufacturer medicine", "pharmacy medicine", "prescriptions", "orders", "sales" };
			Console.WriteLine($"Choose a table to get data from: {string.Join(", ", tables4)}");
			string table4 = Console.ReadLine();
			switch (table4)
			{
				case "categories":
					try
					{
						Console.WriteLine("\nCategories List:");
						Console.WriteLine(new string('-', 20 + 35));
						Console.WriteLine("| {0,-20} | {1,-35} |", "Category Name", "Description");
						Console.WriteLine(new string('-', 20 + 35));

						foreach (var category in await controller.GetAllCategoriesData())
						{
							Console.WriteLine("| {0,-20} | {1,-35} |", category.CategoryName, category.CategoryDescription);
						}

						Console.WriteLine(new string('-', 20 + 35));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "manufacturers":
					try
					{
						Console.WriteLine("\nManufacturers List:");
						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));
						Console.WriteLine("| {0,-20} | {1,-35} | {2,-25} | {3, -15} |", "Manufacturer Name", "Website", "Email", "Phone");
						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));

						foreach (var manufacturer in await controller.GetAllManufacturersData())
						{
							Console.WriteLine("| {0,-20} | {1,-35} | {2,-25} | {3, -15} |", manufacturer.ManufacturerName, manufacturer.Website, manufacturer.Email, manufacturer.Phone);
						}

						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "doctors":
					try
					{
						Console.WriteLine("\nDoctors List:");
						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));
						Console.WriteLine("| {0,-20} | {1,-15} | {2,-25} | {3, -35} |", "Doctor's Name", "Phone", "Email", "Specialty");
						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));

						foreach (var doctor in await controller.GetAllDoctorsData())
						{
							Console.WriteLine("| {0,-20} | {1,-15} | {2,-25} | {3, -35} |", doctor.DoctorName, doctor.Phone, doctor.Email, doctor.Specialty);
						}

						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "patients":
					try
					{
						Console.WriteLine("\nPatients List:");
						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));
						Console.WriteLine("| {0,-20} | {1,-15} | {2,-25} | {3, -35} |", "Doctor's Name", "Phone", "Email", "Birth Date");
						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));

						foreach (var patient in await controller.GetAllPatientsData())
						{
							Console.WriteLine("| {0,-20} | {1,-15} | {2,-25} | {3, -35} |", patient.PatientName, patient.Phone, patient.Email, patient.DateOfBirth.ToString("yyyy-MM-dd"));
						}

						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "medicines":
					try
					{
						Console.WriteLine("\nMedicines List:");
						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));
						Console.WriteLine("| {0,-20} | {1,-15} | {2,-25} | {3, -35} |", "Medicine's Name", "Category's Name", "Recommended Dosage", "Description");
						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));

						foreach (var medicine in await controller.GetAllMedicinesData())
						{
							Console.WriteLine("| {0,-20} | {1,-15} | {2,-25} | {3, -35} |", medicine.MedicineName, medicine.Category.CategoryName == null ? "No category" : medicine.Category.CategoryName, medicine.RecommendedDosage, medicine.Description);
						}

						Console.WriteLine(new string('-', 20 + 35 + 25 + 15));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "manufacturer medicine":
					try
					{
						Console.WriteLine("\nManufacturer Medicines List:");
						Console.WriteLine(new string('-', 20 + 15 + 15 + 15));
						Console.WriteLine("| {0,-20} | {1,-15} | {2,-15} | {3, -15} |", "Manufacturer's Name", "Medicine's Name", "Available Quantity", "Price");
						Console.WriteLine(new string('-', 20 + 15 + 15 + 15));

						foreach (var mm in await controller.GetAllManufacturerMedicinesData())
						{
							Console.WriteLine("| {0,-20} | {1,-15} | {2,-15} | {3, -15} |", mm.Manufacturer.ManufacturerName == null ? "No manufacturer" : mm.Manufacturer.ManufacturerName, mm.Medicine.MedicineName == null ? "No medicine" : mm.Medicine.MedicineName, mm.MadeQuantity, mm.ManufacturerPrice);
						}

						Console.WriteLine(new string('-', 20 + 15 + 15 + 15));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "pharmacy medicine":
					try
					{
						Console.WriteLine("\nPharmacy Medicines:");
						Console.WriteLine(new string('-', 80));
						Console.WriteLine("| {0,-25} | {1,-25} | {2,-10} | {2,-10} |", "Medicine", "Manufacturer", "Stock", "Price");
						Console.WriteLine(new string('-', 80));

						foreach (var pm in await controller.GetAllPharmacyMedicinesData())
						{
							var medicineName = pm.ManufacturerMedicine?.Medicine?.MedicineName ?? "No medicine";
							var manufacturerName = pm.ManufacturerMedicine?.Manufacturer?.ManufacturerName ?? "No manufacturer";

							Console.WriteLine("| {0,-25} | {1,-25} | {2,-10} | {2,-10} |", medicineName, manufacturerName, pm.StockQuantity, pm.PharmacyPrice);
						}

						Console.WriteLine(new string('-', 80));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "prescriptions":
					try
					{
						Console.WriteLine("\nPrescriptions List:");
						Console.WriteLine(new string('-', 100));
						Console.WriteLine("| {0,-20} | {0,-20} | {1,-20} | {2,-50} |", "ID", "Patient", "Doctor", "Medicines Prescribed");
						Console.WriteLine(new string('-', 100));

						foreach (var prescription in await controller.GetAllPrescriptionsData())
						{
							var patientName = prescription.Patient.PatientName;
							var doctorName = prescription.Doctor.DoctorName;
							var medicines = string.Join("\n", prescription.PrescriptionMedicines
								.Select(pm => $"Medicine Name: {pm.Medicine.MedicineName}, Dosage: {pm.Dosage}, Prescribed Quantity: {pm.PrescribedQuantity}"));

							Console.WriteLine("| {0,-20} | {0,-20} | {1,-20} | {2,-50} |", prescription.Id, patientName, doctorName, medicines);
						}

						Console.WriteLine(new string('-', 100));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "orders":
					try
					{
						var ordersWithPrices = await controller.GetAllOrdersData();

						Console.WriteLine("\nOrders List:");
						Console.WriteLine(new string('-', 120));
						Console.WriteLine("| {0,-20} | {0,-20} | {1,-25} | {2,-15} |", "ID", "Manufacturer", "Order Date", "Total Medicines");
						Console.WriteLine(new string('-', 120));

						foreach (var (order, orderMedicines) in ordersWithPrices)
						{
							Console.WriteLine("| {0,-20} | {0,-20} | {1,-25} | {2,-15} |", order.Id, order.Manufacturer.ManufacturerName, order.OrderDate.ToString("yyyy-MM-dd"), orderMedicines.Count);

							foreach (var (om, unitPrice, totalPrice) in orderMedicines)
							{
								string medInfo = $"-{om.Medicine.MedicineName}, Quantity: {om.BoughtQuantity}, Per Unit: {unitPrice:c}, Total: {totalPrice:c}";
								Console.WriteLine("| {0,-20} | {1,-25} | {2,-70} |", "", "", medInfo);
							}

							Console.WriteLine(new string('-', 120));
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				case "sales":
					try
					{
						Console.WriteLine("\nSales List:");
						Console.WriteLine(new string('-', 100));
						Console.WriteLine("| {0,-20} |", "Sold Date");
						Console.WriteLine(new string('-', 100));

						foreach (var sale in await controller.GetAllSalesData())
						{
							var patientName = sale.Prescription.Patient.PatientName;
							var doctorName = sale.Prescription.Doctor.DoctorName;
							var medicines = string.Join("\n", sale.Prescription.PrescriptionMedicines
								.Select(pm => $"Medicine Name: {pm.Medicine.MedicineName}, Dosage: {pm.Dosage}, Prescribed Quantity: {pm.PrescribedQuantity}"));

							Console.WriteLine("| {0,-20} | {0,-20} | {0,-20} | {1,-20} | {2,-50} |", sale.SaleDate.ToString("yyyy-MM-dd"), sale.Prescription.Id, patientName, doctorName, medicines);
						}

						Console.WriteLine(new string('-', 100));
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
					break;
				default:
					Console.WriteLine("Invalid table, try again!");
					break;
			}
			#endregion
			break;
		case 5:
			try
			{
				Console.WriteLine("Here is a list of all medicines in the pharmacy, choose one to see if there is a user who had used it: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicinesInPharmacy())}");
				Console.Write("Enter medicine name: ");
				string medName = Console.ReadLine();

				int medId = await controller.GetMedicineId(medName);

				foreach (var p in await controller.GetPatientsByMedicineName(medId))
				{
					Console.WriteLine($"Patient: {p}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			break;
		case 6:
			try
			{
				Console.Write("Write the year you want to see the total sales for: ");
				int year = int.Parse(Console.ReadLine());
				if (!int.TryParse(year.ToString(), out year))
				{
					Console.WriteLine("Invalid type for year, try again with the correct one!");
					break;
				}

				Console.WriteLine($"Total sales: {await controller.GetTotalSalesByYear(year):c}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

			}
			break;
		case 7:
			try
			{
				Console.WriteLine("Choose a patient you wish to see his last prescription and the medicine in it: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllPatients())}");
				string patient = Console.ReadLine();
				int patientId = await controller.GetPatientId(patient);

				Console.WriteLine(await controller.GetLastPrescriptionByPatient(patientId));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			break;
		case 8:
			try
			{
				Console.WriteLine("Choose a manufacturer you wish to see the orders made to him: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllManufacturers())}");
				string manufacturer = Console.ReadLine();
				int manId = await controller.GetManufacturerId(manufacturer);

				Console.WriteLine(await controller.GetAllOrdersByManufacturer(manId));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			break;
		case 9:
			try
			{
				Console.WriteLine("Choose a medicine whose prescriptions you wish to see it's in: " + "\n" + $"{string.Join(Environment.NewLine + "-", await controller.GetAllMedicinesInPharmacy())}");
				string medicine = Console.ReadLine();
				int medId = await controller.GetMedicineId(medicine);
				Console.WriteLine(await controller.GetPrescriptionsByMedicineName(medId));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			break;
		case 10:
			try
			{
				Console.WriteLine(await controller.GetUnorderedMedicines());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			break;
		case 11:
			try
			{
				Console.WriteLine(await controller.GetLowOnStockMedicinesInPharmacy());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			break;
		case 12:
			try
			{
				Console.WriteLine(await controller.GetOldPatients());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			break;
		case 13:
			try
			{
				Console.WriteLine(await controller.GetOrdersInTheLast30Days());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			break;
		case 14:
			try
			{
				Console.WriteLine(await controller.GetAllManufacturersWithEmailEndingInBg());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			break;
		case 0:
			Console.WriteLine("Exiting the program...");
			return;
		default:
			Console.WriteLine("Invalid command, choose one from the list above!");
			break;
	}
}
