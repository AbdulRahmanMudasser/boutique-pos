using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MaqboolFashionPOS
{
    // Forgot Password form for initiating password reset with premium UI
    public partial class ForgotPasswordForm : Form
    {
        private readonly DatabaseHelper dbHelper = new DatabaseHelper();
        private readonly Timer fadeInTimer = new Timer();

        public ForgotPasswordForm()
        {
            InitializeComponent(); // Designer-generated initialization
            SetupControls(); // Programmatic control setup
            InitializeFadeIn(); // Smooth fade-in animation
        }

        // Sets up UI controls programmatically for a luxurious look
        private void SetupControls()
        {
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 220); // Soft ivory background

            var lblTitle = new Label
            {
                Text = "Forgot Password - Maqbool Fashion POS",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40), // Deep charcoal text
                Location = new Point(200, 50),
                Size = new Size(400, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var txtEmail = new TextBox
            {
                Name = "txtEmail",
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 40),
                Location = new Point(250, 150),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            var btnReset = new Button
            {
                Text = "Send Reset Link",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(300, 50),
                Location = new Point(250, 220),
                BackColor = Color.FromArgb(212, 175, 55), // Gold accent
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReset.MouseEnter += (s, e) => btnReset.BackColor = Color.FromArgb(255, 215, 0); // Gold hover
            btnReset.MouseLeave += (s, e) => btnReset.BackColor = Color.FromArgb(212, 175, 55);
            btnReset.Click += BtnReset_Click;

            var lnkBack = new LinkLabel
            {
                Text = "Back to Login",
                Font = new Font("Segoe UI", 10),
                Location = new Point(250, 280),
                Size = new Size(100, 20),
                LinkColor = Color.FromArgb(212, 175, 55),
                ActiveLinkColor = Color.FromArgb(255, 215, 0)
            };
            lnkBack.Click += (s, e) => FormManager.SwitchForm(new LoginForm());

            this.Controls.AddRange(new Control[] { lblTitle, txtEmail, btnReset, lnkBack });
        }

        // Initializes fade-in animation for premium feel
        private void InitializeFadeIn()
        {
            this.Opacity = 0;
            fadeInTimer.Interval = 50;
            fadeInTimer.Tick += (s, e) =>
            {
                this.Opacity += 0.05;
                if (this.Opacity >= 1) fadeInTimer.Stop();
            };
            fadeInTimer.Start();
        }

        // Handles reset button click with email validation
        private void BtnReset_Click(object sender, EventArgs e)
        {
            var txtEmail = this.Controls["txtEmail"] as TextBox;

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter your email address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND IsActive = 1";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", txtEmail.Text);
                        int count = (int)command.ExecuteScalar();
                        if (count > 0)
                        {
                            // Placeholder for email-based reset logic (e.g., send reset link)
                            MessageBox.Show("A password reset link would be sent to your email (not implemented).",
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            FormManager.SwitchForm(new LoginForm());
                        }
                        else
                        {
                            MessageBox.Show("No account found with this email.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}