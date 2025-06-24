using Pharmacy.Core;
using Pharmacy.Data.Data.Models;

namespace PharmacyForm
{
	public partial class OrdersToManufacturerForm : Form
	{
		private Dictionary<string, List<(string Display, object Value)>> lookupData = new();
		private readonly PharmacyController controller;
		private ComboBox manufacturers;
		private Button check;
		private TextBox txtOrders;
		private Panel panel;

		public OrdersToManufacturerForm(PharmacyController controller)
		{
			InitializeComponent();

			this.controller = controller;

			this.Load += OrdersToManufacturerForm_Load;
		}

		private async void OrdersToManufacturerForm_Load(object sender, EventArgs e)
		{
			try
			{
				lookupData["ManufacturerId"] = (await controller.GetAllManufacturersData()).Select(m => (m.ManufacturerName, (object)m.Id)).ToList();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading manufacturers: {ex.Message}");
				return;
			}

			manufacturers = new ComboBox
			{
				Location = new Point(170, 20),
				Width = 300,
				DropDownStyle = ComboBoxStyle.DropDownList,
				Font = new Font("Segoe UI", 12)
			};

			manufacturers.Items.AddRange(lookupData["ManufacturerId"].Select(m => m.Display).ToArray());

			check = new Button
			{
				Text = "Submit",
				Location = new Point(500, 10),
				Size = new Size(120, 55),
				Font = new Font("Segoe UI", 13.8F, FontStyle.Bold),
				ForeColor = Color.White
			};

			check.Click += BtnGetMedicines_Click;
			this.Controls.Add(manufacturers);
			this.Controls.Add(check);
		}

		private async void BtnGetMedicines_Click(object sender, EventArgs e)
		{
			if (panel != null)
			{
				if (txtOrders != null)
				{
					panel.Controls.Remove(txtOrders);
				}
				this.Controls.Remove(panel);
			}

			if (manufacturers.SelectedItem == null)
			{
				MessageBox.Show("Please select a manufacturer.");
				return;
			}
			string selectedManufacturerName = manufacturers.SelectedItem.ToString();
			var selectedManufacturer = lookupData["ManufacturerId"].FirstOrDefault(m => m.Display == selectedManufacturerName);


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
				string prescriptions = await controller.GetAllOrdersByManufacturer((int)selectedManufacturer.Value);

				txtOrders.Text = prescriptions;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading orders: {ex.Message}");
			}
		}
	}
}
