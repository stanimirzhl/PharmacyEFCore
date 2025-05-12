using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Data.Data.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string PatientName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public bool IsDeleted { get; set; }

        public virtual ICollection<Prescription> Prescriptions { get; set; } = new HashSet<Prescription>();

        public Patient()
        {
            this.DateOfBirth = DateTime.Now;
        }
    }
}
