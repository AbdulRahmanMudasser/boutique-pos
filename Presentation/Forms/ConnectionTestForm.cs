using MaqboolFashion.Data.Database;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Forms
{
    public partial class ConnectionTestForm : Form
    {
        private readonly DatabaseService _dbService = new DatabaseService();

        public ConnectionTestForm()
        {
            InitializeComponent();
            SetupUI();
            _ = TestConnectionAsync();
        }

        private void SetupUI()
        {
            // Form settings
            this.Text = "MaqboolFashion - Database Connection Test";
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Status label
            var lblStatus = new Label
            {
                Text = "Testing database connection...",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(lblStatus);

            // Close button
            var btnClose = new Button
            {
                Text = "Close",
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 40),
                Location = new Point(150, 80)
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private async Task TestConnectionAsync()
        {
            var isConnected = await _dbService.TestConnectionAsync();

            foreach (Control control in this.Controls)
            {
                if (control is Label label)
                {
                    label.Text = isConnected
                        ? "✅ Database connection successful!"
                        : "❌ Database connection failed!";
                    label.ForeColor = isConnected ? Color.Green : Color.Red;
                    break;
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ConnectionTestForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "ConnectionTestForm";
            this.Load += new System.EventHandler(this.ConnectionTestForm_Load);
            this.ResumeLayout(false);

        }

        private void ConnectionTestForm_Load(object sender, EventArgs e)
        {

        }
    }
}