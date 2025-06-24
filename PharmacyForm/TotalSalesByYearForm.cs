using Pharmacy.Core;
using Pharmacy.Data.Data.Models;

namespace PharmacyForm
{
	public partial class TotalSalesByYearForm : Form
	{
		private readonly PharmacyController controller;
		private DateTimePicker yearPicker;
		private Button check;
		private TextBox txtSales;
		private Panel panel;

		public TotalSalesByYearForm(PharmacyController controller)
		{
			InitializeComponent();

			this.controller = controller;

			this.Load += TotalSalesByYearForm_Load;
		}

		private async void TotalSalesByYearForm_Load(object sender, EventArgs e)
		{
			yearPicker = new DateTimePicker
			{
				Location = new Point(170, 20),
				Width = 300,
				Font = new Font("Segoe UI", 12),
				MaxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
				Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
				Format = DateTimePickerFormat.Custom,
				CustomFormat = "yyyy"
			};

			check = new Button
			{
				Text = "Submit",
				Location = new Point(500, 10),
				Size = new Size(120, 55),
				Font = new Font("Segoe UI", 13.8F, FontStyle.Bold),
				ForeColor = Color.White
			};

			check.Click += BtnGetTotalSales_Click;

			this.Controls.Add(yearPicker);
			this.Controls.Add(check);
		}

		private async void BtnGetTotalSales_Click(object sender, EventArgs e)
		{
			if (panel != null)
			{
				if (txtSales != null)
				{
					panel.Controls.Remove(txtSales);
				}
				this.Controls.Remove(panel);
			}

			int selectedYear = yearPicker.Value.Year;


			txtSales = new TextBox
			{
				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				Dock = DockStyle.Fill,
				Font = new Font("Segoe UI", 13, FontStyle.Bold),
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
			panel.Controls.Add(txtSales);

			try
			{
				decimal sales = await controller.GetTotalSalesByYear(selectedYear);

				txtSales.Text = $"Year: {selectedYear}\r\nTotal sales: {sales:f2}";
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading orders: {ex.Message}");
			}
		}
	}
}
