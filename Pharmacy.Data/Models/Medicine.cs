using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Data.Models
{
	public class Medicine
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string MedicineName { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public int CategoryId { get; set; }
		public virtual Category Category { get; set; }
		[Required]
		public string RecommendedDosage { get; set; }
		[Required]
		public bool IsDeleted { get; set; }

		public virtual ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = new HashSet<PrescriptionMedicine>();
		public virtual ICollection<ManufacturerMedicine> ManufacturerMedicines { get; set; } = new HashSet<ManufacturerMedicine>();
		public virtual ICollection<PharmacyMedicine> PharmacyMedicines { get; set; } = new HashSet<PharmacyMedicine>();
		public virtual ICollection<OrderMedicine> OrderMedicines { get; set; } = new HashSet<OrderMedicine>();
	}
}
