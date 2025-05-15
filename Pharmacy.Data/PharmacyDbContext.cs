using Microsoft.EntityFrameworkCore;
using Pharmacy.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=PharmacyDb;Integrated Security=True;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

        public virtual DbSet<Category> Categories {get; set;}
        public virtual DbSet<Manufacturer> Manufacturers {get; set;}
        public virtual DbSet<Doctor> Doctors {get; set;}
        public virtual DbSet<Patient> Patients {get; set;}
        public virtual DbSet<Medicine> Medicines {get; set;}
        public virtual DbSet<ManufacturerMedicine> ManufacturerMedicines {get; set;}
        public virtual DbSet<PharmacyMedicine> PharmacyMedicines {get; set;}
        public virtual DbSet<Prescription> Prescriptions {get; set;}
        public virtual DbSet<PrescriptionMedicine> PrescriptionMedicines {get; set;}
        public virtual DbSet<Order> Orders {get; set;}
        public virtual DbSet<OrderMedicine> OrderMedicines {get; set;}
        public virtual DbSet<Sale> Sales {get; set;}


    }
}
