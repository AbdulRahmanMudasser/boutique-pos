using MaqboolFashion.Data.Database;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Forms
{
    public class LaborFormDialog : Form
    {
        private TextBox txtName;
        private TextBox txtArea;
        private TextBox txtCity;
        private TextBox txtPhoneNumber;
        private TextBox txtCNIC;
        private TextBox txtCaste;
        private NumericUpDown numCost;
        private DateTimePicker dtpJoiningDate;
        private PictureBox picProfile;
        private Button btnUploadImage;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;
        private LaborService laborService;
        private LaborService.Labor editingLabor;
        private bool isEditMode;
        private string selectedImagePath;

        public bool IsSuccess { get; private set; }

        public LaborFormDialog(LaborService.Labor labor = null)
        {
            laborService = new LaborService();
            editingLabor = labor;
            isEditMode = labor != null;

            InitializeDialog();
            CreateFormControls();

            if (isEditMode)
            {
                LoadLaborData();
            }
        }

        private void InitializeDialog()
        {
            this.Text = isEditMode ? "Edit Worker" : "Add Worker";
            this.Size = new Size(600, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);
        }

        private void CreateFormControls()
        {
            lblTitle = new Label
            {
                Text = isEditMode ? "Edit Worker Details" : "Add New Worker",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            // Profile Image Section
            var profileLabel = new Label
            {
                Text = "Profile Picture",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 60)
            };

            picProfile = new PictureBox
            {
                Size = new Size(120, 120),
                Location = new Point(30, 85),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Set default image
            try
            {
                // Create a simple default profile image
                var defaultBitmap = new Bitmap(120, 120);
                using (Graphics g = Graphics.FromImage(defaultBitmap))
                {
                    g.Clear(Color.FromArgb(240, 240, 240));
                    using (var brush = new SolidBrush(Color.Gray))
                    {
                        var font = new Font("Segoe UI", 12);
                        var text = "👤";
                        var size = g.MeasureString(text, font);
                        g.DrawString(text, font, brush,
                            (120 - size.Width) / 2, (120 - size.Height) / 2);
                    }
                }
                picProfile.Image = defaultBitmap;
            }
            catch { }

            btnUploadImage = new Button
            {
                Text = "Upload Photo",
                Font = new Font("Segoe UI", 9),
                Size = new Size(120, 30),
                Location = new Point(30, 215),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnUploadImage.FlatAppearance.BorderSize = 0;
            btnUploadImage.Click += BtnUploadImage_Click;

            // Form Fields - Left Column
            var lblName = new Label
            {
                Text = "Full Name *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(180, 85)
            };

            txtName = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(350, 30),
                Location = new Point(180, 110),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            var lblArea = new Label
            {
                Text = "Area *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(180, 150)
            };

            txtArea = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(170, 30),
                Location = new Point(180, 175),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            var lblCity = new Label
            {
                Text = "City *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(360, 150)
            };

            txtCity = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(170, 30),
                Location = new Point(360, 175),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            var lblPhone = new Label
            {
                Text = "Phone Number *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 270)
            };

            txtPhoneNumber = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(250, 30),
                Location = new Point(30, 295),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black,
                MaxLength = 11
            };

            var lblCNIC = new Label
            {
                Text = "CNIC Number * (13 digits)",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(290, 270)
            };

            txtCNIC = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(240, 30),
                Location = new Point(290, 295),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black,
                MaxLength = 13
            };

            var lblCaste = new Label
            {
                Text = "Caste",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 335)
            };

            txtCaste = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(250, 30),
                Location = new Point(30, 360),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            var lblCost = new Label
            {
                Text = "Monthly/Daily Cost (Rs.) *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(290, 335)
            };

            numCost = new NumericUpDown
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(240, 30),
                Location = new Point(290, 360),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Maximum = 999999,
                Minimum = 0,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };

            var lblJoiningDate = new Label
            {
                Text = "Joining Date *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 400)
            };

            dtpJoiningDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(250, 30),
                Location = new Point(30, 425),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(120, 45),
                Location = new Point(290, 600),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = Color.Black;
            btnCancel.FlatAppearance.BorderSize = 2;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            btnCancel.Click += BtnCancel_Click;

            btnSave = new Button
            {
                Text = isEditMode ? "Update Worker" : "Save Worker",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(140, 45),
                Location = new Point(420, 600),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 64, 64);
            btnSave.Click += BtnSave_Click;

            // Add validation to CNIC and Phone fields
            txtCNIC.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            };

            txtPhoneNumber.KeyPress += (s, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            };

            // Add all controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(profileLabel);
            this.Controls.Add(picProfile);
            this.Controls.Add(btnUploadImage);
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblArea);
            this.Controls.Add(txtArea);
            this.Controls.Add(lblCity);
            this.Controls.Add(txtCity);
            this.Controls.Add(lblPhone);
            this.Controls.Add(txtPhoneNumber);
            this.Controls.Add(lblCNIC);
            this.Controls.Add(txtCNIC);
            this.Controls.Add(lblCaste);
            this.Controls.Add(txtCaste);
            this.Controls.Add(lblCost);
            this.Controls.Add(numCost);
            this.Controls.Add(lblJoiningDate);
            this.Controls.Add(dtpJoiningDate);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnSave);

            // Tab order and enter key navigation
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    BtnCancel_Click(this, EventArgs.Empty);
                }
                else if (e.KeyCode == Keys.F1)
                {
                    BtnSave_Click(this, EventArgs.Empty);
                }
            };
        }

        private void BtnUploadImage_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Select Profile Picture";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        selectedImagePath = openFileDialog.FileName;
                        picProfile.Image = Image.FromFile(selectedImagePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadLaborData()
        {
            if (editingLabor != null)
            {
                txtName.Text = editingLabor.Name;
                txtArea.Text = editingLabor.Area;
                txtCity.Text = editingLabor.City;
                txtPhoneNumber.Text = editingLabor.PhoneNumber;
                txtCNIC.Text = editingLabor.CNIC;
                txtCaste.Text = editingLabor.Caste;
                numCost.Value = editingLabor.Cost;
                dtpJoiningDate.Value = editingLabor.JoiningDate;

                // Load existing profile image if available
                if (!string.IsNullOrEmpty(editingLabor.ProfileImagePath) && File.Exists(editingLabor.ProfileImagePath))
                {
                    try
                    {
                        picProfile.Image = Image.FromFile(editingLabor.ProfileImagePath);
                        selectedImagePath = editingLabor.ProfileImagePath;
                    }
                    catch { }
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                var name = txtName.Text.Trim();
                var area = txtArea.Text.Trim();
                var city = txtCity.Text.Trim();
                var phoneNumber = txtPhoneNumber.Text.Trim();
                var cnic = txtCNIC.Text.Trim();
                var caste = txtCaste.Text.Trim();
                var cost = numCost.Value;
                var joiningDate = dtpJoiningDate.Value;

                // Check if CNIC already exists
                if (laborService.CNICExists(cnic, isEditMode ? editingLabor.Id : (int?)null))
                {
                    MessageBox.Show("A worker with this CNIC already exists.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCNIC.Focus();
                    return;
                }

                string imagePath = null;

                // Handle profile image
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    try
                    {
                        var imageBytes = File.ReadAllBytes(selectedImagePath);
                        imagePath = laborService.SaveProfileImage(name, imageBytes); // Fixed parameter order
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving profile image: {ex.Message}", "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                bool success;
                if (isEditMode)
                {
                    success = laborService.UpdateLabor(editingLabor.Id, name, area, city, phoneNumber,
                                                     cnic, caste, cost, joiningDate, imagePath);
                }
                else
                {
                    success = laborService.AddLabor(name, area, city, phoneNumber, cnic, caste,
                                                  cost, joiningDate, imagePath);
                }

                if (success)
                {
                    IsSuccess = true;
                    var message = isEditMode ? "Worker updated successfully!" : "Worker added successfully!";
                    MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to save worker. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving worker: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            // Name validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter worker's full name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (txtName.Text.Trim().Length < 2)
            {
                MessageBox.Show("Name must be at least 2 characters long.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            // Area validation
            if (string.IsNullOrWhiteSpace(txtArea.Text))
            {
                MessageBox.Show("Please enter worker's area.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtArea.Focus();
                return false;
            }

            // City validation
            if (string.IsNullOrWhiteSpace(txtCity.Text))
            {
                MessageBox.Show("Please enter worker's city.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCity.Focus();
                return false;
            }

            // Phone validation
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Please enter phone number.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhoneNumber.Focus();
                return false;
            }

            if (txtPhoneNumber.Text.Length != 11)
            {
                MessageBox.Show("Phone number must be exactly 11 digits.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhoneNumber.Focus();
                return false;
            }

            // CNIC validation
            if (string.IsNullOrWhiteSpace(txtCNIC.Text))
            {
                MessageBox.Show("Please enter CNIC number.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCNIC.Focus();
                return false;
            }

            if (txtCNIC.Text.Length != 13)
            {
                MessageBox.Show("CNIC must be exactly 13 digits.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCNIC.Focus();
                return false;
            }

            // Cost validation
            if (numCost.Value <= 0)
            {
                MessageBox.Show("Please enter a valid cost amount.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numCost.Focus();
                return false;
            }

            // Joining date validation
            if (dtpJoiningDate.Value > DateTime.Now)
            {
                MessageBox.Show("Joining date cannot be in the future.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpJoiningDate.Focus();
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Clean up image resources
            if (picProfile.Image != null)
            {
                picProfile.Image.Dispose();
            }
            base.OnFormClosed(e);
        }
    }
}