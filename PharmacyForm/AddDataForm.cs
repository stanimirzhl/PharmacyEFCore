using Pharmacy.Core;
using Pharmacy.Data.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace PharmacyForm
{
	public partial class AddDataForm : Form
	{
		private ComboBox cmbEntity;
		private Panel pnlFields;
		private Button btnSubmit;
		private readonly PharmacyController controller;
		private readonly Dictionary<string, Control> inputControls = new();
		private Dictionary<string, List<(string Display, object Value)>> lookupData = new();

		public AddDataForm(PharmacyController controller)
		{
			InitializeComponent();
			InitializeControls();

			Load += AddDataForm_Load;

			this.controller = controller;
		}

		private async void BtnSubmit_Click(object sender, EventArgs e)
		{
			string selected = cmbEntity.SelectedItem.ToString();

			try
			{
				await Add(selected);

				MessageBox.Show($"{selected} added successfully!");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}");
			}
		}

		private async Task Add(string name)
		{
			switch (name)
			{
				case "Category":
					await controller.AddCategory(GetText("CategoryName"), GetText("CategoryDescription"));
					break;

				case "Manufacturer":
					await controller.AddManufacturer(GetText("ManufacturerName"), GetText("Email"), GetText("Website"), GetText("Phone"));
					break;

				case "Doctor":
					await controller.AddDoctor(GetText("DoctorName"), GetText("Email"), GetText("Phone"), GetText("Specialty"));
					break;

				case "Patient":
					DateTime dob = ((DateTimePicker)pnlFields.Controls.Find("input_DateOfBirth", true)[0]).Value;
					await controller.AddPatient(GetText("PatientName"), GetText("Email"), GetText("Phone"), dob);
					break;

				case "Medicine":
					await controller.AddMedicine(
						GetText("MedicineName"),
						GetText("Description"),
						int.Parse(GetText("CategoryId")),
						GetText("RecommendedDosage"));
					break;

				case "ManufacturerMedicine":
					await controller.AddManufacturerMedicine(
						int.Parse(GetText("MedicineId")),
						int.Parse(GetText("ManufacturerId")),
						decimal.Parse(GetText("ManufacturerPrice")),
						int.Parse(GetText("MadeQuantity")));
					break;

				case "PharmacyMedicine":
					await controller.AddPharmacyMedicine(
						int.Parse(GetText("ManufacturerId")),
						int.Parse(GetText("MedicineId")),
						int.Parse(GetText("StockQuantity")),
						decimal.Parse(GetText("PharmacyPrice")));
					break;

				case "Prescription":
					await controller.AddPrescription(
						int.Parse(GetText("PatientId")),
						int.Parse(GetText("DoctorId")),
						((DateTimePicker)pnlFields.Controls.Find("input_PrescribedAt", true)[0]).Value);
					break;

				case "PrescriptionMedicine":
					await controller.AddPrescriptionMedicine(
					   GetText("PrescriptionId"),
					   int.Parse(GetText("MedicineId")),
					   GetText("Dosage"),
					   int.Parse(GetText("PrescribedQuantity")));
					break;

				case "Order":
					await controller.AddOrder(int.Parse(GetText("ManufacturerId")));
					break;

				case "OrderMedicine":
					await controller.AddOrderMedicine(
						GetText("OrderId"),
						int.Parse(GetText("MedicineId")),
						int.Parse(GetText("BoughtQuantity")));
					break;

				case "Sale":
					await controller.AddSale(GetText("PrescriptionId"));
					break;

				default:
					MessageBox.Show("No such entity, try again with a valid one!");
					break;
			}
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
				var lbl = new Label
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
				MessageBox.Show($"Error loading lookups: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
		private string GetText(string fieldName)
		{
			var ctrl = pnlFields.Controls.Find($"input_{fieldName}", true).FirstOrDefault();
			if (ctrl is TextBox tb)
				return tb.Text;
			throw new Exception($"TextBox for {fieldName} not found.");
		}
	}
}