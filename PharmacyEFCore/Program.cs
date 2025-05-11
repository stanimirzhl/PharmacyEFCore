using Pharmacy.Core;
using Pharmacy.Data.Data;
using Pharmacy.Data.Data.Models;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

var context = new PharmacyDbContext();
var controller = new PharmacyController(context);


while (true)
{
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
                        int orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId2);
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
                                int orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId2);
                                await controller.AddOrderMedicine(orderId, medicineId2, quantity2);
                                Console.WriteLine("Order successfully placed!");
                            }
                            catch (Exception inexistant)
                            {
                                Console.WriteLine(inexistant.Message);
                                int orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId2);
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
                        await controller.AddPrescription(doctorId, patientId, date);
                        string prescriptionId = await controller.GetLastPrescriptionId();
                        Console.WriteLine($"Write the name of the medicines {patient} is getting prescribed, here is a list of all medicines in the pharmacy to help you: " + "\n" + $"{string.Join(Environment.NewLine + "-", pharmacyMedicines)}");
                        Console.WriteLine("Write medicine name, quantity and dosage for the patient until the word 'That's all', write them on one line separated by line a dash (e.g., example-example), Dont forget the format for dosage is number + mg e.g. 100mg");
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
                                        int orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId);
                                        await controller.AddOrderMedicine(orderId, medicineIdToParse, quantityToParse);
                                        Console.WriteLine("Order successfully placed!");
                                    }
                                    catch (Exception inexistant)
                                    {
                                        Console.WriteLine(inexistant.Message);
                                        int orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId);
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
                                        int orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId);
                                        await controller.AddOrderMedicine(orderId, medicineIdToParse, quantityToParse);
                                        Console.WriteLine("Order successfully placed!");
                                    }
                                    catch (Exception inexistant)
                                    {
                                        Console.WriteLine(inexistant.Message);
                                        int orderId = await controller.GetLastOrderIdForManufacturer(manufacturerId);
                                        await controller.DeleteOrder(orderId);
                                    }
                                }
                            }
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
                        int orderId = 0;
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
                                if (orderId == 0)
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
                        if (orderId == 0)
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
            break;
        case 0:
            Console.WriteLine("Exiting the program...");
            return;
        default:
            Console.WriteLine("Invalid command, choose one from the list above!");
            break;
    }
}
