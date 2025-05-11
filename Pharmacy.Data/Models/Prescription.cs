using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public virtual ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; } = new HashSet<PrescriptionMedicine>();

        public Prescription()
        {
            this.PrescribedAt = DateTime.Now;
        }
    }
}
