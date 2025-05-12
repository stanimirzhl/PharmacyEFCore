using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Data.Data.Models
{
    public class OrderMedicine
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string OrderId { get; set; }
        public virtual Order Order { get; set; }
        [Required]
        public int MedicineId { get; set; }
        public virtual Medicine Medicine { get; set; }
        [Required]
        public int BoughtQuantity { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}
