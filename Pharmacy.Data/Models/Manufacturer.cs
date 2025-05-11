using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public virtual ICollection<ManufacturerMedicine> ManufacturerMedicines { get; set; } = new HashSet<ManufacturerMedicine>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

    }
}
