using MaqboolFashion.Data.Database;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Forms
{
    public class SignupForm : Form
    {
        private readonly UserService _userService = new UserService();
        private TextBox txtFirstName, txtLastName, txtEmail, txtPassword, txtConfirmPassword;
        private PictureBox eyeIcon;
        private bool passwordVisible = false;

        public SignupForm()
        {
            // Form settings
            this.Text = "MaqboolFashion - Sign Up";
            this.ClientSize = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Padding = new Padding(10);

            InitializeUI();
        }

        private void InitializeUI()
        {
            // Main container with shadow effect
            var mainContainer = new Panel
            {
                BackColor = Color.White,
                Size = new Size(900, 600),
                Location = new Point(50, 50),
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(mainContainer);

            // Add exit button (top right)
            var btnExit = new Button
            {
                Text = "X",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(40, 40),
                Location = new Point(mainContainer.Width - 45, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.Black
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            btnExit.Click += (s, e) => Application.Exit();
            mainContainer.Controls.Add(btnExit);

            // Left side - Branding with background image
            var leftPanel = new Panel
            {
                Size = new Size(400, 600),
                Location = new Point(0, 0),
                BorderStyle = BorderStyle.None
            };
            mainContainer.Controls.Add(leftPanel);

            // Right side - Signup Form
            var rightPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(500, 600),
                Location = new Point(400, 0),
                BorderStyle = BorderStyle.None
            };
            mainContainer.Controls.Add(rightPanel);

            // Load branding section with logo and name
            LoadBrandingSection(leftPanel);

            // Create form elements
            CreateFormElements(rightPanel);
        }

        private void LoadBrandingSection(Panel panel)
        {
            try
            {
                // Load background image
                var bgPath = Path.Combine(Application.StartupPath, "Assets", "background-1.png");
                if (File.Exists(bgPath))
                {
                    var background = new PictureBox
                    {
                        Image = Image.FromFile(bgPath),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Dock = DockStyle.Fill
                    };
                    panel.Controls.Add(background);

                    // Add dark overlay
                    var overlay = new Panel
                    {
                        BackColor = Color.FromArgb(120, 0, 0, 0),
                        Dock = DockStyle.Fill
                    };
                    panel.Controls.Add(overlay);
                }
                else
                {
                    panel.BackColor = Color.FromArgb(30, 30, 30);
                }

                // Brand content container
                var brandContent = new Panel
                {
                    Size = new Size(350, 250),
                    Location = new Point(25, 200),
                    BackColor = Color.Transparent
                };
                panel.Controls.Add(brandContent);

                // Load logo
                var logoPath = Path.Combine(Application.StartupPath, "Assets", "logo.png");
                if (File.Exists(logoPath))
                {
                    var logo = new PictureBox
                    {
                        Image = Image.FromFile(logoPath),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Size = new Size(150, 150),
                        Location = new Point(100, 0)
                    };
                    brandContent.Controls.Add(logo);
                }

                // Brand name - Made more visible
                var lblBrand = new Label
                {
                    Text = "MAQBOOL FASHION",
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(50, 160)
                };
                brandContent.Controls.Add(lblBrand);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading branding: {ex.Message}");
            }
        }

        private void CreateFormElements(Panel panel)
        {
            // Form header
            var lblWelcome = new Label
            {
                Text = "CREATE ACCOUNT",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, 40)
            };
            panel.Controls.Add(lblWelcome);

            var lblSubtitle = new Label
            {
                Text = "Join Maqbool Fashion's exclusive community",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(50, 80)
            };
            panel.Controls.Add(lblSubtitle);

            // Form fields with labels
            var fieldWidth = 400;
            var fieldHeight = 40;
            var fieldPadding = 15;
            var startY = 130;

            // First Name Label
            var lblFirstName = new Label
            {
                Text = "First Name",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, startY)
            };
            panel.Controls.Add(lblFirstName);

            // First Name Field
            txtFirstName = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(fieldWidth, fieldHeight),
                Location = new Point(50, startY + 20),
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(txtFirstName);

            // Last Name Label
            var lblLastName = new Label
            {
                Text = "Last Name",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, startY + fieldHeight + 20 + 5)
            };
            panel.Controls.Add(lblLastName);

            // Last Name Field
            txtLastName = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(fieldWidth, fieldHeight),
                Location = new Point(50, startY + fieldHeight + 40 + 5),
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(txtLastName);

            // Email Label
            var lblEmail = new Label
            {
                Text = "Email Address",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 2 + 20)
            };
            panel.Controls.Add(lblEmail);

            // Email Field
            txtEmail = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(fieldWidth, fieldHeight),
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 2 + 40),
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(txtEmail);

            // Password Label
            var lblPassword = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 3 + 20)
            };
            panel.Controls.Add(lblPassword);

            // Password Field
            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(fieldWidth, fieldHeight),
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 3 + 40),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '•'
            };
            panel.Controls.Add(txtPassword);

            // Confirm Password Label
            var lblConfirmPassword = new Label
            {
                Text = "Confirm Password",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 4 + 20)
            };
            panel.Controls.Add(lblConfirmPassword);

            // Confirm Password Field
            txtConfirmPassword = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(fieldWidth, fieldHeight),
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 4 + 40),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '•'
            };
            panel.Controls.Add(txtConfirmPassword);

            // Eye icon for password visibility
            eyeIcon = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(410, startY + (fieldHeight + fieldPadding) * 3 + 48),
                Cursor = Cursors.Hand,
                BackColor = Color.White
            };
            UpdateEyeIcon();
            eyeIcon.Click += (s, e) => TogglePasswordVisibility();
            panel.Controls.Add(eyeIcon);

            // Create Account Button
            var btnCreateAccount = new Button
            {
                Text = "CREATE ACCOUNT",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(fieldWidth, 50),
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 5 + 40),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCreateAccount.FlatAppearance.BorderSize = 0;
            btnCreateAccount.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);
            btnCreateAccount.Click += BtnCreateAccount_Click;
            panel.Controls.Add(btnCreateAccount);

            // Already have account - made more visible
            var lblHaveAccount = new Label
            {
                Text = "Already have an account?",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(120, startY + (fieldHeight + fieldPadding) * 5 + 110)
            };
            panel.Controls.Add(lblHaveAccount);

            // Sign In link - made more prominent
            var linkSignIn = new LinkLabel
            {
                Text = "SIGN IN",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                LinkColor = Color.Black,
                ActiveLinkColor = Color.Black,
                AutoSize = true,
                Location = new Point(280, startY + (fieldHeight + fieldPadding) * 5 + 110),
                Cursor = Cursors.Hand
            };
            linkSignIn.Click += (s, e) =>
            {
                //var loginForm = new LoginForm();
                //loginForm.Show();
                //this.Hide();
            };
            panel.Controls.Add(linkSignIn);
        }

        private void UpdateEyeIcon()
        {
            try
            {
                var iconPath = Path.Combine(Application.StartupPath, "Assets", "Icons",
                    passwordVisible ? "eye-show.png" : "eye-hide.png");

                if (File.Exists(iconPath))
                {
                    eyeIcon.Image = Image.FromFile(iconPath);
                    return;
                }
            }
            catch { }

            // Fallback to drawn icon
            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                using (var pen = new Pen(Color.Black, 2))
                {
                    g.DrawEllipse(pen, 2, 2, 20, 20);
                    if (!passwordVisible) g.DrawLine(pen, 2, 2, 22, 22);
                }
            }
            eyeIcon.Image = bmp;
        }

        private void TogglePasswordVisibility()
        {
            passwordVisible = !passwordVisible;
            txtPassword.PasswordChar = passwordVisible ? '\0' : '•';
            txtConfirmPassword.PasswordChar = passwordVisible ? '\0' : '•';
            UpdateEyeIcon();
        }

        private void BtnCreateAccount_Click(object sender, EventArgs e)
        {
            // Validate First Name and Last Name
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please enter your first and last name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return;
            }

            // Validate Email Format
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Basic email regex
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !Regex.IsMatch(txtEmail.Text, emailPattern))
            {
                MessageBox.Show("Please enter a valid email address (e.g., user@domain.com)", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // Check if User Already Exists
            try
            {
                if (_userService.CheckUserExists(txtEmail.Text))
                {
                    MessageBox.Show("An account with this email already exists", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking user existence: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate Password
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$";
            if (!Regex.IsMatch(txtPassword.Text, passwordPattern))
            {
                MessageBox.Show("Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character (!@#$%^&*)", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Validate Confirm Password
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Register User
            try
            {
                bool success = _userService.RegisterUser(
                    txtFirstName.Text,
                    txtLastName.Text,
                    txtEmail.Text,
                    txtPassword.Text);

                if (success)
                {
                    MessageBox.Show("Account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Clear form
                    txtFirstName.Clear();
                    txtLastName.Clear();
                    txtEmail.Clear();
                    txtPassword.Clear();
                    txtConfirmPassword.Clear();
                    txtFirstName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}