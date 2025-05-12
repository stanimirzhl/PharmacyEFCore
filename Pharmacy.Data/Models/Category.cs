using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
