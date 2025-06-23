using Pharmacy.Core;

namespace PharmacyForm
{
	public partial class UnorderedMedicinesForm : Form
	{
		private readonly PharmacyController controller;
		private TextBox txtUnordered;
		private Panel panel;

		public UnorderedMedicinesForm(PharmacyController controller)
		{
			InitializeComponent();

			this.controller = controller;

			this.Load += UnorderedMedicinesForm_Load;
		}

		private async void UnorderedMedicinesForm_Load(object sender, EventArgs e)
		{

			txtUnordered = new TextBox
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
			panel.Controls.Add(txtUnordered);


			try
			{
				string unorderedString = await controller.GetUnorderedMedicines();

				txtUnordered.Text = unorderedString;

			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading orders: {ex.Message}");
			}
		}
	}
}
