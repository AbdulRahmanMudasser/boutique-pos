using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Windows.Forms;

namespace MaqboolFashionPOS
{
    public partial class MainForm : Form
    {
        public MainForm()
        {

            InitializeUI();
            TestDatabaseConnection();
        }

        private void InitializeUI()
        {
            // Set up the minimalist UI with white background and black foreground
            this.BackColor = System.Drawing.Color.White;
            this.ForeColor = System.Drawing.Color.Black;
            this.Text = "MaqboolFashion POS System";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new System.Drawing.Size(800, 600);

            // Create status label
            var statusLabel = new Label
            {
                Text = "Database connection status: Testing...",
                ForeColor = System.Drawing.Color.Black,
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),
                AutoSize = true,
                Location = new System.Drawing.Point(20, 20)
            };
            this.Controls.Add(statusLabel);
        }

        private void TestDatabaseConnection()
        {
            // Connection string - in production, use config file with encryption
            string connectionString = @"Data Source=PROBOOK\SQL2022;Initial Catalog=MaqboolFashionDB;Integrated Security=SSPI;Encrypt=False;TrustServerCertificate=True";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Update the status label
                    foreach (Control control in this.Controls)
                    {
                        if (control is Label label && label.Text.StartsWith("Database connection status:"))
                        {
                            label.Text = "Database connection status: Connected to MaqboolFashionDB successfully";
                            label.ForeColor = System.Drawing.Color.Green;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Update the status label with error
                foreach (Control control in this.Controls)
                {
                    if (control is Label label && label.Text.StartsWith("Database connection status:"))
                    {
                        label.Text = $"Database connection status: Failed to connect to MaqboolFashionDB - {ex.Message}";
                        label.ForeColor = System.Drawing.Color.Red;
                        break;
                    }
                }

                // Log the error (in production, use proper logging framework)
                Console.WriteLine($"MaqboolFashionDB connection failed: {ex}");
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}