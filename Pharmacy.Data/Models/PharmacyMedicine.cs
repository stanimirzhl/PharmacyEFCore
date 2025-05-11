using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
