using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;

namespace MaqboolFashionPOS
{
    // Registration form for creating new user accounts with premium UI
    public partial class RegisterForm : Form
    {
        private readonly DatabaseHelper dbHelper = new DatabaseHelper();
        private readonly Timer fadeInTimer = new Timer();

        public RegisterForm()
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
                Text = "Register - Maqbool Fashion POS",
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

            var txtEmail = new TextBox
            {
                Name = "txtEmail",
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 40),
                Location = new Point(250, 220),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            var txtPassword = new TextBox
            {
                Name = "txtPassword",
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 40),
                Location = new Point(250, 290),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            var txtConfirmPassword = new TextBox
            {
                Name = "txtConfirmPassword",
                Font = new Font("Segoe UI", 12),
                Size = new Size(300, 40),
                Location = new Point(250, 360),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            var btnRegister = new Button
            {
                Text = "Register",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(300, 50),
                Location = new Point(250, 430),
                BackColor = Color.FromArgb(212, 175, 55), // Gold accent
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegister.MouseEnter += (s, e) => btnRegister.BackColor = Color.FromArgb(255, 215, 0); // Gold hover
            btnRegister.MouseLeave += (s, e) => btnRegister.BackColor = Color.FromArgb(212, 175, 55);
            btnRegister.Click += BtnRegister_Click;

            var lnkBack = new LinkLabel
            {
                Text = "Back to Login",
                Font = new Font("Segoe UI", 10),
                Location = new Point(250, 490),
                Size = new Size(100, 20),
                LinkColor = Color.FromArgb(212, 175, 55),
                ActiveLinkColor = Color.FromArgb(255, 215, 0)
            };
            lnkBack.Click += (s, e) => FormManager.SwitchForm(new LoginForm());

            this.Controls.AddRange(new Control[] { lblTitle, txtUsername, txtEmail, txtPassword, txtConfirmPassword, btnRegister, lnkBack });
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

        // Handles register button click with secure user creation
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            var txtUsername = this.Controls["txtUsername"] as TextBox;
            var txtEmail = this.Controls["txtEmail"] as TextBox;
            var txtPassword = this.Controls["txtPassword"] as TextBox;
            var txtConfirmPassword = this.Controls["txtConfirmPassword"] as TextBox;

            if (!ValidateInput(txtUsername.Text, txtEmail.Text, txtPassword.Text, txtConfirmPassword.Text))
                return;

            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text);
                string deviceId = DeviceBinding.GetMachineId();

                using (var connection = dbHelper.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO Users (Username, PasswordHash, Email, DeviceId) VALUES (@Username, @PasswordHash, @Email, @DeviceId)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", txtUsername.Text);
                        command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        command.Parameters.AddWithValue("@Email", txtEmail.Text);
                        command.Parameters.AddWithValue("@DeviceId", deviceId);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Registration successful! Please log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FormManager.SwitchForm(new LoginForm());
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint violation
                    MessageBox.Show("Username or email already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Validates user input for registration
        private bool ValidateInput(string username, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[A-Z]") ||
                !Regex.IsMatch(password, @"[a-z]") || !Regex.IsMatch(password, @"[0-9]"))
            {
                MessageBox.Show("Password must be at least 8 characters long and contain uppercase, lowercase, and a number.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}