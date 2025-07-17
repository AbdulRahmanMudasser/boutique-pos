using MaqboolFashion.Data.Database;
using MaqboolFashion.Presentation.Services;
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
        private readonly NavigationService _navigationService;
        private TextBox txtFirstName, txtLastName, txtEmail, txtPassword, txtConfirmPassword;
        private PictureBox eyeIcon, eyeIconConfirm;
        private Button btnCreateAccount;
        private Label lblPasswordStrength, lblValidationMessage;
        private ProgressBar progressBarPassword;
        private bool passwordVisible = false;
        private bool confirmPasswordVisible = false;
        private bool isProcessing = false;

        public SignupForm()
        {
            _navigationService = NavigationService.Instance(this);

            this.Text = "MaqboolFashion - Create Account";
            this.ClientSize = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Padding = new Padding(10);
            this.KeyPreview = true;

            this.KeyDown += SignupForm_KeyDown;

            InitializeUI();
            SetupFormValidation();
        }

        private void SignupForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _navigationService.ExitApplication();
            }
            else if (e.Control && e.KeyCode == Keys.L)
            {
                _navigationService.ShowLoginForm();
            }
            else if (e.KeyCode == Keys.Enter && !isProcessing)
            {
                BtnCreateAccount_Click(this, EventArgs.Empty);
            }
        }

        private void InitializeUI()
        {
            var mainContainer = new Panel
            {
                BackColor = Color.White,
                Size = new Size(900, 600),
                Location = new Point(50, 50),
                BorderStyle = BorderStyle.None
            };

            mainContainer.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, mainContainer.ClientRectangle,
                    Color.FromArgb(230, 230, 230), ButtonBorderStyle.Solid);
            };

            this.Controls.Add(mainContainer);

            var btnExit = CreateModernButton("×", new Point(mainContainer.Width - 45, 5), new Size(40, 40));
            btnExit.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            btnExit.ForeColor = Color.FromArgb(120, 120, 120);
            btnExit.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 100, 100);
            btnExit.FlatAppearance.MouseDownBackColor = Color.FromArgb(255, 80, 80);
            btnExit.Click += (s, e) => _navigationService.ExitApplication();
            mainContainer.Controls.Add(btnExit);

            var leftPanel = new Panel
            {
                Size = new Size(400, 600),
                Location = new Point(0, 0),
                BorderStyle = BorderStyle.None
            };
            mainContainer.Controls.Add(leftPanel);

            var rightPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(500, 500),
                Location = new Point(400, 60),
                BorderStyle = BorderStyle.None,
                AutoScroll = true,
                AutoScrollMinSize = new Size(480, 700)
            };
            mainContainer.Controls.Add(rightPanel);

            LoadEnhancedBrandingSection(leftPanel);
            CreateEnhancedFormElements(rightPanel);
        }

        private Button CreateModernButton(string text, Point location, Size size)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = size,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
        }

        private void LoadEnhancedBrandingSection(Panel panel)
        {
            try
            {
                panel.Paint += (s, e) =>
                {
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        panel.ClientRectangle,
                        Color.FromArgb(45, 45, 45),
                        Color.FromArgb(25, 25, 25),
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        e.Graphics.FillRectangle(brush, panel.ClientRectangle);
                    }
                };

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

                    var overlay = new Panel { Dock = DockStyle.Fill };
                    overlay.Paint += (s, e) =>
                    {
                        using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                            overlay.ClientRectangle,
                            Color.FromArgb(150, 0, 0, 0),
                            Color.FromArgb(100, 0, 0, 0),
                            System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                        {
                            e.Graphics.FillRectangle(brush, overlay.ClientRectangle);
                        }
                    };
                    panel.Controls.Add(overlay);
                }

                var brandContent = new Panel
                {
                    Size = new Size(350, 280),
                    Location = new Point(25, 180),
                    BackColor = Color.Transparent
                };
                panel.Controls.Add(brandContent);

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

                var lblBrand = new Label
                {
                    Text = "MAQBOOL FASHION",
                    Font = new Font("Segoe UI", 20, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(45, 160)
                };
                brandContent.Controls.Add(lblBrand);

                var lblTagline = new Label
                {
                    Text = "Elevating Style, Defining Fashion",
                    Font = new Font("Segoe UI", 11, FontStyle.Italic),
                    ForeColor = Color.FromArgb(220, 220, 220),
                    AutoSize = true,
                    Location = new Point(65, 195)
                };
                brandContent.Controls.Add(lblTagline);

                var features = new[] { "• Premium Quality", "• Latest Trends", "• Secure Shopping" };
                for (int i = 0; i < features.Length; i++)
                {
                    var lblFeature = new Label
                    {
                        Text = features[i],
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.FromArgb(200, 200, 200),
                        AutoSize = true,
                        Location = new Point(70, 230 + (i * 20))
                    };
                    brandContent.Controls.Add(lblFeature);
                }
            }
            catch (Exception ex)
            {
                ShowValidationMessage($"Error loading branding: {ex.Message}", true);
            }
        }

        private void CreateEnhancedFormElements(Panel panel)
        {
            var lblWelcome = new Label
            {
                Text = "CREATE ACCOUNT",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true,
                Location = new Point(50, 35)
            };
            panel.Controls.Add(lblWelcome);

            var lblSubtitle = new Label
            {
                Text = "Join our exclusive fashion community today",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(120, 120, 120),
                AutoSize = true,
                Location = new Point(50, 75)
            };
            panel.Controls.Add(lblSubtitle);

            lblValidationMessage = new Label
            {
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(220, 53, 69),
                AutoSize = true,
                Location = new Point(50, 105),
                Visible = false
            };
            panel.Controls.Add(lblValidationMessage);

            var fieldWidth = 400;
            var fieldHeight = 45;
            var fieldPadding = 25;
            var startY = 140;

            txtFirstName = CreateEnhancedField("First Name *", panel, startY, fieldWidth, fieldHeight);
            txtLastName = CreateEnhancedField("Last Name *", panel, startY + (fieldHeight + fieldPadding), fieldWidth, fieldHeight);
            txtEmail = CreateEnhancedField("Email Address *", panel, startY + (fieldHeight + fieldPadding) * 2, fieldWidth, fieldHeight);

            txtPassword = CreateEnhancedField("Password *", panel, startY + (fieldHeight + fieldPadding) * 3, fieldWidth, fieldHeight);
            eyeIcon = CreatePasswordField("Password *", txtPassword, panel, startY + (fieldHeight + fieldPadding) * 3, fieldWidth, fieldHeight, false);

            lblPasswordStrength = new Label
            {
                Text = "Password strength: Weak",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(120, 120, 120),
                AutoSize = true,
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 3 + fieldHeight + 5)
            };
            panel.Controls.Add(lblPasswordStrength);

            progressBarPassword = new ProgressBar
            {
                Size = new Size(200, 8),
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 3 + fieldHeight + 25),
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.FromArgb(220, 53, 69)
            };
            panel.Controls.Add(progressBarPassword);

            txtConfirmPassword = CreateEnhancedField("Confirm Password *", panel, startY + (fieldHeight + fieldPadding) * 4 + 40, fieldWidth, fieldHeight);
            eyeIconConfirm = CreatePasswordField("Confirm Password *", txtConfirmPassword, panel, startY + (fieldHeight + fieldPadding) * 4 + 40, fieldWidth, fieldHeight, true);

            btnCreateAccount = new Button
            {
                Text = "CREATE ACCOUNT",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(fieldWidth, 50),
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 5 + 80),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCreateAccount.FlatAppearance.BorderSize = 0;
            btnCreateAccount.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);
            btnCreateAccount.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 30, 30);
            btnCreateAccount.Click += BtnCreateAccount_Click;
            panel.Controls.Add(btnCreateAccount);

            var lblHaveAccount = new Label
            {
                Text = "Already have an account?",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(120, startY + (fieldHeight + fieldPadding) * 5 + 150)
            };
            panel.Controls.Add(lblHaveAccount);

            var linkSignIn = new LinkLabel
            {
                Text = "SIGN IN",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                LinkColor = Color.FromArgb(40, 40, 40),
                ActiveLinkColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Location = new Point(280, startY + (fieldHeight + fieldPadding) * 5 + 150),
                Cursor = Cursors.Hand
            };
            linkSignIn.LinkBehavior = LinkBehavior.HoverUnderline;
            linkSignIn.Click += (s, e) => AnimateToLoginForm();
            panel.Controls.Add(linkSignIn);
        }

        private TextBox CreateEnhancedField(string labelText, Panel panel, int y, int width, int height)
        {
            var label = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(80, 80, 80),
                AutoSize = true,
                Location = new Point(50, y - 20)
            };
            panel.Controls.Add(label);

            var textBox = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(width, height),
                Location = new Point(50, y),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 250)
            };

            textBox.Enter += (s, e) => ((TextBox)s).BackColor = Color.White;
            textBox.Leave += (s, e) => ((TextBox)s).BackColor = Color.FromArgb(250, 250, 250);

            panel.Controls.Add(textBox);
            return textBox;
        }

        private PictureBox CreatePasswordField(string labelText, TextBox textBox, Panel panel, int y, int width, int height, bool isConfirm)
        {
            if (textBox == null) return null;

            textBox.PasswordChar = '•';

            if (!isConfirm)
            {
                textBox.TextChanged += TxtPassword_TextChanged;
            }

            var eyeIconOut = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(410, y + 10),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };

            panel.Controls.Add(eyeIconOut);

            UpdateEyeIcon(eyeIconOut, isConfirm ? confirmPasswordVisible : passwordVisible);
            eyeIconOut.Click += (s, e) => TogglePasswordVisibility(isConfirm);

            return eyeIconOut;
        }

        private void SetupFormValidation()
        {
            txtFirstName.TextChanged += (s, e) => ValidateField(txtFirstName, !string.IsNullOrWhiteSpace(txtFirstName.Text));
            txtLastName.TextChanged += (s, e) => ValidateField(txtLastName, !string.IsNullOrWhiteSpace(txtLastName.Text));
            txtEmail.TextChanged += (s, e) => ValidateField(txtEmail, IsValidEmail(txtEmail.Text));
            txtPassword.TextChanged += (s, e) => ValidateField(txtPassword, IsValidPassword(txtPassword.Text));
            txtConfirmPassword.TextChanged += (s, e) => ValidateField(txtConfirmPassword, txtPassword.Text == txtConfirmPassword.Text && !string.IsNullOrEmpty(txtConfirmPassword.Text));
        }

        private void ValidateField(TextBox textBox, bool isValid)
        {
            textBox.BackColor = isValid ? Color.FromArgb(240, 255, 240) : Color.FromArgb(255, 240, 240);
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidPassword(string password)
        {
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {
            UpdatePasswordStrength(txtPassword.Text);
        }

        private void UpdatePasswordStrength(string password)
        {
            int score = 0;
            string strengthText = "";
            Color strengthColor = Color.FromArgb(220, 53, 69);

            if (password.Length >= 8) score++;
            if (Regex.IsMatch(password, @"[a-z]")) score++;
            if (Regex.IsMatch(password, @"[A-Z]")) score++;
            if (Regex.IsMatch(password, @"\d")) score++;
            if (Regex.IsMatch(password, @"[!@#$%^&*]")) score++;

            switch (score)
            {
                case 0:
                case 1:
                    strengthText = "Very Weak";
                    strengthColor = Color.FromArgb(220, 53, 69);
                    progressBarPassword.Value = 20;
                    break;
                case 2:
                    strengthText = "Weak";
                    strengthColor = Color.FromArgb(255, 193, 7);
                    progressBarPassword.Value = 40;
                    break;
                case 3:
                    strengthText = "Fair";
                    strengthColor = Color.FromArgb(255, 193, 7);
                    progressBarPassword.Value = 60;
                    break;
                case 4:
                    strengthText = "Good";
                    strengthColor = Color.FromArgb(40, 167, 69);
                    progressBarPassword.Value = 80;
                    break;
                case 5:
                    strengthText = "Strong";
                    strengthColor = Color.FromArgb(40, 167, 69);
                    progressBarPassword.Value = 100;
                    break;
            }

            lblPasswordStrength.Text = $"Password strength: {strengthText}";
            lblPasswordStrength.ForeColor = strengthColor;
        }

        private void UpdateEyeIcon(PictureBox eyeIconParam, bool visible)
        {
            if (eyeIconParam == null) return;

            try
            {
                var iconPath = Path.Combine(Application.StartupPath, "Assets", "Icons",
                    visible ? "eye-show.png" : "eye-hide.png");

                if (File.Exists(iconPath))
                {
                    eyeIconParam.Image = Image.FromFile(iconPath);
                    return;
                }
            }
            catch { }

            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.FromArgb(100, 100, 100), 2))
                {
                    g.DrawEllipse(pen, 4, 8, 16, 8);
                    g.FillEllipse(new SolidBrush(Color.FromArgb(100, 100, 100)), 10, 10, 4, 4);
                    if (!visible)
                    {
                        g.DrawLine(pen, 2, 2, 22, 22);
                    }
                }
            }
            eyeIconParam.Image = bmp;
        }

        private void TogglePasswordVisibility(bool isConfirm)
        {
            if (isConfirm)
            {
                confirmPasswordVisible = !confirmPasswordVisible;
                if (txtConfirmPassword != null)
                    txtConfirmPassword.PasswordChar = confirmPasswordVisible ? '\0' : '•';
                if (eyeIconConfirm != null)
                    UpdateEyeIcon(eyeIconConfirm, confirmPasswordVisible);
            }
            else
            {
                passwordVisible = !passwordVisible;
                if (txtPassword != null)
                    txtPassword.PasswordChar = passwordVisible ? '\0' : '•';
                if (eyeIcon != null)
                    UpdateEyeIcon(eyeIcon, passwordVisible);
            }
        }

        private void ShowValidationMessage(string message, bool isError = true)
        {
            lblValidationMessage.Text = message;
            lblValidationMessage.ForeColor = isError ? Color.FromArgb(220, 53, 69) : Color.FromArgb(40, 167, 69);
            lblValidationMessage.Visible = true;

            var timer = new Timer { Interval = 5000 };
            timer.Tick += (s, e) => { lblValidationMessage.Visible = false; timer.Stop(); };
            timer.Start();
        }

        private void SetProcessingState(bool processing)
        {
            isProcessing = processing;
            btnCreateAccount.Enabled = !processing;
            btnCreateAccount.Text = processing ? "CREATING ACCOUNT..." : "CREATE ACCOUNT";
            this.Cursor = processing ? Cursors.WaitCursor : Cursors.Default;
        }

        private void AnimateToLoginForm()
        {
            var fadeTimer = new Timer { Interval = 10 };
            int currentOpacity = 100;

            fadeTimer.Tick += (s, e) =>
            {
                currentOpacity -= 5;
                this.Opacity = currentOpacity / 100.0;

                if (currentOpacity <= 0)
                {
                    fadeTimer.Stop();
                    _navigationService.ShowLoginForm();
                }
            };

            fadeTimer.Start();
        }

        private async void BtnCreateAccount_Click(object sender, EventArgs e)
        {
            if (isProcessing) return;

            lblValidationMessage.Visible = false;

            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                ShowValidationMessage("Please enter your first and last name");
                txtFirstName.Focus();
                return;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                ShowValidationMessage("Please enter a valid email address (e.g., user@domain.com)");
                txtEmail.Focus();
                return;
            }

            if (!IsValidPassword(txtPassword.Text))
            {
                ShowValidationMessage("Password must be at least 8 characters with uppercase, lowercase, number, and special character");
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                ShowValidationMessage("Passwords do not match");
                txtConfirmPassword.Focus();
                return;
            }

            SetProcessingState(true);

            try
            {
                if (_userService.CheckUserExists(txtEmail.Text))
                {
                    ShowValidationMessage("An account with this email already exists");
                    txtEmail.Focus();
                    return;
                }

                bool success = _userService.RegisterUser(
                    txtFirstName.Text.Trim(),
                    txtLastName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtPassword.Text);

                if (success)
                {
                    ShowValidationMessage("Account created successfully! Redirecting to login...", false);

                    txtFirstName.Clear();
                    txtLastName.Clear();
                    txtEmail.Clear();
                    txtPassword.Clear();
                    txtConfirmPassword.Clear();

                    await System.Threading.Tasks.Task.Delay(2000);
                    _navigationService.ShowLoginForm();
                }
            }
            catch (Exception ex)
            {
                ShowValidationMessage($"Registration failed: {ex.Message}");
            }
            finally
            {
                SetProcessingState(false);
            }
        }
    }
}