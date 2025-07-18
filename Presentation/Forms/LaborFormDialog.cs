using MaqboolFashion.Data.Database;
using System;
using System.Drawing;
using System.IO;
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
        private NumericUpDown numCost;
        private DateTimePicker dtpJoiningDate;
        private PictureBox picProfile;
        private Button btnBrowseImage;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;
        private LaborService laborService;
        private LaborService.Labor editingLabor;
        private bool isEditMode;
        private string selectedImagePath = "";

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
            this.Text = isEditMode ? "Edit Labor" : "Add Labor";
            this.Size = new Size(600, 620);
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
                Text = isEditMode ? "Edit Labor Information" : "Add New Labor",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            // Left Column
            var lblName = new Label
            {
                Text = "Full Name *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 70)
            };

            txtName = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(200, 30),
                Location = new Point(30, 95),
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
                Location = new Point(30, 140)
            };

            txtArea = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(200, 30),
                Location = new Point(30, 165),
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
                Location = new Point(30, 210)
            };

            txtCity = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(200, 30),
                Location = new Point(30, 235),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            var lblPhoneNumber = new Label
            {
                Text = "Phone Number *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 280)
            };

            txtPhoneNumber = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(200, 30),
                Location = new Point(30, 305),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black,
            };

            var lblCNIC = new Label
            {
                Text = "CNIC Number *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 350)
            };

            txtCNIC = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(200, 30),
                Location = new Point(30, 375),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black,
            };

            var lblCost = new Label
            {
                Text = "Daily Cost (Rs.) *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 420)
            };

            numCost = new NumericUpDown
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(200, 30),
                Location = new Point(30, 445),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };

            var lblJoiningDate = new Label
            {
                Text = "Joining Date *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 490)
            };

            dtpJoiningDate = new DateTimePicker
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(200, 30),
                Location = new Point(30, 515),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Right Column - Profile Picture
            var lblProfilePicture = new Label
            {
                Text = "Profile Picture",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(300, 70)
            };

            picProfile = new PictureBox
            {
                Size = new Size(200, 200),
                Location = new Point(300, 95),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Default profile icon
            picProfile.Paint += (s, e) =>
            {
                if (picProfile.Image == null)
                {
                    var rect = picProfile.ClientRectangle;
                    using (var brush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                    {
                        var font = new Font("Segoe UI", 48);
                        var text = "👤";
                        var size = e.Graphics.MeasureString(text, font);
                        var x = (rect.Width - size.Width) / 2;
                        var y = (rect.Height - size.Height) / 2;
                        e.Graphics.DrawString(text, font, brush, x, y);
                    }
                }
            };

            btnBrowseImage = new Button
            {
                Text = "📁 Browse Image",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(200, 35),
                Location = new Point(300, 310),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Cursor = Cursors.Hand
            };
            btnBrowseImage.FlatAppearance.BorderColor = Color.Black;
            btnBrowseImage.FlatAppearance.BorderSize = 1;
            btnBrowseImage.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            btnBrowseImage.Click += BtnBrowseImage_Click;

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(100, 40),
                Location = new Point(380, 560),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = Color.Black;
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            btnCancel.Click += BtnCancel_Click;

            btnSave = new Button
            {
                Text = isEditMode ? "Update" : "Save",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(100, 40),
                Location = new Point(490, 560),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 64, 64);
            btnSave.Click += BtnSave_Click;

            // Add all controls to form
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblArea);
            this.Controls.Add(txtArea);
            this.Controls.Add(lblCity);
            this.Controls.Add(txtCity);
            this.Controls.Add(lblPhoneNumber);
            this.Controls.Add(txtPhoneNumber);
            this.Controls.Add(lblCNIC);
            this.Controls.Add(txtCNIC);
            this.Controls.Add(lblCost);
            this.Controls.Add(numCost);
            this.Controls.Add(lblJoiningDate);
            this.Controls.Add(dtpJoiningDate);
            this.Controls.Add(lblProfilePicture);
            this.Controls.Add(picProfile);
            this.Controls.Add(btnBrowseImage);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnSave);

            // Event handlers for validation feedback
            txtPhoneNumber.TextChanged += (s, e) => ValidatePhoneNumber();
            txtCNIC.TextChanged += (s, e) => ValidateCNIC();

            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    BtnCancel_Click(this, EventArgs.Empty);
                }
            };

            this.KeyPreview = true;
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
                numCost.Value = editingLabor.Cost;
                dtpJoiningDate.Value = editingLabor.JoiningDate;
                selectedImagePath = editingLabor.ProfileImagePath;

                // Load profile image if exists
                if (!string.IsNullOrEmpty(editingLabor.ProfileImagePath) && File.Exists(editingLabor.ProfileImagePath))
                {
                    try
                    {
                        picProfile.Image = Image.FromFile(editingLabor.ProfileImagePath);
                    }
                    catch
                    {
                        // Image file not found or corrupted
                        picProfile.Image = null;
                    }
                }
            }
        }

        private void BtnBrowseImage_Click(object sender, EventArgs e)
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

        private void ValidatePhoneNumber()
        {
            var phoneNumber = txtPhoneNumber.Text.Trim();
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                if (LaborService.IsValidPhoneNumber(phoneNumber))
                {
                    txtPhoneNumber.BackColor = Color.White;
                }
                else
                {
                    txtPhoneNumber.BackColor = Color.FromArgb(255, 240, 240);
                }
            }
            else
            {
                txtPhoneNumber.BackColor = Color.White;
            }
        }

        private void ValidateCNIC()
        {
            var cnic = txtCNIC.Text.Trim();
            if (!string.IsNullOrEmpty(cnic))
            {
                if (LaborService.IsValidCNIC(cnic))
                {
                    txtCNIC.BackColor = Color.White;
                }
                else
                {
                    txtCNIC.BackColor = Color.FromArgb(255, 240, 240);
                }
            }
            else
            {
                txtCNIC.BackColor = Color.White;
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
                var cost = numCost.Value;
                var joiningDate = dtpJoiningDate.Value;

                // Check if CNIC already exists
                if (laborService.CNICExists(cnic, isEditMode ? editingLabor.Id : (int?)null))
                {
                    MessageBox.Show("A labor with this CNIC already exists.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCNIC.Focus();
                    return;
                }

                // Check if phone number already exists
                if (laborService.PhoneNumberExists(phoneNumber, isEditMode ? editingLabor.Id : (int?)null))
                {
                    MessageBox.Show("A labor with this phone number already exists.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPhoneNumber.Focus();
                    return;
                }

                // Copy image to profile pictures folder if a new image was selected
                string finalImagePath = "";
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    finalImagePath = CopyImageToProfileFolder(selectedImagePath, name);
                }
                else if (isEditMode)
                {
                    finalImagePath = editingLabor.ProfileImagePath;
                }

                bool success;
                if (isEditMode)
                {
                    success = laborService.UpdateLabor(editingLabor.Id, name, area, city, phoneNumber, cnic, cost, joiningDate, finalImagePath);
                }
                else
                {
                    success = laborService.AddLabor(name, area, city, phoneNumber, cnic, cost, joiningDate, finalImagePath);
                }

                if (success)
                {
                    IsSuccess = true;
                    var message = isEditMode ? "Labor updated successfully!" : "Labor added successfully!";
                    MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to save labor. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving labor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CopyImageToProfileFolder(string sourcePath, string laborName)
        {
            try
            {
                // Create profile pictures directory if it doesn't exist
                var profileDir = Path.Combine(Application.StartupPath, "ProfilePictures");
                if (!Directory.Exists(profileDir))
                {
                    Directory.CreateDirectory(profileDir);
                }

                // Create unique filename
                var extension = Path.GetExtension(sourcePath);
                var fileName = $"{laborName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                var destinationPath = Path.Combine(profileDir, fileName);

                // Copy file
                File.Copy(sourcePath, destinationPath, true);
                return destinationPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying profile picture: {ex.Message}", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return "";
            }
        }

        private bool ValidateInput()
        {
            // Name validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter labor name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (txtName.Text.Trim().Length < 2)
            {
                MessageBox.Show("Labor name must be at least 2 characters long.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            // Area validation
            if (string.IsNullOrWhiteSpace(txtArea.Text))
            {
                MessageBox.Show("Please enter area.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtArea.Focus();
                return false;
            }

            // City validation
            if (string.IsNullOrWhiteSpace(txtCity.Text))
            {
                MessageBox.Show("Please enter city.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCity.Focus();
                return false;
            }

            // Phone number validation
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Please enter phone number.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhoneNumber.Focus();
                return false;
            }

            if (!LaborService.IsValidPhoneNumber(txtPhoneNumber.Text.Trim()))
            {
                MessageBox.Show("Please enter a valid phone number (10-11 digits).", "Validation Error",
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

            if (!LaborService.IsValidCNIC(txtCNIC.Text.Trim()))
            {
                MessageBox.Show("Please enter a valid CNIC number (13 digits).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCNIC.Focus();
                return false;
            }

            // Cost validation
            if (numCost.Value <= 0)
            {
                MessageBox.Show("Please enter a valid daily cost.", "Validation Error",
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up any resources
            if (picProfile.Image != null)
            {
                picProfile.Image.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}