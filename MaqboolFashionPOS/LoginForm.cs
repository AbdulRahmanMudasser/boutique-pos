using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using BCrypt.Net;

namespace MaqboolFashionPOS
{
    // Login form for user authentication with premium UI
    public partial class LoginForm : Form
    {
        private readonly DatabaseHelper dbHelper = new DatabaseHelper();
        private readonly Timer fadeInTimer = new Timer();

        public LoginForm()
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
            this.FormClosing += (s, e) => Application.Exit();

            var lblTitle = new Label
            {
                Text = "Maqbool Fashion POS",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40), // Deep charcoal text
                Location = new Point(200, 50),
                Size = new Size(400, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var txtUsername = new TextBox
            {
                Name = "txtUsername",
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 40),
                Location = new Point(250, 150),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            var txtPassword = new TextBox
            {
                Name = "txtPassword",
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 40),
                Location = new Point(250, 220),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            var btnLogin = new Button
            {
                Text = "Login",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(300, 50),
                Location = new Point(250, 290),
                BackColor = Color.FromArgb(212, 175, 55), // Gold accent
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(255, 215, 0); // Gold hover
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(212, 175, 55);
            btnLogin.Click += BtnLogin_Click;

            var lnkRegister = new LinkLabel
            {
                Text = "Register",
                Font = new Font("Segoe UI", 10),
                Location = new Point(250, 350),
                Size = new Size(100, 20),
                LinkColor = Color.FromArgb(212, 175, 55),
                ActiveLinkColor = Color.FromArgb(255, 215, 0)
            };
            lnkRegister.Click += (s, e) => FormManager.SwitchForm(new RegisterForm());

            var lnkForgotPassword = new LinkLabel
            {
                Text = "Forgot Password?",
                Font = new Font("Segoe UI", 10),
                Location = new Point(360, 350),
                Size = new Size(150, 20),
                LinkColor = Color.FromArgb(212, 175, 55),
                ActiveLinkColor = Color.FromArgb(255, 215, 0)
            };
            lnkForgotPassword.Click += (s, e) => FormManager.SwitchForm(new ForgotPasswordForm());

            this.Controls.AddRange(new Control[] { lblTitle, txtUsername, txtPassword, btnLogin, lnkRegister, lnkForgotPassword });
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

        // Handles login button click with secure authentication
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var txtUsername = this.Controls["txtUsername"] as TextBox;
            var txtPassword = this.Controls["txtPassword"] as TextBox;

            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT PasswordHash, DeviceId FROM Users WHERE Username = @Username AND IsActive = 1";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", txtUsername.Text);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string passwordHash = reader["PasswordHash"].ToString();
                                string deviceId = reader["DeviceId"].ToString();

                                if (!DeviceBinding.IsValidDevice(deviceId))
                                {
                                    MessageBox.Show("This software is not licensed for this device.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                if (BCrypt.Net.BCrypt.Verify(txtPassword.Text, passwordHash))
                                {
                                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    // TODO: Navigate to main POS dashboard
                                }
                                else
                                {
                                    MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Checks database connection on form load
        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (!dbHelper.TestConnection())
            {
                MessageBox.Show("Failed to connect to the database. Please check your SQL Server configuration.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}