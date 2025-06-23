using Pharmacy.Core;

namespace PharmacyForm
{
	public partial class Main : Form
	{
		private readonly PharmacyController controller;

		public Main(PharmacyController controller)
		{
			InitializeComponent();
			btnAddData.Click += btnAddData_Click;
			btnPatientsByMedicine.Click += btnPatientsByMedicine_Click;
			btnSalesByYear.Click += btnSalesByYear_Click;
			btnLastPrescriptionByPatient.Click += btnLastPrescriptionByPatient_Click;
			btnOrdersToManufacturer.Click += btnOrdersToManufacturer_Click;
			btnPrescriptionsByMedicine.Click += btnPrescriptionsByMedicine_Click;
			btnUnorderedMedicines.Click += btnUnorderedMedicines_Click;
			btnLowStockMedicines.Click += btnLowStockMedicines_Click;
			btnSeniorPatients.Click += btnSeniorPatients_Click;
			btnRecentOrders.Click += btnRecentOrders_Click;
			btnManufacturersWithBgEmail.Click += btnManufacturersWithBgEmail_Click;

			this.controller = controller;
		}

		private void btnAddData_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new AddDataForm(controller));
		}

		private void btnPatientsByMedicine_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new PatientsByMedicineForm());
		}

		private void btnSalesByYear_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new TotalSalesByYearForm());
		}

		private void btnLastPrescriptionByPatient_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new LastPrescriptionForm());
		}

		private void btnOrdersToManufacturer_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new OrdersToManufacturerForm());
		}

		private void btnPrescriptionsByMedicine_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new PrescriptionsByMedicineForm());
		}

		private void btnUnorderedMedicines_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new UnorderedMedicinesForm());
		}

		private void btnLowStockMedicines_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new LowStockMedicinesForm());
		}

		private void btnSeniorPatients_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new SeniorPatientsForm());
		}

		private void btnRecentOrders_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new RecentOrdersForm());
		}

		private void btnManufacturersWithBgEmail_Click(object sender, EventArgs e)
		{
			this.ShowFormWithBackButton(new ManufacturersWithBgEmailForm());
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void ShowFormWithBackButton(Form form)
		{
			form.BackColor = Color.Navy;

			var btnBack = new Button
			{
				//Segoe UI, 13.8pt, style=Bold
				Text = "Back",
				Font = new Font("Segoe UI", 13.8F, FontStyle.Bold),
				Size = new Size(120, 55),
				Location = new Point(10, 10),
				ForeColor = Color.White
			};

			btnBack.Click += (s, e) =>
			{
				this.Show();
				form.Close();
			};

			form.Controls.Add(btnBack);
			form.FormClosed += (s, e) => this.Show();
			form.Show();
			this.Hide();
		}
	}
}

