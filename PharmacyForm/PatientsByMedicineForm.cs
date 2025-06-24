using Pharmacy.Core;
using Pharmacy.Data.Data.Models;

namespace PharmacyForm
{
	public partial class PatientsByMedicineForm : Form
	{
		private Dictionary<string, List<(string Display, object Value)>> lookupData = new();
		private readonly PharmacyController controller;
		private ComboBox medicines;
		private Button check;
		private TextBox txtPatients;
		private Panel panel;

		public PatientsByMedicineForm(PharmacyController controller)
		{
			InitializeComponent();

			this.controller = controller;

			this.Load += PatientsByMedicineForm_Load;
		}

		private async void PatientsByMedicineForm_Load(object sender, EventArgs e)
		{
			try
			{
				lookupData["MedicineId"] = (await controller.GetAllMedicinesData()).Select(m => (m.MedicineName, (object)m.Id)).ToList();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading medicines: {ex.Message}");
				return;
			}

			medicines = new ComboBox
			{
				Location = new Point(170, 20),
				Width = 300,
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 12)
			};

			medicines.Items.AddRange(lookupData["MedicineId"].Select(m => m.Display).ToArray());

			check = new Button
			{
				Text = "Submit",
				Location = new Point(500, 10),
				Size = new Size(120, 55),
				Font = new Font("Segoe UI", 13.8F, FontStyle.Bold),
				ForeColor = Color.White
			};

			check.Click += BtnGetPatients_Click;
			this.Controls.Add(medicines);
			this.Controls.Add(check);
		}

		private async void BtnGetPatients_Click(object sender, EventArgs e)
		{
			if (panel != null)
			{
				if (txtPatients != null)
				{
					panel.Controls.Remove(txtPatients);
				}
				this.Controls.Remove(panel);
			}

			if (medicines.SelectedItem == null)
			{
				MessageBox.Show("Please select a medicine.");
				return;
			}
			string selectedMedicineName = medicines.SelectedItem.ToString();
			var selectedMedicine = lookupData["MedicineId"].FirstOrDefault(m => m.Display == selectedMedicineName);


			txtPatients = new TextBox
			{
				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				Dock = DockStyle.Fill,
				Font = new Font("Segoe UI", 10, FontStyle.Bold),
				ReadOnly = true,
				ForeColor = Color.White,
			};

			panel = new Panel
			{
				Location = new Point(20, 70),
				Size = new Size(300, 300),
				AutoScroll = true,
				BackColor = Color.MidnightBlue
			};

			this.Controls.Add(panel);
			panel.Controls.Add(txtPatients);

			try
			{
				List<string> patients = await controller.GetPatientsByMedicineName((int)selectedMedicine.Value);

				txtPatients.Text = string.Join(Environment.NewLine, patients);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading orders: {ex.Message}");
			}
		}
	}
}
