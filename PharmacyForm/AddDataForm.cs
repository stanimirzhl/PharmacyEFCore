using Pharmacy.Core;
using Pharmacy.Data.Data.Models;
using System.Data;
using System.Text.RegularExpressions;

namespace PharmacyForm
{
	public partial class AddDataForm : Form
	{
		private ComboBox cmbEntity;
		private Panel pnlFields;
		private Button btnSubmit;
		private readonly PharmacyController controller;
		private Dictionary<string, List<(string Display, object Value)>> lookupData = new();

		public AddDataForm(PharmacyController controller)
		{
			this.controller = controller;
			InitializeComponent();
			InitializeControls();

			Load += AddDataForm_Load;

		}

		private async void BtnSubmit_Click(object sender, EventArgs e)
		{

			if (cmbEntity.SelectedItem is null)
			{
				MessageBox.Show("Please select table to add.");
				return;
			}

			string selected = cmbEntity.SelectedItem.ToString();

			try
			{
				bool result = await Add(selected);

				if (result)
				{
					MessageBox.Show($"{selected} added successfully!");
					this.ClearInputs();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}");
			}
		}

		private void ClearInputs()
		{
			foreach (Control control in pnlFields.Controls)
			{
				switch (control)
				{
					case TextBox tb:
						tb.Text = string.Empty;
						break;
					case ComboBox cb:
						cb.SelectedIndex = -1;
						cb.Text = string.Empty;
						break;
					case DateTimePicker dtp:
						dtp.Value = DateTime.Now;
						break;
				}
			}
		}

		private async Task<bool> Add(string name)
		{
			switch (name)
			{
				case "Category":
					if (string.IsNullOrEmpty(GetText("CategoryName")) || string.IsNullOrEmpty(GetText("CategoryDescription")))
					{
						MessageBox.Show("Please fill in all fields for Category.");
						return false;
					}
					await controller.AddCategory(GetText("CategoryName"), GetText("CategoryDescription"));
					break;

				case "Manufacturer":
					if (string.IsNullOrEmpty(GetText("ManufacturerName")) || string.IsNullOrEmpty(GetText("Email")) ||
						string.IsNullOrEmpty(GetText("Website")) || string.IsNullOrEmpty(GetText("Phone")))
					{
						MessageBox.Show("Please fill in all fields for Manufacturer.");
						return false;
					}
					if (!GetText("Email").Contains('@'))
					{
						MessageBox.Show("Invalid email format. Please enter a valid email address with @.");
						return false;
					}
					await controller.AddManufacturer(GetText("ManufacturerName"), GetText("Email"), GetText("Website"), GetText("Phone"));
					break;

				case "Doctor":
					if (string.IsNullOrEmpty(GetText("DoctorName")) || string.IsNullOrEmpty(GetText("Email")) ||
						string.IsNullOrEmpty(GetText("Phone")) || string.IsNullOrEmpty(GetText("Specialty")))
					{
						MessageBox.Show("Please fill in all fields for Doctor.");
						return false;
					}
					if (!GetText("Email").Contains('@'))
					{
						MessageBox.Show("Invalid email format. Please enter a valid email address with @.");
						return false;
					}
					await controller.AddDoctor(GetText("DoctorName"), GetText("Email"), GetText("Phone"), GetText("Specialty"));
					break;

				case "Patient":
					if (string.IsNullOrEmpty(GetText("PatientName")) || string.IsNullOrEmpty(GetText("Email")) ||
						string.IsNullOrEmpty(GetText("Phone")))
					{
						MessageBox.Show("Please fill in all fields for Patient.");
						return false;
					}
					if (!GetText("Email").Contains('@'))
					{
						MessageBox.Show("Invalid email format. Please enter a valid email address with @.");
						return false;
					}
					DateTime dob = ((DateTimePicker)pnlFields.Controls.Find("input_DateOfBirth", true)[0]).Value;
					await controller.AddPatient(GetText("PatientName"), GetText("Email"), GetText("Phone"), dob);
					break;

				case "Medicine":
					if (string.IsNullOrEmpty(GetText("MedicineName")) || string.IsNullOrEmpty(GetText("Description")) ||
						string.IsNullOrEmpty(GetText("RecommendedDosage")) || string.IsNullOrEmpty(GetText("CategoryId")))
					{
						MessageBox.Show("Please fill in all fields for Medicine.");
						return false;
					}
					await controller.AddMedicine(
						GetText("MedicineName"),
						GetText("Description"),
						int.Parse(GetText("CategoryId")),
						GetText("RecommendedDosage"));
					break;

				case "ManufacturerMedicine":
					if (string.IsNullOrEmpty(GetText("MedicineId")) || string.IsNullOrEmpty(GetText("ManufacturerId")) ||
						string.IsNullOrEmpty(GetText("ManufacturerPrice")) || string.IsNullOrEmpty(GetText("MadeQuantity")))
					{
						MessageBox.Show("Please fill in all fields for Manufacturer Medicine.");
						return false;
					}
					if (!decimal.TryParse(GetText("ManufacturerPrice"), out decimal manufacturerPrice) ||
						!int.TryParse(GetText("MadeQuantity"), out int madeQuantity))
					{
						MessageBox.Show("Invalid price or quantity format.");
						return false;
					}
					await controller.AddManufacturerMedicine(
						int.Parse(GetText("MedicineId")),
						int.Parse(GetText("ManufacturerId")),
						decimal.Parse(GetText("ManufacturerPrice")),
						int.Parse(GetText("MadeQuantity")));
					break;

				case "PharmacyMedicine":
					if (string.IsNullOrEmpty(GetText("ManufacturerId")) || string.IsNullOrEmpty(GetText("MedicineId")) ||
						string.IsNullOrEmpty(GetText("StockQuantity")) || string.IsNullOrEmpty(GetText("PharmacyPrice")))
					{
						MessageBox.Show("Please fill in all fields for Pharmacy Medicine.");
						return false;
					}
					if (!decimal.TryParse(GetText("PharmacyPrice"), out decimal pharmacyPrice) ||
						!int.TryParse(GetText("StockQuantity"), out int stockQuantity))
					{
						MessageBox.Show("Invalid price or quantity format.");
						return false;
					}
					await controller.AddPharmacyMedicine(
						int.Parse(GetText("ManufacturerId")),
						int.Parse(GetText("MedicineId")),
						int.Parse(GetText("StockQuantity")),
						decimal.Parse(GetText("PharmacyPrice")));
					break;

				case "Prescription":
					if (string.IsNullOrEmpty(GetText("PatientId")) || string.IsNullOrEmpty(GetText("DoctorId")))
					{
						MessageBox.Show("Please fill in all fields for Prescription.");
						return false;
					}
					await controller.AddPrescription(
						int.Parse(GetText("PatientId")),
						int.Parse(GetText("DoctorId")),
						((DateTimePicker)pnlFields.Controls.Find("input_PrescribedAt", true)[0]).Value);
					break;

				case "PrescriptionMedicine":
					if (string.IsNullOrEmpty(GetText("PrescriptionId")) || string.IsNullOrEmpty(GetText("MedicineId")) ||
						string.IsNullOrEmpty(GetText("Dosage")) || string.IsNullOrEmpty(GetText("PrescribedQuantity")))
					{
						MessageBox.Show("Please fill in all fields for Prescription Medicine.");
						return false;
					}
					if (!int.TryParse(GetText("PrescribedQuantity"), out int prescribedQuantity))
					{
						MessageBox.Show("Invalid quantity format.");
						return false;
					}
					if (!Regex.IsMatch(GetText("Dosage"), @"^\d+mg$"))
					{
						MessageBox.Show("Dosage must be in the format 'Xmg' where X is a number (e.g., 500mg).");
						return false;
					}
					await controller.AddPrescriptionMedicine(
				   GetText("PrescriptionId"),
				   int.Parse(GetText("MedicineId")),
				   GetText("Dosage"),
				   int.Parse(GetText("PrescribedQuantity")));
					break;

				case "Order":
					if (string.IsNullOrEmpty(GetText("ManufacturerId")))
					{
						MessageBox.Show("Please fill in all fields for Order.");
						return false;
					}
					await controller.AddOrder(int.Parse(GetText("ManufacturerId")));
					break;

				case "OrderMedicine":
					if (string.IsNullOrEmpty(GetText("OrderId")) || string.IsNullOrEmpty(GetText("MedicineId")) ||
						string.IsNullOrEmpty(GetText("BoughtQuantity")))
					{
						MessageBox.Show("Please fill in all fields for Order Medicine.");
						return false;
					}
					if (!int.TryParse(GetText("BoughtQuantity"), out int boughtQuantity))
					{
						MessageBox.Show("Invalid quantity format.");
						return false;
					}
					await controller.AddOrderMedicine(
						GetText("OrderId"),
						int.Parse(GetText("MedicineId")),
						int.Parse(GetText("BoughtQuantity")));
					break;

				case "Sale":
					if (string.IsNullOrEmpty(GetText("PrescriptionId")))
					{
						MessageBox.Show("Please fill in all fields for Sale.");
						return false;
					}
					await controller.AddSale(GetText("PrescriptionId"));
					break;

				default:
					MessageBox.Show("No such entity, try again with a valid one!");
					break;
			}
			return true;
		}

		private void InitializeControls()
		{
			cmbEntity = new ComboBox
			{
				Location = new Point(170, 20),
				Width = 300,
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 12),
			};

			cmbEntity.Items.AddRange(new string[]
			{
				nameof(Sale),
				nameof(Prescription),
				nameof(PrescriptionMedicine),
				nameof(PharmacyMedicine),
				nameof(Patient),
				nameof(OrderMedicine),
				nameof(Order),
				nameof(Medicine),
				nameof(ManufacturerMedicine),
				nameof(Manufacturer),
				nameof(Category),
				nameof(Doctor)
			});

			cmbEntity.SelectedIndexChanged += CmbEntity_SelectedIndexChanged;

			pnlFields = new Panel
			{
				Location = new Point(20, 70),
				Size = new Size(740, 400),
				AutoScroll = true,
				BackColor = Color.MidnightBlue
			};

			btnSubmit = new Button
			{
				Text = "Submit",
				Location = new Point(500, 10),
				Size = new Size(120, 55),
				Font = new Font("Segoe UI", 13.8F, FontStyle.Bold),
				ForeColor = Color.White
			};
			btnSubmit.Click += BtnSubmit_Click;

			this.Controls.Add(cmbEntity);
			this.Controls.Add(pnlFields);
			this.Controls.Add(btnSubmit);
		}

		private void CmbEntity_SelectedIndexChanged(object sender, EventArgs e)
		{
			pnlFields.Controls.Clear();
			string selected = cmbEntity.SelectedItem.ToString();

			if (!_inputMap.ContainsKey(selected))
				return;

			var fields = _inputMap[selected];
			int top = 10;

			foreach (var (label, type) in fields)
			{
				Label lbl = new Label
				{
					Text = label,
					ForeColor = Color.White,
					Font = new Font("Segoe UI", 10, FontStyle.Bold),
					Location = new Point(10, top + 5),
					Size = new Size(200, 25)
				};

				Control input;

				if (type == typeof(DateTime))
				{
					input = new DateTimePicker
					{
						Name = $"input_{label}",
						Location = new Point(220, top),
						Width = 300
					};
				}
				else if (lookupData.ContainsKey(label))
				{
					ComboBox cmb = new ComboBox
					{
						Name = $"input_{label}",
						Location = new Point(220, top),
						Width = 300,
						DropDownStyle = ComboBoxStyle.DropDownList
					};
					cmb.Items.AddRange(lookupData[label].Select(item => item.Display).ToArray());
					input = cmb;
				}
				else
				{
					input = new TextBox
					{
						Name = $"input_{label}",
						Location = new Point(220, top),
						Width = 300
					};
				}


				pnlFields.Controls.Add(lbl);
				pnlFields.Controls.Add(input);
				top += 40;
			}
		}


		private async void AddDataForm_Load(object sender, EventArgs e)
		{
			try
			{
				await LoadData();
		}
			catch (Exception ex)
			{
				//MessageBox.Show($"Error loading lookups: {ex.Message}");
			}
}

		private async Task LoadData()
		{
			lookupData["CategoryId"] = (await controller.GetAllCategoriesData()).Select(c => (c.CategoryName, (object)c.Id)).ToList();
			lookupData["ManufacturerId"] = (await controller.GetAllManufacturersData()).Select(m => (m.ManufacturerName, (object)m.Id)).ToList();
			lookupData["DoctorId"] = (await controller.GetAllDoctorsData()).Select(d => (d.DoctorName, (object)d.Id)).ToList();
			lookupData["PatientId"] = (await controller.GetAllPatientsData()).Select(p => (p.PatientName, (object)p.Id)).ToList();
			lookupData["MedicineId"] = (await controller.GetAllMedicinesData()).Select(m => (m.MedicineName, (object)m.Id)).ToList();
			lookupData["PrescriptionId"] = (await controller.GetAllPrescriptionsData()).Select(p => (p.Id, (object)p.Id)).ToList();
			lookupData["OrderId"] = (await controller.GetAllOrdersData()).Select(o => (o.order.Id, (object)o.order.Id)).ToList();
		}

		private readonly Dictionary<string, List<(string label, Type type)>> _inputMap = new()
		{
			["Category"] = new()
			{
				("CategoryName", typeof(string)),
				("CategoryDescription", typeof(string))
			},

			["Manufacturer"] = new()
			{
				("ManufacturerName", typeof(string)),
				("Email", typeof(string)),
				("Website", typeof(string)),
				("Phone", typeof(string))
			},

			["Doctor"] = new()
			{
				("DoctorName", typeof(string)),
				("Email", typeof(string)),
				("Phone", typeof(string)),
				("Specialty", typeof(string))
			},

			["Patient"] = new()
			{
				("PatientName", typeof(string)),
				("Email", typeof(string)),
				("Phone", typeof(string)),
				("DateOfBirth", typeof(DateTime))
			},

			["Medicine"] = new()
			{
				("MedicineName", typeof(string)),
				("Description", typeof(string)),
				("CategoryId", typeof(int)),
				("RecommendedDosage", typeof(string))
			},

			["ManufacturerMedicine"] = new()
			{
				("MedicineId", typeof(int)),
				("ManufacturerId", typeof(int)),
				("ManufacturerPrice", typeof(decimal)),
				("MadeQuantity", typeof(int))
			},

			["PharmacyMedicine"] = new()
			{
				("ManufacturerId", typeof(int)),
				("MedicineId", typeof(int)),
				("StockQuantity", typeof(int)),
				("PharmacyPrice", typeof(decimal))
			},

			["Prescription"] = new()
			{
				("PatientId", typeof(int)),
				("DoctorId", typeof(int)),
				("PrescribedAt", typeof(DateTime))
			},

			["PrescriptionMedicine"] = new()
			{
				("PrescriptionId", typeof(string)),
				("MedicineId", typeof(int)),
				("Dosage", typeof(string)),
				("PrescribedQuantity", typeof(int))
			},

			["Order"] = new()
			{
				("ManufacturerId", typeof(int))
			},

			["OrderMedicine"] = new()
			{
				("OrderId", typeof(string)),
				("MedicineId", typeof(int)),
				("BoughtQuantity", typeof(int))
			},

			["Sale"] = new()
			{
				("PrescriptionId", typeof(string))
			}
		};
		private string GetText(string label)
		{
			var control = pnlFields.Controls.Find($"input_{label}", true).FirstOrDefault();
			return control switch
			{
				TextBox tb => tb.Text,
				ComboBox cb => cb.SelectedItem != null
					? lookupData[label].FirstOrDefault(x => x.Display == cb.SelectedItem.ToString()).Value?.ToString()
					: null,
				_ => string.Empty
			};
		}
	}
}