using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Data.Models
{
	public class Category
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string CategoryName { get; set; }
		[Required]
		public string CategoryDescription { get; set; }
		[Required]
		public bool IsDeleted { get; set; }

		public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
	}
}
