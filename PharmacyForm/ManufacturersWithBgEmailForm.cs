using Pharmacy.Core;

namespace PharmacyForm
{
	public partial class ManufacturersWithBgEmailForm : Form
	{
		private readonly PharmacyController controller;
		private TextBox txtManufacturers;
		private Panel panel;

		public ManufacturersWithBgEmailForm(PharmacyController controller)
		{
			InitializeComponent();

			this.controller = controller;
			this.Load += ManufacturersWithBgEmailForm_Load;
		}

		private async void ManufacturersWithBgEmailForm_Load(object sender, EventArgs e)
		{

			txtManufacturers = new TextBox
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
			panel.Controls.Add(txtManufacturers);


			try
			{
				string manufacturersString = await controller.GetAllManufacturersWithEmailEndingInBg();

				txtManufacturers.Text = manufacturersString;

			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading manufacturers: {ex.Message}");
			}
		}
	}
}
