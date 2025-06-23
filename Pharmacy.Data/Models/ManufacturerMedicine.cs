using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Data.Models
{
	public class ManufacturerMedicine
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public int ManufacturerId { get; set; }
		public virtual Manufacturer Manufacturer { get; set; }
		[Required]
		public int MedicineId { get; set; }
		public virtual Medicine Medicine { get; set; }
		[Required]
		public decimal ManufacturerPrice { get; set; }
		[Required]
		public int MadeQuantity { get; set; }
		[Required]
		public bool IsDeleted { get; set; }
	}
}
