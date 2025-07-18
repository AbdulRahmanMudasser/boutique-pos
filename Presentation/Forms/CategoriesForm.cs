using MaqboolFashion.Data.Database;
using MaqboolFashion.Presentation.Components;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Forms
{
    public class CategoriesForm : BaseFormComponent
    {
        private CategoryService categoryService;
        private DataGridView categoriesGrid;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnRefresh;
        private PaginationComponent pagination;
        private Panel searchPanel;
        private Panel actionPanel;
        private Label totalCategoriesLabel;
        private Timer searchTimer;

        public CategoriesForm() : base("CATEGORIES - MAQBOOL FASHION")
        {
            this.Text = "MaqboolFashion - Categories";
            categoryService = new CategoryService();
            SetActiveMenu("Categories");
            LoadPageContent();
        }

        protected override void CreatePageContent()
        {
            var contentPanel = GetContentPanel();

            CreateHeaderSection(contentPanel);
            CreateSearchAndActionPanel(contentPanel);
            CreateDataGrid(contentPanel);
            CreatePagination(contentPanel);

            LoadCategories();
        }

        private void CreateHeaderSection(Panel contentPanel)
        {
            var headerContainer = new Panel
            {
                Size = new Size(contentPanel.Width - 60, 100),
                Location = new Point(0, 0),
                BackColor = Color.White
            };

            var headerLabel = new Label
            {
                Text = "Categories Management",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 10)
            };

            var subtitleLabel = new Label
            {
                Text = "Organize and manage your product categories efficiently",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(0, 50)
            };

            totalCategoriesLabel = new Label
            {
                Text = "Total Categories: 0",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 75)
            };

            var separatorLine = new Panel
            {
                Height = 2,
                Width = headerContainer.Width,
                Location = new Point(0, 95),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            headerContainer.Controls.Add(headerLabel);
            headerContainer.Controls.Add(subtitleLabel);
            headerContainer.Controls.Add(totalCategoriesLabel);
            headerContainer.Controls.Add(separatorLine);

            contentPanel.Controls.Add(headerContainer);
        }

        private void CreateSearchAndActionPanel(Panel contentPanel)
        {
            searchPanel = new Panel
            {
                Size = new Size(contentPanel.Width - 60, 100),
                Location = new Point(0, 120),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            searchPanel.Paint += (s, e) =>
            {
                var rect = searchPanel.ClientRectangle;
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            // Main container for better layout control
            var mainContainer = new Panel
            {
                Size = new Size(searchPanel.Width - 40, 80),
                Location = new Point(20, 10),
                BackColor = Color.Transparent
            };

            // Left section - Search
            var searchSection = new Panel
            {
                Size = new Size(450, 70),
                Location = new Point(0, 5),
                BackColor = Color.Transparent
            };

            var lblSearch = new Label
            {
                Text = "🔍 Search Categories",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(5, 0)
            };

            var searchInputContainer = new Panel
            {
                Size = new Size(400, 45),
                Location = new Point(5, 25),
                BackColor = Color.White
            };

            searchInputContainer.Paint += (s, e) =>
            {
                var rect = searchInputContainer.ClientRectangle;
                using (var pen = new Pen(Color.FromArgb(200, 200, 200), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 12),
                Size = new Size(390, 35),
                Location = new Point(5, 5),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = Color.Black,
            };

            txtSearch.Enter += (s, e) =>
            {
                searchInputContainer.BackColor = Color.FromArgb(250, 250, 255);
                searchInputContainer.Invalidate();
            };
            txtSearch.Leave += (s, e) =>
            {
                searchInputContainer.BackColor = Color.White;
                searchInputContainer.Invalidate();
            };

            searchTimer = new Timer { Interval = 500 };
            searchTimer.Tick += (s, e) =>
            {
                searchTimer.Stop();
                LoadCategories();
            };

            txtSearch.TextChanged += (s, e) =>
            {
                searchTimer.Stop();
                searchTimer.Start();
            };

            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    searchTimer.Stop();
                    LoadCategories();
                }
            };

            searchInputContainer.Controls.Add(txtSearch);
            searchSection.Controls.Add(lblSearch);
            searchSection.Controls.Add(searchInputContainer);

            // Right section - Actions
            actionPanel = new Panel
            {
                Size = new Size(300, 70),
                Location = new Point(mainContainer.Width - 300, 5),
                BackColor = Color.Transparent
            };

            var lblActions = new Label
            {
                Text = "Actions",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(5, 0)
            };

            var buttonContainer = new Panel
            {
                Size = new Size(290, 45),
                Location = new Point(5, 25),
                BackColor = Color.Transparent
            };

            btnAdd = new Button
            {
                Text = "➕ Add Category",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(140, 40),
                Location = new Point(0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 40);
            btnAdd.FlatAppearance.MouseDownBackColor = Color.FromArgb(20, 20, 20);
            btnAdd.Click += BtnAdd_Click;

            // Add subtle shadow effect to Add button
            btnAdd.Paint += (s, e) =>
            {
                var rect = btnAdd.ClientRectangle;

                // Draw shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                }

                // Draw main button
                using (var brush = new SolidBrush(btnAdd.BackColor))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                // Draw text
                using (var brush = new SolidBrush(btnAdd.ForeColor))
                {
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(btnAdd.Text, btnAdd.Font, brush, rect, stringFormat);
                }
            };

            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(120, 40),
                Location = new Point(150, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.Black;
            btnRefresh.FlatAppearance.BorderSize = 2;
            btnRefresh.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            btnRefresh.FlatAppearance.MouseDownBackColor = Color.FromArgb(220, 220, 220);
            btnRefresh.Click += (s, e) => LoadCategories();

            // Add hover effects
            btnRefresh.MouseEnter += (s, e) =>
            {
                btnRefresh.BackColor = Color.FromArgb(240, 240, 240);
            };
            btnRefresh.MouseLeave += (s, e) =>
            {
                btnRefresh.BackColor = Color.White;
            };

            buttonContainer.Controls.Add(btnAdd);
            buttonContainer.Controls.Add(btnRefresh);

            actionPanel.Controls.Add(lblActions);
            actionPanel.Controls.Add(buttonContainer);

            mainContainer.Controls.Add(searchSection);
            mainContainer.Controls.Add(actionPanel);
            searchPanel.Controls.Add(mainContainer);

            contentPanel.Controls.Add(searchPanel);

            // Enhanced resize handling
            contentPanel.Resize += (s, e) =>
            {
                if (searchPanel != null && mainContainer != null && actionPanel != null)
                {
                    searchPanel.Width = contentPanel.Width - 60;
                    mainContainer.Width = searchPanel.Width - 40;
                    actionPanel.Location = new Point(mainContainer.Width - 300, 5);
                }
            };
        }

        private void CreateDataGrid(Panel contentPanel)
        {
            var gridContainer = new Panel
            {
                Location = new Point(0, 240),
                Size = new Size(contentPanel.Width - 60, 400),
                BackColor = Color.White
            };

            gridContainer.Paint += (s, e) =>
            {
                var rect = gridContainer.ClientRectangle;
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            categoriesGrid = new DataGridView
            {
                Location = new Point(2, 2),
                Size = new Size(gridContainer.Width - 4, gridContainer.Height - 4),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 50,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 11),
                GridColor = Color.FromArgb(240, 240, 240),
                RowTemplate = { Height = 45 },
                EnableHeadersVisualStyles = false
            };

            // Header style
            categoriesGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(15, 10, 15, 10),
                SelectionBackColor = Color.Black,
                SelectionForeColor = Color.White
            };

            // Default cell style - applies to ALL cells
            categoriesGrid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                SelectionBackColor = Color.FromArgb(230, 230, 230),
                SelectionForeColor = Color.Black,
                Font = new Font("Segoe UI", 11),
                Padding = new Padding(15, 8, 15, 8),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            // Remove alternating row style completely
            categoriesGrid.AlternatingRowsDefaultCellStyle = categoriesGrid.DefaultCellStyle;

            // Add columns with consistent styling
            categoriesGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Visible = false
            });

            categoriesGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Category Name",
                DataPropertyName = "Name",
                FillWeight = 30,
                MinimumWidth = 150
            });

            categoriesGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Description",
                HeaderText = "Description",
                DataPropertyName = "Description",
                FillWeight = 40,
                MinimumWidth = 200
            });

            categoriesGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CreatedDate",
                HeaderText = "Created Date",
                DataPropertyName = "CreatedDate",
                FillWeight = 20,
                MinimumWidth = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy",
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    SelectionBackColor = Color.FromArgb(230, 230, 230),
                    SelectionForeColor = Color.Black,
                    Font = new Font("Segoe UI", 11),
                    Padding = new Padding(15, 8, 15, 8)
                }
            });

            var editColumn = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "Actions",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                FillWeight = 10,
                MinimumWidth = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    SelectionBackColor = Color.Black,
                    SelectionForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5)
                }
            };

            var deleteColumn = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                FillWeight = 10,
                MinimumWidth = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    SelectionBackColor = Color.Black,
                    SelectionForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5)
                }
            };

            categoriesGrid.Columns.Add(editColumn);
            categoriesGrid.Columns.Add(deleteColumn);

            // Ensure all columns inherit the same base style
            foreach (DataGridViewColumn column in categoriesGrid.Columns)
            {
                if (column.Name != "Edit" && column.Name != "Delete" && column.Name != "CreatedDate")
                {
                    column.DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.White,
                        ForeColor = Color.Black,
                        SelectionBackColor = Color.FromArgb(230, 230, 230),
                        SelectionForeColor = Color.Black,
                        Font = new Font("Segoe UI", 11),
                        Padding = new Padding(15, 8, 15, 8),
                        Alignment = DataGridViewContentAlignment.MiddleLeft
                    };
                }
            }

            categoriesGrid.CellClick += CategoriesGrid_CellClick;

            // Force consistent row styling
            categoriesGrid.RowsAdded += (s, e) =>
            {
                for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                {
                    if (i < categoriesGrid.Rows.Count)
                    {
                        var row = categoriesGrid.Rows[i];
                        row.DefaultCellStyle = new DataGridViewCellStyle
                        {
                            BackColor = Color.White,
                            ForeColor = Color.Black,
                            SelectionBackColor = Color.FromArgb(230, 230, 230),
                            SelectionForeColor = Color.Black,
                            Font = new Font("Segoe UI", 11),
                            Padding = new Padding(15, 8, 15, 8),
                            Alignment = DataGridViewContentAlignment.MiddleLeft
                        };
                    }
                }
            };

            gridContainer.Controls.Add(categoriesGrid);
            contentPanel.Controls.Add(gridContainer);

            contentPanel.Resize += (s, e) =>
            {
                if (gridContainer != null && categoriesGrid != null)
                {
                    gridContainer.Width = contentPanel.Width - 60;
                    categoriesGrid.Width = gridContainer.Width - 4;
                }
            };
        }

        private void CreatePagination(Panel contentPanel)
        {
            pagination = new PaginationComponent();
            pagination.PageChanged += (s, page) => LoadCategories();
            contentPanel.Controls.Add(pagination);

            contentPanel.Resize += (s, e) =>
            {
                if (pagination != null)
                {
                    pagination.Width = contentPanel.Width - 60;
                    pagination.Location = new Point(0, contentPanel.Height - 50);
                }
            };
        }

        private void LoadCategories()
        {
            try
            {
                var searchTerm = txtSearch?.Text?.Trim() ?? "";
                var currentPage = pagination?.CurrentPage ?? 1;
                var pageSize = pagination?.PageSize ?? 10;

                var categories = categoryService.GetCategories(currentPage, pageSize, searchTerm);
                var totalCount = categoryService.GetTotalCategoriesCount(searchTerm);

                categoriesGrid.DataSource = categories.Select(c => new
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = string.IsNullOrEmpty(c.Description) ? "No description" : c.Description,
                    CreatedDate = c.CreatedDate
                }).ToList();

                pagination.UpdatePagination(totalCount, currentPage, pageSize);
                totalCategoriesLabel.Text = $"Total Categories: {totalCount}";

                if (categories.Count == 0 && !string.IsNullOrEmpty(searchTerm))
                {
                    totalCategoriesLabel.Text = $"No categories found for '{searchTerm}'";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CategoriesGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var categoryId = (int)categoriesGrid.Rows[e.RowIndex].Cells["Id"].Value;

            if (categoriesGrid.Columns[e.ColumnIndex].Name == "Edit")
            {
                EditCategory(categoryId);
            }
            else if (categoriesGrid.Columns[e.ColumnIndex].Name == "Delete")
            {
                DeleteCategory(categoryId);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var dialog = new CategoryFormDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadCategories();
            }
        }

        private void EditCategory(int categoryId)
        {
            try
            {
                var category = categoryService.GetCategoryById(categoryId);
                if (category != null)
                {
                    var dialog = new CategoryFormDialog(category);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadCategories();
                    }
                }
                else
                {
                    MessageBox.Show("Category not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteCategory(int categoryId)
        {
            try
            {
                var category = categoryService.GetCategoryById(categoryId);
                if (category == null)
                {
                    MessageBox.Show("Category not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to delete the category '{category.Name}'?\n\nThis action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (categoryService.DeleteCategory(categoryId))
                    {
                        MessageBox.Show("Category deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCategories();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete category.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnMenuItemClicked(object sender, string menuItem)
        {
            if (menuItem.ToLower() == "categories")
            {
                return;
            }

            base.OnMenuItemClicked(sender, menuItem);
        }
    }
}