﻿using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data.Data.Models
{
	public class Doctor
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string DoctorName { get; set; }
		[Required]
		public string Specialty { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Phone { get; set; }
		[Required]
		public bool IsDeleted { get; set; }

		public virtual ICollection<Prescription> Prescriptions { get; set; } = new HashSet<Prescription>();

	}
}
