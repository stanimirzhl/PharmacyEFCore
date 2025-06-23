using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Data.Models
{
	public class Manufacturer
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string ManufacturerName { get; set; }
		[Required]
		public string Website { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Phone { get; set; }
		[Required]
		public bool IsDeleted { get; set; }

		public virtual ICollection<ManufacturerMedicine> ManufacturerMedicines { get; set; } = new HashSet<ManufacturerMedicine>();
		public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

	}
}
