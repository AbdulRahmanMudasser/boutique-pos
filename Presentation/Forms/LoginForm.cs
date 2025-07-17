using MaqboolFashion.Data.Database;
using MaqboolFashion.Presentation.Services;
using MaqboolFashion.Presentation.Components;
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
        private HeaderComponent headerComponent;
        private TextBox txtEmail, txtPassword;
        private PictureBox eyeIcon;
        private CheckBox chkRememberMe;
        private Button btnLogin;
        private Label lblValidationMessage;
        private bool passwordVisible = false;
        private bool isProcessing = false;
        private string rememberedEmail = "";

        public LoginForm()
        {
            _navigationService = NavigationService.Instance(this);

            this.Text = "MaqboolFashion - Sign In";
            this.ClientSize = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.Padding = new Padding(10);
            this.KeyPreview = true;

            this.KeyDown += LoginForm_KeyDown;

            InitializeUI();
            SetupFormValidation();
            LoadRememberedCredentials();
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _navigationService.ExitApplication();
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                _navigationService.ShowSignupForm();
            }
            else if (e.KeyCode == Keys.Enter && !isProcessing)
            {
                BtnLogin_Click(this, EventArgs.Empty);
            }
        }

        private void InitializeUI()
        {
            // Add HeaderComponent
            headerComponent = new HeaderComponent("MAQBOOL FASHION - SIGN IN");
            headerComponent.ExitClicked += (s, e) => _navigationService.ExitApplication();
            headerComponent.MinimizeClicked += (s, e) => this.WindowState = FormWindowState.Minimized;
            this.Controls.Add(headerComponent);

            var mainContainer = new Panel
            {
                BackColor = Color.White,
                Size = new Size(900, 600),
                Location = new Point(50, headerComponent.GetHeaderHeight() + 10),
                BorderStyle = BorderStyle.None
            };

            mainContainer.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, mainContainer.ClientRectangle,
                    Color.FromArgb(230, 230, 230), ButtonBorderStyle.Solid);
            };

            this.Controls.Add(mainContainer);

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
                Size = new Size(500, 550),
                Location = new Point(400, 0),
                BorderStyle = BorderStyle.None,
                AutoScroll = true,
                AutoScrollMinSize = new Size(480, 550)
            };
            mainContainer.Controls.Add(rightPanel);

            LoadEnhancedBrandingSection(leftPanel);
            CreateEnhancedFormElements(rightPanel);
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

                var lblWelcome = new Label
                {
                    Text = "Welcome Back!",
                    Font = new Font("Segoe UI", 14, FontStyle.Regular),
                    ForeColor = Color.FromArgb(220, 220, 220),
                    AutoSize = true,
                    Location = new Point(120, 195)
                };
                brandContent.Controls.Add(lblWelcome);

                var benefits = new[] { "• Access Your Account", "• Track Your Orders", "• Exclusive Offers" };
                for (int i = 0; i < benefits.Length; i++)
                {
                    var lblBenefit = new Label
                    {
                        Text = benefits[i],
                        Font = new Font("Segoe UI", 9),
                        ForeColor = Color.FromArgb(200, 200, 200),
                        AutoSize = true,
                        Location = new Point(70, 230 + (i * 20))
                    };
                    brandContent.Controls.Add(lblBenefit);
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
                Text = "SIGN IN",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true,
                Location = new Point(50, 35)
            };
            panel.Controls.Add(lblWelcome);

            var lblSubtitle = new Label
            {
                Text = "Welcome back to your fashion destination",
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
            var fieldPadding = 30;
            var startY = 150;

            txtEmail = CreateEnhancedField("Email Address *", panel, startY, fieldWidth, fieldHeight);
            txtEmail.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };

            txtPassword = CreateEnhancedField("Password *", panel, startY + fieldHeight + fieldPadding, fieldWidth, fieldHeight);
            eyeIcon = CreatePasswordField("Password *", txtPassword, panel, startY + fieldHeight + fieldPadding, fieldWidth, fieldHeight);
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter && !isProcessing) BtnLogin_Click(this, EventArgs.Empty); };

            chkRememberMe = new CheckBox
            {
                Text = "Remember me",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 2 + 10),
                Cursor = Cursors.Hand
            };
            panel.Controls.Add(chkRememberMe);

            var linkForgotPassword = new LinkLabel
            {
                Text = "Forgot Password?",
                Font = new Font("Segoe UI", 9),
                LinkColor = Color.FromArgb(40, 40, 40),
                ActiveLinkColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Location = new Point(320, startY + (fieldHeight + fieldPadding) * 2 + 12),
                Cursor = Cursors.Hand
            };
            linkForgotPassword.LinkBehavior = LinkBehavior.HoverUnderline;
            linkForgotPassword.Click += (s, e) =>
            {
                MessageBox.Show("Password reset functionality coming soon!", "Feature Coming Soon",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            panel.Controls.Add(linkForgotPassword);

            btnLogin = new Button
            {
                Text = "SIGN IN",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(fieldWidth, 50),
                Location = new Point(50, startY + (fieldHeight + fieldPadding) * 2 + 50),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);
            btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 30, 30);
            btnLogin.Click += BtnLogin_Click;
            panel.Controls.Add(btnLogin);

            var lblDivider = new Label
            {
                Text = "────────────── OR ──────────────",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(180, 180, 180),
                AutoSize = true,
                Location = new Point(150, startY + (fieldHeight + fieldPadding) * 2 + 120)
            };
            panel.Controls.Add(lblDivider);

            var lblNoAccount = new Label
            {
                Text = "Don't have an account?",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(120, startY + (fieldHeight + fieldPadding) * 2 + 150)
            };
            panel.Controls.Add(lblNoAccount);

            var linkSignUp = new LinkLabel
            {
                Text = "SIGN UP",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                LinkColor = Color.FromArgb(40, 40, 40),
                ActiveLinkColor = Color.FromArgb(60, 60, 60),
                AutoSize = true,
                Location = new Point(280, startY + (fieldHeight + fieldPadding) * 2 + 150),
                Cursor = Cursors.Hand
            };
            linkSignUp.LinkBehavior = LinkBehavior.HoverUnderline;
            linkSignUp.Click += (s, e) => AnimateToSignupForm();
            panel.Controls.Add(linkSignUp);
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

        private PictureBox CreatePasswordField(string labelText, TextBox textBox, Panel panel, int y, int width, int height)
        {
            if (textBox == null) return null;

            textBox.PasswordChar = '•';

            var eyeIconOut = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(410, y + 10),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };

            panel.Controls.Add(eyeIconOut);

            UpdateEyeIcon();
            eyeIconOut.Click += (s, e) => TogglePasswordVisibility();

            return eyeIconOut;
        }

        private void SetupFormValidation()
        {
            txtEmail.TextChanged += (s, e) => ValidateField(txtEmail, IsValidEmail(txtEmail.Text));
            txtPassword.TextChanged += (s, e) => ValidateField(txtPassword, !string.IsNullOrWhiteSpace(txtPassword.Text));
        }

        private void ValidateField(TextBox textBox, bool isValid)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.BackColor = Color.FromArgb(250, 250, 250);
            }
            else
            {
                textBox.BackColor = isValid ? Color.FromArgb(240, 255, 240) : Color.FromArgb(255, 240, 240);
            }
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, emailPattern);
        }

        private void UpdateEyeIcon()
        {
            if (eyeIcon == null) return;

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

            var bmp = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.FromArgb(100, 100, 100), 2))
                {
                    g.DrawEllipse(pen, 4, 8, 16, 8);
                    g.FillEllipse(new SolidBrush(Color.FromArgb(100, 100, 100)), 10, 10, 4, 4);
                    if (!passwordVisible)
                    {
                        g.DrawLine(pen, 2, 2, 22, 22);
                    }
                }
            }
            eyeIcon.Image = bmp;
        }

        private void TogglePasswordVisibility()
        {
            passwordVisible = !passwordVisible;
            if (txtPassword != null)
                txtPassword.PasswordChar = passwordVisible ? '\0' : '•';
            UpdateEyeIcon();
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
            btnLogin.Enabled = !processing;
            btnLogin.Text = processing ? "SIGNING IN..." : "SIGN IN";
            txtEmail.Enabled = !processing;
            txtPassword.Enabled = !processing;
            chkRememberMe.Enabled = !processing;
            this.Cursor = processing ? Cursors.WaitCursor : Cursors.Default;
        }

        private void AnimateToSignupForm()
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
                    _navigationService.ShowSignupForm();
                }
            };

            fadeTimer.Start();
        }

        private void LoadRememberedCredentials()
        {
            try
            {
                if (!string.IsNullOrEmpty(rememberedEmail))
                {
                    txtEmail.Text = rememberedEmail;
                    chkRememberMe.Checked = true;
                    txtPassword.Focus();
                }
                else
                {
                    txtEmail.Focus();
                }
            }
            catch
            {
                txtEmail.Focus();
            }
        }

        private void SaveRememberedCredentials()
        {
            try
            {
                if (chkRememberMe.Checked)
                {
                    rememberedEmail = txtEmail.Text;
                }
                else
                {
                    rememberedEmail = "";
                }
            }
            catch
            {
            }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (isProcessing) return;

            lblValidationMessage.Visible = false;

            if (!IsValidEmail(txtEmail.Text))
            {
                ShowValidationMessage("Please enter a valid email address");
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowValidationMessage("Please enter your password");
                txtPassword.Focus();
                return;
            }

            SetProcessingState(true);

            try
            {
                await System.Threading.Tasks.Task.Delay(500);

                bool success = _userService.LoginUser(txtEmail.Text.Trim(), txtPassword.Text);

                if (success)
                {
                    SaveRememberedCredentials();

                    ShowValidationMessage("Login successful! Welcome back.", false);

                    await System.Threading.Tasks.Task.Delay(1000);

                    _navigationService.ShowDashboardForm();
                }
                else
                {
                    ShowValidationMessage("Invalid email or password. Please try again.");
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowValidationMessage($"Login failed: {ex.Message}");
            }
            finally
            {
                SetProcessingState(false);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                eyeIcon?.Image?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}