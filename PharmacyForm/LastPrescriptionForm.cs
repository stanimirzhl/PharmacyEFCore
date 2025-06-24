using Pharmacy.Core;
using Pharmacy.Data.Data.Models;

namespace PharmacyForm
{
	public partial class LastPrescriptionForm : Form
	{
		private Dictionary<string, List<(string Display, object Value)>> lookupData = new();
		private readonly PharmacyController controller;
		private ComboBox patients;
		private Button check;
		private TextBox txtMedicines;
		private Panel panel;

		public LastPrescriptionForm(PharmacyController controller)
		{
			InitializeComponent();

			this.controller = controller;

			this.Load += LastPrescriptionForm_Load;
		}

		private async void LastPrescriptionForm_Load(object sender, EventArgs e)
		{
			try
			{
				lookupData["PatientId"] = (await controller.GetAllPatientsData()).Select(p => (p.PatientName, (object)p.Id)).ToList();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading patients: {ex.Message}");
				return;
			}

			patients = new ComboBox
			{
				Location = new Point(170, 20),
				Width = 300,
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 12)
			};

			patients.Items.AddRange(lookupData["PatientId"].Select(p => p.Display).ToArray());

			check = new Button
			{
				Text = "Submit",
				Location = new Point(500, 10),
				Size = new Size(120, 55),
				Font = new Font("Segoe UI", 13.8F, FontStyle.Bold),
				ForeColor = Color.White
			};

			check.Click += BtnGetMedicines_Click;
			this.Controls.Add(patients);
			this.Controls.Add(check);
		}	

		private async void BtnGetMedicines_Click(object sender, EventArgs e)
		{
			if (panel != null)
			{
				if (txtMedicines != null)
				{
					panel.Controls.Remove(txtMedicines);
				}
				this.Controls.Remove(panel);
			}

			if (patients.SelectedItem == null)
			{
				MessageBox.Show("Please select a patient.");
				return;
			}
			string selectedPatientName = patients.SelectedItem.ToString();
			var selectedPatient = lookupData["PatientId"].FirstOrDefault(m => m.Display == selectedPatientName);


			txtMedicines = new TextBox
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
			panel.Controls.Add(txtMedicines);

			try
			{
				string prescriptions = await controller.GetAllOrdersByManufacturer((int)selectedPatient.Value);

				txtMedicines.Text = prescriptions;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading orders: {ex.Message}");
			}
		}
	}
}
