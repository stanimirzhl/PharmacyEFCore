using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Data.Models
{
	public class Prescription
	{
		[Key]
		public string Id { get; set; }
		[Required]
		public int PatientId { get; set; }
		public virtual Patient Patient { get; set; }
		[Required]
		public int DoctorId { get; set; }
		public virtual Doctor Doctor { get; set; }
		[Required]
		public DateTime PrescribedAt { get; set; }
		[Required]
		public bool HasUsed { get; set; }
		[Required]
		public bool IsDeleted { get; set; }

		public DateTime? DeletedAt { get; set; }

		public virtual ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = new HashSet<PrescriptionMedicine>();

		public Prescription()
		{
			this.PrescribedAt = DateTime.Now;
		}
	}
}
