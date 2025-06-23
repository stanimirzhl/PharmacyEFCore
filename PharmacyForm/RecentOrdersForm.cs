using Pharmacy.Core;

namespace PharmacyForm
{
	public partial class RecentOrdersForm : Form
	{
		private readonly PharmacyController controller;
		private TextBox txtOrders;
		private Panel panel;

		public RecentOrdersForm(PharmacyController controller)
		{
			InitializeComponent();

			this.controller = controller;

			this.Load += RecentOrdersForm_Load;
		}

		private async void RecentOrdersForm_Load(object sender, EventArgs e)
		{

			txtOrders = new TextBox
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
			panel.Controls.Add(txtOrders);


			try
			{
				string ordersString = await controller.GetOrdersInTheLast30Days();

				txtOrders.Text = ordersString;

			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading orders: {ex.Message}");
			}
		}
	}
}
