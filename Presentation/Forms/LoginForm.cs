using MaqboolFashion.Data.Database;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Forms
{
    public class LoginForm : Form
    {
        private readonly UserService _userService = new UserService();
        private readonly NavigationService _navigationService;
        private TextBox txtEmail, txtPassword;
        private PictureBox eyeIcon;
        private bool passwordVisible = false;

        public LoginForm()
        {
            // Initialize NavigationService
            _navigationService = NavigationService.Instance(this);

            // Form settings
            this.Text = "MaqboolFashion - Log In";
            this.ClientSize = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Padding = new Padding(10);

            InitializeUI();
        }

        private void InitializeUI()
        {
            // Main container
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
            btnExit.Click += (s, e) => _navigationService.ExitApplication();
            mainContainer.Controls.Add(btnExit);

            // Left side - Branding with background image
            var leftPanel = new Panel
            {
                Size = new Size(400, 600),
                Location = new Point(0, 0),
                BorderStyle = BorderStyle.None
            };
            mainContainer.Controls.Add(leftPanel);

            // Right side - Login Form
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

                // Brand name
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
                Text = "LOG IN",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, 40)
            };
            panel.Controls.Add(lblWelcome);

            var lblSubtitle = new Label
            {
                Text = "Welcome back to Maqbool Fashion",
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

            // Email Label
            var lblEmail = new Label
            {
                Text = "Email Address",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(50, startY)
            };
            panel.Controls.Add(lblEmail);

            // Email Field
            txtEmail = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(fieldWidth, fieldHeight),
                Location = new Point(50, startY + 20),
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
                Location = new Point(50, startY + fieldHeight + 20 + fieldPadding)
            };
            panel.Controls.Add(lblPassword);

            // Password Field
            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(fieldWidth, fieldHeight),
                Location = new Point(50, startY + fieldHeight + 40 + fieldPadding),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '•'
            };
            panel.Controls.Add(txtPassword);

            // Eye icon for password visibility
            eyeIcon = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(410, startY + fieldHeight + 48 + fieldPadding),
                Cursor = Cursors.Hand,
                BackColor = Color.White
            };
            UpdateEyeIcon();
            eyeIcon.Click += (s, e) => TogglePasswordVisibility();
            panel.Controls.Add(eyeIcon);

            // Log In Button
            var btnLogin = new Button
            {
                Text = "LOG IN",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(fieldWidth, 50),
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 2 + 40),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);
            btnLogin.Click += BtnLogin_Click;
            panel.Controls.Add(btnLogin);

            // Don't have an account
            var lblNoAccount = new Label
            {
                Text = "Don't have an account?",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(120, startY + (fieldHeight + fieldPadding) * 2 + 110)
            };
            panel.Controls.Add(lblNoAccount);

            // Sign Up link
            var linkSignUp = new LinkLabel
            {
                Text = "SIGN UP",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                LinkColor = Color.Black,
                ActiveLinkColor = Color.Black,
                AutoSize = true,
                Location = new Point(280, startY + (fieldHeight + fieldPadding) * 2 + 110),
                Cursor = Cursors.Hand
            };
            linkSignUp.Click += (s, e) => _navigationService.ShowSignupForm();
            panel.Controls.Add(linkSignUp);
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
            UpdateEyeIcon();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LoginForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // Validate Email Format
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !Regex.IsMatch(txtEmail.Text, emailPattern))
            {
                MessageBox.Show("Please enter a valid email address (e.g., user@domain.com)", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // Validate Password
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter your password", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            // Attempt Login
            try
            {
                bool success = _userService.LoginUser(txtEmail.Text, txtPassword.Text);
                if (success)
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Clear form
                    txtEmail.Clear();
                    txtPassword.Clear();
                    txtEmail.Focus();
                    // TODO: Navigate to main application
                }
                else
                {
                    MessageBox.Show("Invalid email or password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}