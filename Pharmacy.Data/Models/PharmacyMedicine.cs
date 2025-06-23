using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Data.Models
{
	public class PharmacyMedicine
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public int ManufacturerMedicineId { get; set; }
		public virtual ManufacturerMedicine ManufacturerMedicine { get; set; }
		[Required]
		public int StockQuantity { get; set; }
		[Required]
		public decimal PharmacyPrice { get; set; }
		[Required]
		public bool IsDeleted { get; set; }
	}
}
