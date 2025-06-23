using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Data.Models
{
	public class PrescriptionMedicine
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string PrescriptionId { get; set; }
		public virtual Prescription Prescription { get; set; }
		[Required]
		public int MedicineId { get; set; }
		public virtual Medicine Medicine { get; set; }
		[Required]
		public string Dosage { get; set; }
		[Required]
		public int PrescribedQuantity { get; set; }
		[Required]
		public bool IsDeleted { get; set; }
	}
}
