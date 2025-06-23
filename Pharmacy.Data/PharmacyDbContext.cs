using Microsoft.EntityFrameworkCore;
using Pharmacy.Data.Data.Models;

namespace Pharmacy.Data.Data
{
	public class PharmacyDbContext : DbContext
	{
		public PharmacyDbContext()
		{
		}
		public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			=> optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectModels;Database=PharmacyDb;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");

		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<Manufacturer> Manufacturers { get; set; }
		public virtual DbSet<Doctor> Doctors { get; set; }
		public virtual DbSet<Patient> Patients { get; set; }
		public virtual DbSet<Medicine> Medicines { get; set; }
		public virtual DbSet<ManufacturerMedicine> ManufacturerMedicines { get; set; }
		public virtual DbSet<PharmacyMedicine> PharmacyMedicines { get; set; }
		public virtual DbSet<Prescription> Prescriptions { get; set; }
		public virtual DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }
		public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<OrderMedicine> OrderMedicines { get; set; }
		public virtual DbSet<Sale> Sales { get; set; }


	}
}
