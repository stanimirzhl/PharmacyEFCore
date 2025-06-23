using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Data.Models
{
	public class Sale
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string PrescriptionId { get; set; }
		public virtual Prescription Prescription { get; set; }
		[Required]
		public bool IsDeleted { get; set; }

		public DateTime SaleDate { get; set; }

		public Sale()
		{
			this.SaleDate = DateTime.Now;
		}
	}
}
