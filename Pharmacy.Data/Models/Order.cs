using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Data.Models
{
	public class Order
	{
		[Key]
		public string Id { get; set; }
		[Required]
		public DateTime OrderDate { get; set; }
		[Required]
		public int ManufacturerId { get; set; }
		public virtual Manufacturer Manufacturer { get; set; }
		[Required]
		public bool IsDeleted { get; set; }

		public DateTime? DeletedAt { get; set; }

		public virtual ICollection<OrderMedicine> OrderMedicines { get; set; } = new HashSet<OrderMedicine>();

		public Order()
		{
			this.OrderDate = DateTime.Now;
		}
	}
}
