using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Data.Data.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public int ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        [Required]
        public bool IsDeleted { get; set; }

        public virtual ICollection<OrderMedicine> OrderMedicines { get; set; } = new HashSet<OrderMedicine>();

        public Order()
        {
            this.OrderDate = DateTime.Now;
        }
    }
}
