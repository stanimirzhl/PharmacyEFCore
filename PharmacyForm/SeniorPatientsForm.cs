using Pharmacy.Core;

namespace PharmacyForm
{
	public partial class SeniorPatientsForm : Form
	{
		private readonly PharmacyController controller;
		private TextBox txtPatients;
		private Panel panel;

		public SeniorPatientsForm(PharmacyController controller)
		{
			InitializeComponent();

			this.controller = controller;

			this.Load += RecentOrdersForm_Load;
		}

		private async void RecentOrdersForm_Load(object sender, EventArgs e)
		{

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
				string patientString = await controller.GetOldPatients();

				txtPatients.Text = patientString;

			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading orders: {ex.Message}");
			}
		}
	}
}
