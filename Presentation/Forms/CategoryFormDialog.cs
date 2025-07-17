using MaqboolFashion.Data.Database;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Forms
{
    public class CategoryFormDialog : Form
    {
        private TextBox txtName;
        private TextBox txtDescription;
        private Button btnSave;
        private Button btnCancel;
        private Label lblTitle;
        private CategoryService categoryService;
        private CategoryService.Category editingCategory;
        private bool isEditMode;

        public bool IsSuccess { get; private set; }

        public CategoryFormDialog(CategoryService.Category category = null)
        {
            categoryService = new CategoryService();
            editingCategory = category;
            isEditMode = category != null;

            InitializeDialog();
            CreateFormControls();

            if (isEditMode)
            {
                LoadCategoryData();
            }
        }

        private void InitializeDialog()
        {
            this.Text = isEditMode ? "Edit Category" : "Add Category";
            this.Size = new Size(520, 370);
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
                Text = isEditMode ? "Edit Category" : "Add New Category",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 30)
            };

            var lblName = new Label
            {
                Text = "Category Name *",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 80)
            };

            txtName = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(440, 35),
                Location = new Point(30, 105),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            var lblDescription = new Label
            {
                Text = "Description",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(30, 160)
            };

            txtDescription = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(440, 60),
                Location = new Point(30, 185),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(100, 40),
                Location = new Point(270, 285),
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
                Location = new Point(380, 285),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(64, 64, 64);
            btnSave.Click += BtnSave_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnSave);

            txtName.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                }
            };

            txtDescription.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && e.Control)
                {
                    BtnSave_Click(this, EventArgs.Empty);
                }
            };

            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    BtnCancel_Click(this, EventArgs.Empty);
                }
            };

            this.KeyPreview = true;
        }

        private void LoadCategoryData()
        {
            if (editingCategory != null)
            {
                txtName.Text = editingCategory.Name;
                txtDescription.Text = editingCategory.Description;
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
                var description = txtDescription.Text.Trim();

                if (categoryService.CategoryNameExists(name, isEditMode ? editingCategory.Id : (int?)null))
                {
                    MessageBox.Show("A category with this name already exists.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                bool success;
                if (isEditMode)
                {
                    success = categoryService.UpdateCategory(editingCategory.Id, name, description);
                }
                else
                {
                    success = categoryService.AddCategory(name, description);
                }

                if (success)
                {
                    IsSuccess = true;
                    var message = isEditMode ? "Category updated successfully!" : "Category added successfully!";
                    MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to save category. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a category name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (txtName.Text.Trim().Length < 2)
            {
                MessageBox.Show("Category name must be at least 2 characters long.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (txtName.Text.Trim().Length > 100)
            {
                MessageBox.Show("Category name cannot exceed 100 characters.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CategoryFormDialog
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "CategoryFormDialog";
            this.Load += new System.EventHandler(this.CategoryFormDialog_Load);
            this.ResumeLayout(false);

        }

        private void CategoryFormDialog_Load(object sender, EventArgs e)
        {

        }
    }
}