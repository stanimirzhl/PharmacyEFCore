using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using Pharmacy.Data.Data;
using Pharmacy.Data.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pharmacy.Core
{
    public class PharmacyController
    {
        private class NonExistentEntity : Exception
        {
            public NonExistentEntity(string? message) : base(message)
            {
            }
        }
        private string PrescriptionIdGenerator()
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy");
            string random = Guid.NewGuid().ToString("N")[..6].ToUpper();
            return $"℞-{random}-{date}";
        }
        private string OrderIdGenerator()
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy");
            string random = Guid.NewGuid().ToString("N")[..6].ToUpper();
            return $"ORD-{date}-{random}";
        }
        PharmacyDbContext context;

        string message = "Entity with that name already exists, try to add something different!";

        public PharmacyController(PharmacyDbContext context)
        {
            this.context = context;
        }

        #region AddMethods
        public async Task AddCategory(string name, string description)
        {
            var category = await context.Categories.Where(x => x.IsDeleted == false).FirstOrDefaultAsync(x => x.CategoryName == name);
            if (category != null)
            {
                throw new ArgumentException(message);
            }
            await context.Categories.AddAsync(new Category { CategoryName = name, CategoryDescription = description });
            await context.SaveChangesAsync();
        }

        public async Task AddManufacturer(string name, string email, string website, string phone)
        {
            var manufacturer = await context.Manufacturers.Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.ManufacturerName == name || x.Email == email || x.Website == website || x.Phone == phone);
            if (manufacturer is not null)
            {
                throw new ArgumentException(message);
            }

            await context.Manufacturers.AddAsync(new Manufacturer
            {
                ManufacturerName = name,
                Email = email,
                Phone = phone,
                Website = website
            });
            await context.SaveChangesAsync();
        }

        public async Task AddDoctor(string name, string email, string phone, string specialty)
        {
            var doctor = await context.Doctors.Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.DoctorName == name || x.Email == email || x.Phone == phone);
            if (doctor is not null)
            {
                throw new ArgumentException(message);
            }

            await context.Doctors.AddAsync(new Doctor
            {
                DoctorName = name,
                Email = email,
                Phone = phone,
                Specialty = specialty
            });
            await context.SaveChangesAsync();
        }

        public async Task AddPatient(string name, string email, string phone, DateTime birth)
        {
            var patient = await context.Patients.Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.PatientName == name || x.Email == email || x.Phone == phone);
            if (patient is not null)
            {
                throw new ArgumentException(message);
            }

            await context.Patients.AddAsync(new Patient
            {
                PatientName = name,
                Email = email,
                Phone = phone,
                DateOfBirth = birth
            });
            await context.SaveChangesAsync();
        }

        public async Task AddMedicine(string name, string description, int id, string dosage)
        {
            var medicine = await context.Medicines.Where(x => x.IsDeleted == false).FirstOrDefaultAsync(x => x.MedicineName == name);
            if (medicine is not null)
            {
                throw new ArgumentException(message);
            }
            var category = await context.Categories
                .Include(x => x.Medicines)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Category by the given name cannot be found, try again with valid one!");

            var newMed = new Medicine
            {
                MedicineName = name,
                Description = description,
                CategoryId = id,
                RecommendedDosage = dosage
            };

            await context.Medicines.AddAsync(newMed);
            category.Medicines.Add(newMed);
            await context.SaveChangesAsync();
        }

        public async Task AddManufacturerMedicine(int medicineId, int manId, decimal price, int quantity)
        {
            var mm = await context.ManufacturerMedicines.Where(x => x.IsDeleted == false).FirstOrDefaultAsync(x => x.ManufacturerId == manId && x.MedicineId == medicineId);
            if (mm is not null)
            {
                throw new ArgumentException(message);
            }
            var medicine = await context.Medicines.Where(x => x.IsDeleted == false)
                .Include(x => x.ManufacturerMedicines)
                .FirstOrDefaultAsync(x => x.Id == medicineId) ?? throw new NonExistentEntity("Medicine by the given name cannot be found, try again with valid one!");
            var manufacturer = await context.Manufacturers.Where(x => x.IsDeleted == false)
                .Include(x => x.ManufacturerMedicines)
                .FirstOrDefaultAsync(x => x.Id == manId) ?? throw new NonExistentEntity("Manufacturer by the given name cannot be found, try again with valid one!");

            var newMM = new ManufacturerMedicine
            {
                ManufacturerId = manId,
                MedicineId = medicineId,
                ManufacturerPrice = price,
                MadeQuantity = quantity
            };

            await context.ManufacturerMedicines.AddAsync(newMM);
            medicine.ManufacturerMedicines.Add(newMM);
            manufacturer.ManufacturerMedicines.Add(newMM);
            await context.SaveChangesAsync();
        }

        public async Task RefillManufacturerMedicineStock(int manufacturerId, int medicineId, int quantity)
        {
            var mm = await context.ManufacturerMedicines.Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.ManufacturerId == manufacturerId && x.MedicineId == medicineId);

            mm.MadeQuantity += quantity;
            await context.SaveChangesAsync();
        }

        public async Task AddPharmacyMedicine(int manufacturerId, int medicineId, int quantity, decimal price)
        {
            var medicine = await context.Medicines.Where(x => x.IsDeleted == false)
                .Include(x => x.PharmacyMedicines)
               .FirstOrDefaultAsync(x => x.Id == medicineId) ?? throw new NonExistentEntity("Medicine by the given name cannot be found, try again with valid one!");

            var manufacturerMedicineId = await context.ManufacturerMedicines
                .Where(x => x.IsDeleted == false)
                .Where(x => x.ManufacturerId == manufacturerId && x.MedicineId == medicineId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync() ?? throw new NonExistentEntity("The manufacturer doesn't produce the desired medicine yet, try with different one!");

            var manufacturerMedicine = await context.ManufacturerMedicines
                .Where(x => x.IsDeleted == false)
                //.Include(mm => mm.Medicine)
                //.Include(mm => mm.Manufacturer)
                .FirstOrDefaultAsync(mm => mm.Id == manufacturerMedicineId);

            if (manufacturerMedicine.MadeQuantity < quantity)
            {
                throw new ArgumentOutOfRangeException("The manufacturer currently does not have enough quantity to satisfy the needs, try again with lower amount or later!");
            }

            if (price < manufacturerMedicine.ManufacturerPrice)
            {
                throw new ArgumentException("Pharmacy cannot sell below the manufacturer's price!");
            }

            var existingMed = await context.PharmacyMedicines
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.ManufacturerMedicineId == manufacturerMedicineId);

            if (existingMed is not null)
            {
                if (existingMed.StockQuantity == 0)
                {
                    throw new ArgumentNullException("This medicine already exists but is out of stock, please restock and try again!");
                }
                else
                {
                    throw new ArgumentException("The medicine by the given manufacturer already exists and it is in stock, use it!");
                }
            }

            var pm = new PharmacyMedicine
            {
                ManufacturerMedicineId = manufacturerMedicineId,
                StockQuantity = quantity,
                PharmacyPrice = price
            };

            await context.PharmacyMedicines.AddAsync(pm);
            medicine.PharmacyMedicines.Add(pm);
            await context.SaveChangesAsync();
        }

        public async Task AddPrescription(int patiendId, int doctorId, DateTime prescribed)
        {
            var patient = await context.Patients.Where(x => x.IsDeleted == false)
                .Include(x => x.Prescriptions)
                .FirstOrDefaultAsync(x => x.Id == patiendId) ?? throw new NonExistentEntity("Patient by the given name cannot be found, try again with valid one!");
            var doctor = await context.Doctors.Where(x => x.IsDeleted == false)
                .Include(x => x.Prescriptions)
                .FirstOrDefaultAsync(x => x.Id == doctorId) ?? throw new NonExistentEntity("Doctor by the given name cannot be found, try again with valid one!");

            var prescription = new Prescription
            {
                Id = PrescriptionIdGenerator(),
                PatientId = patiendId,
                DoctorId = doctorId,
                PrescribedAt = prescribed
            };

            await context.Prescriptions.AddAsync(prescription);
            patient.Prescriptions.Add(prescription);
            doctor.Prescriptions.Add(prescription);
            await context.SaveChangesAsync();
        }

        public async Task AddPrescriptionMedicine(string prescriptionId, int medicineId, string dosage, int quantity)
        {
            var prescription = await context.Prescriptions.Where(x => x.IsDeleted == false)
                .Include(x => x.PrescriptionMedicines)
                .FirstOrDefaultAsync(x => x.Id == prescriptionId) ?? throw new NonExistentEntity("Prescription by the given Id cannot be found, try again with valid one!");
            var medicine = await context.Medicines.Where(x => x.IsDeleted == false)
                .Include(x => x.PrescriptionMedicines)
                .FirstOrDefaultAsync(x => x.Id == medicineId) ?? throw new NonExistentEntity("Medicine by the given name cannot be found, try again with valid one!");
            var pms = await context.PharmacyMedicines
                .Where(x => x.IsDeleted == false)
                .Where(x => x.ManufacturerMedicine.ManufacturerId == medicineId)
                .Where(x => x.StockQuantity > 0)
                .Include(x => x.ManufacturerMedicine)
                .OrderBy(x => x.ManufacturerMedicine.ManufacturerPrice)
                .FirstAsync() ?? throw new NonExistentEntity("The desired medicine is not available!");

            if (pms.StockQuantity < quantity)
            {
                if (pms.StockQuantity == 0)
                {
                    var ex = new ArgumentNullException("This medicine is out of stock, please restock and try again!");
                    ex.Data["manufacturerId"] = pms.ManufacturerMedicine.ManufacturerId;
                    throw ex;
                }
                var ex2 = new ArgumentOutOfRangeException("The pharmacy currently does not have enough quantity to satisfy the needs, try again with lower amount or later!");
                ex2.Data["manufacturerId"] = pms.ManufacturerMedicine.ManufacturerId;
                throw ex2;
            }

            pms.StockQuantity -= quantity;

            var pm = new PrescriptionMedicine
            {
                PrescriptionId = prescriptionId,
                MedicineId = medicineId,
                Dosage = dosage,
                PrescribedQuantity = quantity
            };

            await context.PrescriptionMedicines.AddAsync(pm);
            prescription.PrescriptionMedicines.Add(pm);
            medicine.PrescriptionMedicines.Add(pm);
            await context.SaveChangesAsync();
        }

        public async Task AddOrder(int id)
        {
            var manufacturer = await context.Manufacturers.Where(x => x.IsDeleted == false)
                .Include(x => x.Orders)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Manufacturer by the given name cannot be found, try again with valid one!");

            var order = new Order
            {
                Id = OrderIdGenerator(),
                ManufacturerId = id
            };

            await context.Orders.AddAsync(order);
            manufacturer.Orders.Add(order);
            await context.SaveChangesAsync();
        }

        public async Task AddOrderMedicine(string orderId, int medicineId, int quantity)
        {
            var order = await context.Orders
                .Where(x => x.IsDeleted == false)
                .Include(x => x.OrderMedicines)
                .Include(x => x.Manufacturer)
                .FirstOrDefaultAsync(x => x.Id == orderId) ?? throw new NonExistentEntity("Order by the given Id cannot be found, try again with valid one!");
            var medicine = await context.Medicines
                .Where(x => x.IsDeleted == false)
                .Include(x => x.OrderMedicines)
                .FirstOrDefaultAsync(x => x.Id == medicineId) ?? throw new NonExistentEntity("Medicine by the given name cannot be found, try again with valid one!");

            var medicineQuantity = await context.ManufacturerMedicines.Where(x => x.IsDeleted == false)
                .Where(x => x.MedicineId == medicineId && x.ManufacturerId == order.ManufacturerId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync() ?? throw new NonExistentEntity("The manufacturer doesn't produce the desired medicine yet, try with different one!");

            var manufacturerMedicine = await context.ManufacturerMedicines
                .Where(x => x.IsDeleted == false)
                //.Include(mm => mm.Medicine)
                //.Include(mm => mm.Manufacturer)
                .FirstOrDefaultAsync(mm => mm.Id == medicineQuantity);

            var pm = await context.PharmacyMedicines
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.ManufacturerMedicineId == medicineQuantity) ?? throw new InvalidOperationException("The pharmacy currently does not offer the desired medicine, try again later!");

            if (manufacturerMedicine.MadeQuantity < quantity)
            {
                throw new ArgumentOutOfRangeException("The manufacturer currently does not have enough quantity to satisfy the needs, try again with lower amount of later!");
            }

            pm.StockQuantity += quantity;
            manufacturerMedicine.MadeQuantity -= quantity;

            var om = new OrderMedicine
            {
                OrderId = orderId,
                MedicineId = medicineId,
                BoughtQuantity = quantity
            };

            await context.OrderMedicines.AddAsync(om);
            order.OrderMedicines.Add(om);
            medicine.OrderMedicines.Add(om);
            await context.SaveChangesAsync();
        }

        public async Task AddSale(string id)
        {
            var prescription = await context.Prescriptions
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Prescription by the given Id cannot be found, try again with valid one!");

            prescription.HasUsed = true;

            await context.Sales.AddAsync(new Sale
            {
                PrescriptionId = id
            });
            await context.SaveChangesAsync();
        }
        #endregion

        #region DeleteMethods
        public async Task DeleteCategory(int id)
        {
            var category = await context.Categories
                .Where(x => x.IsDeleted == false)
                .Include(x => x.Medicines)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Category by the given name cannot be found, try again with valid one!");

            category.IsDeleted = true;
            await context.SaveChangesAsync();
        }
        public async Task DeleteManufacturer(int id)
        {
            var manufacturer = await context.Manufacturers
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Manufacturer by the given name cannot be found, try again with valid one!");

            manufacturer.IsDeleted = true;
            await context.SaveChangesAsync();
        }
        public async Task DeleteDoctor(int id)
        {
            var doctor = await context.Doctors
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Doctor by the given name cannot be found, try again with valid one!");

            doctor.IsDeleted = true;
            await context.SaveChangesAsync();
        }
        public async Task DeletePatient(int id)
        {
            var patient = await context.Patients
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Patient by the given name cannot be found, try again with valid one!");

            patient.IsDeleted = true;
            await context.SaveChangesAsync();
        }
        public async Task DeleteMedicine(int id)
        {
            var medicine = await context.Medicines
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Medicine by the given name cannot be found, try again with valid one!");

            medicine.IsDeleted = true;
            await context.SaveChangesAsync();
        }
        public async Task DeleteManufacturerMedicine(int manufacturerId, int medicineId)
        {
            var mm = await context.ManufacturerMedicines
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.ManufacturerId == manufacturerId && x.MedicineId == medicineId) ?? throw new NonExistentEntity("Manufacturer medicine by the given name cannot be found, try again with valid one!");

            mm.IsDeleted = true;
            await context.SaveChangesAsync();
        }
        public async Task DeletePharmacyMedicine(int manufacturerId, int medicineId)
        {
            var pm = await context.PharmacyMedicines
                .Where(x => x.IsDeleted == false)
                .Include(x => x.ManufacturerMedicine)
                .FirstOrDefaultAsync(x => x.ManufacturerMedicine.ManufacturerId == manufacturerId && x.ManufacturerMedicine.MedicineId == medicineId) ?? throw new NonExistentEntity("Pharmacy medicine by the given medicine cannot be found, try again with valid one!");

            pm.IsDeleted = true;
            await context.SaveChangesAsync();
        }
        public async Task<List<int>> CheckQuantityPharmacy(int id)
        {
            var medicinesInPharmacy = await context.PharmacyMedicines
                .Where(x => x.IsDeleted == false)
                .Include(x => x.ManufacturerMedicine)
                .ToListAsync() ?? throw new NonExistentEntity("Medicine by the given name cannot be found, try again with valid one!");

            return medicinesInPharmacy
                .Where(x => x.ManufacturerMedicine.MedicineId == id)
                .Where(x => x.StockQuantity == 0)
                .Select(x => x.ManufacturerMedicine.ManufacturerId)
                .ToList();
        }
        public async Task<List<int>> CheckQuantityManufacturer(int id)
        {
            var medicinesInManufacturer = await context.ManufacturerMedicines
                .Where(x => x.IsDeleted == false)
                .Include(x => x.Medicine)
                .ToListAsync() ?? throw new NonExistentEntity("Medicine by the given name cannot be found, try again with valid one!");

            return medicinesInManufacturer
                .Where(x => x.MedicineId == id)
                .Where(x => x.MadeQuantity == 0)
                .Select(x => x.Id)
                .ToList();
        }
        public async Task DeletePrescription(string id)
        {
            var prescription = await context.Prescriptions
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Prescription by the given Id cannot be found, try again with valid one!");

            prescription.IsDeleted = true;
            prescription.DeletedAt = DateTime.Now;
            await context.SaveChangesAsync();
        }
        public async Task DeletePrescriptionMedicine(string id)
        {
            var pm = await context.PrescriptionMedicines
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.PrescriptionId == id) ?? throw new NonExistentEntity("Prescription medicine by the given Id cannot be found, try again with valid one!");

            pm.IsDeleted = true;
            await context.SaveChangesAsync();
        }
        public async Task DeleteOrder(string id)
        {
            var order = await context.Orders
              .Where(x => x.IsDeleted == false)
              .Include(x => x.OrderMedicines)
              .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Order by the given Id cannot be found, try again with valid one!");

            order.IsDeleted = true;
            order.DeletedAt = DateTime.Now;
            await context.SaveChangesAsync();
        }
        public async Task DeleteOrderMedicine(string id)
        {
            var om = await context.OrderMedicines
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.OrderId == id) ?? throw new NonExistentEntity("Order medicine by the given Id cannot be found, try again with valid one!");

            om.IsDeleted = true;
            await context.SaveChangesAsync();
        }
        #endregion

        #region UpdateMethods
        public async Task UpdateCategory(int id, string[] strings)
        {
            var category = await context.Categories
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Category by the given name cannot be found, try again with valid one!");

            if (strings[0] != null)
            {
                category.CategoryName = strings[0];
            }
            if (strings[1] != null)
            {
                category.CategoryDescription = strings[1];
            }

            await context.SaveChangesAsync();
        }
        public async Task UpdateManufacturer(int id, string[] strings)
        {
            var manufacturer = await context.Manufacturers
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Manufacturer by the given name cannot be found, try again with valid one!");

            if (strings[0] != null)
            {
                manufacturer.ManufacturerName = strings[0];
            }
            if (strings[1] != null)
            {
                manufacturer.Email = strings[1];
            }
            if (strings[2] != null)
            {
                manufacturer.Phone = strings[2];
            }
            if (strings[3] != null)
            {
                manufacturer.Website = strings[3];
            }

            await context.SaveChangesAsync();
        }
        public async Task UpdateDoctor(int id, string[] strings)
        {
            var doctor = await context.Doctors
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Doctor by the given name cannot be found, try again with valid one!");

            if (strings[0] != null)
            {
                doctor.DoctorName = strings[0];
            }
            if (strings[1] != null)
            {
                doctor.Email = strings[1];
            }
            if (strings[2] != null)
            {
                doctor.Phone = strings[2];
            }
            if (strings[3] != null)
            {
                doctor.Specialty = strings[3];
            }

            await context.SaveChangesAsync();
        }
        public async Task UpdatePatient(int id, string[] strings)
        {
            var patient = await context.Patients
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Patient by the given name cannot be found, try again with valid one!");

            if (strings[0] != null)
            {
                patient.PatientName = strings[0];
            }
            if (strings[1] != null)
            {
                patient.Email = strings[1];
            }
            if (strings[2] != null)
            {
                patient.Phone = strings[2];
            }
            if (strings[3] != null)
            {
                patient.DateOfBirth = DateTime.Parse(strings[3]);
            }

            await context.SaveChangesAsync();
        }
        public async Task UpdateMedicine(int id, string[] strings)
        {
            var medicine = await context.Medicines
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Medicine by the given name cannot be found, try again with valid one!");

            if (strings[0] != null)
            {
                medicine.MedicineName = strings[0];
            }
            if (strings[1] != null)
            {
                medicine.Description = strings[1];
            }
            if (strings[2] != null)
            {
                medicine.RecommendedDosage = strings[2];
            }
            if (strings[3] != null)
            {
                medicine.CategoryId = int.Parse(strings[3]);
            }

            await context.SaveChangesAsync();
        }
        public async Task UpdateManufacturerMedicine(string[] strings)
        {
            int manufacturerId = int.Parse(strings[3]);
            int medicineId = int.Parse(strings[4]);

            var mm = await context.ManufacturerMedicines
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.ManufacturerId == manufacturerId && x.MedicineId == medicineId) ?? throw new NonExistentEntity("Manufacturer medicine cannot be found, try again with valid one!");

            if (strings[0] != null)
            {
                mm.ManufacturerPrice = decimal.Parse(strings[0]);
            }
            if (strings[1] != null)
            {
                mm.MadeQuantity = int.Parse(strings[1]);
            }

            await context.SaveChangesAsync();
        }
        #endregion

        #region GetAll/Ids
        public async Task<List<string>> GetAllCategories()
        {
            var categories = await context.Categories
                .Where(x => x.IsDeleted == false)
                .ToListAsync();

            if (categories.Count == 0)
            {
                throw new NonExistentEntity("There are no categories in the database, try to add some first!");
            }
            return categories.Select((x, i) =>
                i == 0 ? $"-{x.CategoryName}" : x.CategoryName).ToList();
        }
        public async Task<int> GetCategoryId(string name)
        {
            var category = await context.Categories
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.CategoryName == name) ?? throw new NonExistentEntity("Category by the given name cannot be found, try again with valid one!");

            return category.Id;
        }
        public async Task<List<string>> GetAllManufacturers()
        {
            var manufacturers = await context.Manufacturers
                .Where(x => x.IsDeleted == false)
                .ToListAsync();

            if (manufacturers.Count == 0)
            {
                throw new NonExistentEntity("There are no manufacturers in the database, try to add some first!");
            }
            return manufacturers.Select((x, i) =>
                i == 0 ? $"-{x.ManufacturerName}" : x.ManufacturerName).ToList();
        }
        public async Task<int> GetManufacturerId(string name)
        {
            var manufacturer = await context.Manufacturers
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.ManufacturerName == name) ?? throw new NonExistentEntity("Manufacturer by the given name cannot be found, try again with valid one!");

            return manufacturer.Id;
        }
        public async Task<string> GetManufacturerNameById(int id)
        {
            var manufacturer = await context.Manufacturers
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Manufacturer by the given Id cannot be found, try again with valid one!");
            return manufacturer.ManufacturerName;
        }
        public async Task<List<string>> GetAllDoctors()
        {
            var doctors = await context.Doctors
                .Where(x => x.IsDeleted == false)
                .ToListAsync();

            if (doctors.Count == 0)
            {
                throw new NonExistentEntity("There are no doctors in the database, try to add some first!");
            }
            return doctors.Select((x, i) =>
                i == 0 ? $"-{x.DoctorName}" : x.DoctorName).ToList();
        }
        public async Task<int> GetDoctorId(string name)
        {
            var doctor = await context.Doctors
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.DoctorName == name) ?? throw new NonExistentEntity("Doctor by the given name cannot be found, try again with valid one!");

            return doctor.Id;
        }
        public async Task<List<string>> GetAllPatients()
        {
            var patients = await context.Patients
                .Where(x => x.IsDeleted == false)
                .ToListAsync();

            if (patients.Count == 0)
            {
                throw new NonExistentEntity("There are no patients in the database, try to add some first!");
            }
            return patients.Select((x, i) =>
                i == 0 ? $"-{x.PatientName}" : x.PatientName).ToList();
        }
        public async Task<int> GetPatientId(string name)
        {

            var patient = await context.Patients
                 .Where(x => x.IsDeleted == false)
                 .FirstOrDefaultAsync(x => x.PatientName == name) ?? throw new NonExistentEntity("Patient by the given name cannot be found, try again with valid one!");

            return patient.Id;
        }
        public async Task<List<string>> GetAllMedicines()
        {
            var medicines = await context.Medicines
                .Where(x => x.IsDeleted == false)
                .ToListAsync();

            if (medicines.Count == 0)
            {
                throw new NonExistentEntity("There are no medicines in the database, try to add some first!");
            }
            return medicines.Select((x, i) =>
                i == 0 ? $"-{x.MedicineName}" : x.MedicineName).ToList();
        }
        public async Task<int> GetMedicineId(string name)
        {
            var medicine = await context.Medicines
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.MedicineName == name) ?? throw new NonExistentEntity("Medicine by the given name cannot be found, try again with valid one!");

            return medicine.Id;
        }
        public async Task<List<string>> GetAllMedicinesByManufacturer(int manufacturerId)
        {
            var mms = await context.ManufacturerMedicines
                .Where(x => x.IsDeleted == false)
                .Where(x => x.ManufacturerId == manufacturerId)
                .Include(x => x.Medicine)
                .Select((x, i) => i == 0 ? $"-{x.Medicine.MedicineName} - In Stock: {x.MadeQuantity}" : $"{x.Medicine.MedicineName} - In Stock: {x.MadeQuantity}")
                .ToListAsync();

            if (mms.Count == 0)
            {
                throw new NonExistentEntity("This manufacturer doesn't offer any medicines.");
            }

            return mms;
        }
        public async Task<List<string>> GetAllMedicinesInPharmacy(string? type = null)
        {
            var medicines = await context.PharmacyMedicines
                .Where(x => x.IsDeleted == false)
                .Include(x => x.ManufacturerMedicine)
                .ThenInclude(x => x.Medicine)
                .ToListAsync();
            if (medicines.Count == 0)
            {
                throw new NonExistentEntity("There are no medicines in the pharmacy, try to add some first!");
            }

            if (type == "dose")
            {
                return medicines
                .Select((x, i) => i == 0 ? $"-{x.ManufacturerMedicine.Medicine.MedicineName} - Recommended dose: {x.ManufacturerMedicine.Medicine.RecommendedDosage}" : $"{x.ManufacturerMedicine.Medicine.MedicineName} - Recommended dose: {x.ManufacturerMedicine.Medicine.RecommendedDosage}")
                .ToList();
            }
            if (type == "manufacturer")
            {
                return medicines
                    .Select((x, i) => i == 0 ? $"-{x.ManufacturerMedicine.Medicine.MedicineName} - Manufacturer: {x.ManufacturerMedicine.Manufacturer.ManufacturerName}" : $"{x.ManufacturerMedicine.Medicine.MedicineName} - Manufacturer: {x.ManufacturerMedicine.Manufacturer.ManufacturerName}")
                    .ToList();
            }

            return medicines
                .Select((x, i) => i == 0 ? $"-{x.ManufacturerMedicine.Medicine.MedicineName} - In Stock: {x.StockQuantity}" : $"{x.ManufacturerMedicine.Medicine.MedicineName} - In Stock: {x.StockQuantity}")
                .ToList();
        }
        public async Task<string> GetLastOrderIdForManufacturer(int manufacturerId)
        {
            var order = await context.Orders
                .Where(x => x.IsDeleted == false)
                .Where(x => x.ManufacturerId == manufacturerId)
                .LastAsync();
            return order.Id;
        }
        public async Task<List<string>> GetAllOrders()
        {
            var orders = await context.Orders
                .Where(x => x.IsDeleted == false)
                .Include(x => x.OrderMedicines)
                .ThenInclude(x => x.Medicine)
                .ToListAsync();
            if (orders.Count == 0)
            {
                throw new NonExistentEntity("There are no orders in the database, try to add some first!");
            }
            return orders.Select((x, i) =>
                i == 0 ? $"-{x.Id}-Items: {x.OrderMedicines.Select(om => $"{om.Medicine.MedicineName}" + "\n")}"
                 : $"{x.Id}-Items: {x.OrderMedicines.Select(om => $"{om.Medicine.MedicineName}" + "\n")}")
                .ToList();
        }
        public async Task<List<string>> GetAllPrescriptionIds()
        {
            var prescriptions = await context.Prescriptions
                .Where(x => x.IsDeleted == false)
                .Where(x => x.HasUsed == false)
                .ToListAsync();
            if (prescriptions.Count == 0)
            {
                throw new NonExistentEntity("There are no prescriptions in the database, try to add some first!");
            }
            return prescriptions.Select((x, i) =>
                i == 0 ? $"-{x.Id}" : x.Id).ToList();
        }
        public async Task<(string, List<PrescriptionMedicine>)> GetLastDeletedPrescriptionIdAndCollection()
        {
            var prescription = await context.Prescriptions
                 .Where(p => p.IsDeleted && p.DeletedAt != null)
                 .Include(x => x.PrescriptionMedicines)
                 .OrderByDescending(p => p.DeletedAt)
                 .FirstOrDefaultAsync();

            return (prescription.Id, prescription.PrescriptionMedicines.ToList());
        }
        public async Task<(string, List<OrderMedicine>)> GetLastDeletedOrderIdAndCollection()
        {
            var order = await context.Orders
                 .Where(p => p.IsDeleted && p.DeletedAt != null)
                 .Include(x => x.OrderMedicines)
                 .OrderByDescending(p => p.DeletedAt)
                 .FirstOrDefaultAsync();

            return (order.Id, order.OrderMedicines.ToList());
        }
        public async Task<string> GetPrescriptionId(string id)
        {
            string customId = $"℞-{id}";
            var prescription = await context.Prescriptions.Where(x => x.IsDeleted == false)
                  .FirstOrDefaultAsync(x => x.Id == customId) ?? throw new NonExistentEntity("Prescription by the given Id cannot be found, try again with valid one!");

            return prescription.Id;
        }
        public async Task<string> GetOrderId(string id)
        {
            string customId = $"ORD-{id}";
            var order = await context.Orders.Where(x => x.IsDeleted == false)
                  .FirstOrDefaultAsync(x => x.Id == customId) ?? throw new NonExistentEntity("Order by the given Id cannot be found, try again with valid one!");

            return order.Id;
        }
        public async Task<string> GetLastPrescriptionId()
        {
            var prescription = await context.Prescriptions
                .Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.PrescribedAt)
                .FirstAsync();

            return prescription.Id;
        }

        #endregion

        #region GetAllData
        public async Task<List<Category>> GetAllCategoriesData()
        {
            var categories = await context.Categories
                .Where(x => x.IsDeleted == false)
                .ToListAsync();

            if (categories.Count == 0)
            {
                throw new NonExistentEntity("There are no categories in the database, try to add some first!");
            }
            return categories;
        }
        public async Task<List<Manufacturer>> GetAllManufacturersData()
        {
            var manufacturers = await context.Manufacturers
                .Where(x => x.IsDeleted == false)
                .ToListAsync();

            if (manufacturers.Count == 0)
            {
                throw new NonExistentEntity("There are no manufacturers in the database, try to add some first!");
            }
            return manufacturers;
        }
        public async Task<List<Doctor>> GetAllDoctorsData()
        {
            var doctors = await context.Doctors
                .Where(x => x.IsDeleted == false)
                .ToListAsync();

            if (doctors.Count == 0)
            {
                throw new NonExistentEntity("There are no doctors in the database, try to add some first!");
            }
            return doctors;
        }
        public async Task<List<Patient>> GetAllPatientsData()
        {
            var patients = await context.Patients
                .Where(x => x.IsDeleted == false)
                .ToListAsync();

            if (patients.Count == 0)
            {
                throw new NonExistentEntity("There are no patients in the database, try to add some first!");
            }
            return patients;
        }
        public async Task<List<Medicine>> GetAllMedicinesData()
        {
            var medicines = await context.Medicines
                .Where(x => x.IsDeleted == false)
                .Include(x => x.Category)
                .ToListAsync();

            if (medicines.Count == 0)
            {
                throw new NonExistentEntity("There are no medicines in the database, try to add some first!");
            }
            return medicines;
        }
        public async Task<List<ManufacturerMedicine>> GetAllManufacturerMedicinesData()
        {
            var mms = await context.ManufacturerMedicines
                .Where(x => x.IsDeleted == false)
                .Include(x => x.Medicine)
                .Include(x => x.Manufacturer)
                .ToListAsync();

            if (mms.Count == 0)
            {
                throw new NonExistentEntity("There are no manufacturer medicines in the database, try to add some first!");
            }
            return mms;
        }
        public async Task<List<PharmacyMedicine>> GetAllPharmacyMedicinesData()
        {
            var pm = await context.PharmacyMedicines
                .Where(x => x.IsDeleted == false)
                .Include(x => x.ManufacturerMedicine)
                 .ThenInclude(x => x.Medicine)
                .Include(x => x.ManufacturerMedicine)
                 .ThenInclude(x => x.Manufacturer)
                .ToListAsync();

            if (pm.Count == 0)
            {
                throw new NonExistentEntity("There are no pharmacy medicines in the database, try to add some first!");
            }
            return pm;
        }
        public async Task<List<Prescription>> GetAllPrescriptionsData()
        {
            var prescriptions = await context.Prescriptions
                .Where(x => x.IsDeleted == false)
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.PrescriptionMedicines)
                .ToListAsync();

            if (prescriptions.Count == 0)
            {
                throw new NonExistentEntity("There are no prescriptions in the database, try to add some first!");
            }
            return prescriptions;
        }
        public async Task<List<(Order order, List<(OrderMedicine om, decimal UnitPrice, decimal TotalPrice)>)>> GetAllOrdersData()
        {
            var orders = await context.Orders
                .Where(o => !o.IsDeleted)
                .Include(o => o.Manufacturer)
                .Include(o => o.OrderMedicines)
                    .ThenInclude(om => om.Medicine)
                .ToListAsync();

            if (orders.Count == 0)
            {
                throw new NonExistentEntity("There are no orders in the database, try to add some first!");
            }

            var manufacturerMedicines = await context.ManufacturerMedicines.Where(x => x.IsDeleted == false).ToListAsync();

            var result = new List<(Order, List<(OrderMedicine, decimal, decimal)>)>();

            foreach (var order in orders)
            {
                var orderMedicinesWithPrices = new List<(OrderMedicine, decimal, decimal)>();

                foreach (var om in order.OrderMedicines)
                {
                    var mm = manufacturerMedicines
                        .FirstOrDefault(med => med.ManufacturerId == order.ManufacturerId && med.MedicineId == om.MedicineId);

                    decimal unitPrice = mm?.ManufacturerPrice ?? 0m;
                    decimal totalPrice = unitPrice * om.BoughtQuantity;

                    orderMedicinesWithPrices.Add((om, unitPrice, totalPrice));
                }

                result.Add((order, orderMedicinesWithPrices));
            }

            return result;
        }
        public async Task<List<Sale>> GetAllSalesData()
        {
            var sales = await context.Sales
                 .Where(x => x.IsDeleted == false)
                 .Include(x => x.Prescription)
                  .ThenInclude(x => x.PrescriptionMedicines)
                 .Include(x => x.Prescription)
                  .ThenInclude(x => x.Patient)
                 .Include(x => x.Prescription)
                  .ThenInclude(x => x.Doctor)
                 .ToListAsync();

            if (sales.Count == 0)
            {
                throw new NonExistentEntity("There are no sales in the database, try to add some first!");
            }
            return sales;
        }
        #endregion

        #region Filtration
        public async Task<List<string>> GetPatientsByMedicineName(int medicineId)
        {
            return await context.PrescriptionMedicines
                .Where(x => x.Medicine.Id == medicineId && x.Prescription.IsDeleted == false)
                .Select(x => x.Prescription.Patient.PatientName)
                .Distinct()
                .ToListAsync() ?? throw new NonExistentEntity("No patient with that medicine have been found!");
        }
        public async Task<decimal> GetTotalSalesByYear(int year)
        {
            var sales = await context.Sales
                .Where(x => x.SaleDate.Year == year && x.IsDeleted == false)
                .Include(x => x.Prescription)
                    .ThenInclude(x => x.PrescriptionMedicines)
                .ToListAsync() ?? throw new NonExistentEntity("No sales for this year!");

            decimal total = 0;

            foreach (var sale in sales)
            {
                foreach (var pm in sale.Prescription.PrescriptionMedicines)
                {
                    var price = await context.ManufacturerMedicines
                        .Where(x => x.MedicineId == pm.MedicineId)
                        .Select(x => x.ManufacturerPrice)
                        .FirstOrDefaultAsync();

                    total += pm.PrescribedQuantity * price;
                }
            }

            return total;
        }
        public async Task<string> GetLastPrescriptionByPatient(int id)
        {
            var patient = await context.Patients
                .Where(x => x.IsDeleted == false)
                .Include(x => x.Prescriptions)
                 .ThenInclude(x => x.PrescriptionMedicines)
                  .ThenInclude(x => x.Medicine)
                .Include(x => x.Prescriptions)
                 .ThenInclude(x => x.Doctor)
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NonExistentEntity("Patient by the given name cannot be found, try again!");

            var lastPrescription = patient.Prescriptions
                .Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.PrescribedAt)
                .FirstOrDefault() ?? throw new NonExistentEntity("Patient doesn't have any prescriptions yet, try again later!");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Patient: {patient.PatientName} had prescription prescribed at: {lastPrescription.PrescribedAt} by Doctor: {lastPrescription.Doctor}, with medicines: ");
            foreach (var medicine in lastPrescription.PrescriptionMedicines)
            {
                sb.AppendLine($"Medicine Name: {medicine.Medicine.MedicineName}, Dosage: {medicine.Dosage}, Prescribed Quantity of the medicine: {medicine.PrescribedQuantity}");
            }

            return sb.ToString().Trim();
        }
        public async Task<string> GetAllOrdersByManufacturer(int manId)
        {
            var orders = await context.Orders
                .Where(x => x.ManufacturerId == manId && x.IsDeleted == false)
                .Include(x => x.Manufacturer)
                .Include(x => x.OrderMedicines)
                 .ThenInclude(x => x.Medicine)
                .ToListAsync() ?? throw new NonExistentEntity("No orders have been made to the manufacturer, try again later!");


            StringBuilder sb = new StringBuilder();

            foreach (var order in orders)
            {
                sb.AppendLine($"Order Date: {order.OrderDate}");
                foreach (var om in order.OrderMedicines)
                {
                    sb.AppendLine($"-Medicine: {om.Medicine.MedicineName}, Quantity: {om.BoughtQuantity}");
                }
            }

            return sb.ToString().Trim();
        }
        public async Task<string> GetPrescriptionsByMedicineName(int medicineId)
        {
            var prescriptions = await context.Prescriptions
                .Where(x => x.PrescriptionMedicines.Any(x => x.MedicineId == medicineId) && x.IsDeleted == false)
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .ToListAsync() ?? throw new NonExistentEntity("No prescriptions have the desired medicine, try again later!");

            StringBuilder sb = new StringBuilder();

            foreach (var prescription in prescriptions)
            {
                sb.AppendLine($"Prescription ID: {prescription.Id}, Date: {prescription.PrescribedAt}, Doctor: {prescription.Doctor.DoctorName}, Patient: {prescription.Patient.PatientName}");
            }

            return sb.ToString().Trim();
        }
        public async Task<string> GetUnorderedMedicines()
        {
            var medicines = await context.Medicines
                .Where(x => !x.OrderMedicines.Any() && x.IsDeleted == false)
                .ToListAsync() ?? throw new ArgumentException("Every medicine has been ordered or there aren't any medicines availabe yet!");

            StringBuilder sb = new StringBuilder();

            foreach (var medicine in medicines)
            {
                sb.AppendLine($"-{medicine.MedicineName}");
            }

            return sb.ToString().Trim();
        }
        public async Task<string> GetLowOnStockMedicinesInPharmacy()
        {
            var pms = await context.PharmacyMedicines
                .Where(x => x.StockQuantity < 10 && x.IsDeleted == false)
                .Include(x => x.ManufacturerMedicine)
                  .ThenInclude(x => x.Medicine)
                .ToListAsync() ?? throw new ArgumentException("No medicines with stock quantity under 10!");

            StringBuilder sb = new StringBuilder();

            foreach (var medicine in pms)
            {
                sb.AppendLine($"-{medicine.ManufacturerMedicine.Medicine.MedicineName}, Quantity: {medicine.StockQuantity}");
            }

            return sb.ToString().Trim();
        }
        public async Task<string> GetOldPatients()
        {
            var patients = await context.Patients
               .Where(x => x.DateOfBirth < DateTime.Now.AddYears(-65) && x.IsDeleted == false)
               .ToListAsync() ?? throw new ArgumentException("No patients older than 60!");

            StringBuilder sb = new StringBuilder();

            foreach (var patient in patients)
            {
                sb.AppendLine($"-{patient.PatientName}, Date of Birth: {patient.DateOfBirth}");
            }

            return sb.ToString().Trim();
        }
        public async Task<string> GetOrdersInTheLast30Days()
        {
            var orders = await context.Orders
              .Where(x => x.OrderDate >= DateTime.Now.AddDays(-30) && x.IsDeleted == false)
              .Include(x => x.Manufacturer)
               .ThenInclude(x => x.ManufacturerMedicines)
              .Include(x => x.OrderMedicines)
               .ThenInclude(x => x.Medicine)
              .ToListAsync() ?? throw new ArgumentException("No orders in the last 30 days!");

            StringBuilder sb = new StringBuilder();

            foreach (var order in orders)
            {
                sb.AppendLine($"-{order.Id}, Order Date: {order.OrderDate}, Made to: {order.Manufacturer.ManufacturerName}, Medicines: ");
                foreach (var medicine in order.OrderMedicines)
                {
                    sb.AppendLine($"-Medicine Name: {medicine.Medicine.MedicineName}, Bought Quantity: {medicine.BoughtQuantity}, Recommended Dosage: {medicine.Medicine.RecommendedDosage}");
                }
            }

            return sb.ToString().Trim();
        }
        public async Task<string> GetAllManufacturersWithEmailEndingInBg()
        {
            var manufacturers = await context.Manufacturers
                .Where(x => x.Email.EndsWith(".bg") && x.IsDeleted == false)
                .ToListAsync() ?? throw new ArgumentException("No manufacturers with email ending in .bg!");

            StringBuilder sb = new StringBuilder();

            foreach (var manufacturer in manufacturers)
            {
                sb.AppendLine($"-{manufacturer.ManufacturerName}, Email: {manufacturer.Email}");
            }

            return sb.ToString().Trim();
        }
        #endregion
    }
}
