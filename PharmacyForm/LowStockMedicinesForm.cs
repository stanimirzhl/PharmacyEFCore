using Pharmacy.Core;

namespace PharmacyForm
{
	public partial class LowStockMedicinesForm : Form
	{
		private readonly PharmacyController controller;
		private TextBox txtLowMedicines;
		private Panel panel;

		public LowStockMedicinesForm(PharmacyController controller)
		{
			InitializeComponent();

			this.controller = controller;

			this.Load += LowStockMedicinesForm_Load;
		}

		private async void LowStockMedicinesForm_Load(object sender, EventArgs e)
		{

			txtLowMedicines = new TextBox
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
			panel.Controls.Add(txtLowMedicines);


			try
			{
				string lowStockString = await controller.GetLowOnStockMedicinesInPharmacy();

				txtLowMedicines.Text = lowStockString;

			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading orders: {ex.Message}");
			}
		}
	}
}
