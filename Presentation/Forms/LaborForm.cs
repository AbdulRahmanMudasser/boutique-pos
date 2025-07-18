using MaqboolFashion.Data.Database;
using MaqboolFashion.Presentation.Components;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Forms
{
    public class LaborForm : BaseFormComponent
    {
        private LaborService laborService;
        private DataGridView laborGrid;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnRefresh;
        private PaginationComponent pagination;
        private Panel searchPanel;
        private Panel actionPanel;
        private Label totalLaborLabel;
        private Timer searchTimer;

        public LaborForm() : base("LABOR MANAGEMENT - MAQBOOL FASHION")
        {
            this.Text = "MaqboolFashion - Labor Management";
            laborService = new LaborService();
            SetActiveMenu("Labor");
            LoadPageContent();
        }

        protected override void CreatePageContent()
        {
            var contentPanel = GetContentPanel();

            // Enable only vertical scroll for the content panel
            contentPanel.AutoScroll = true;
            contentPanel.AutoScrollMinSize = new Size(0, 700); // Only vertical scroll

            CreateHeaderSection(contentPanel);
            CreateSearchAndActionPanel(contentPanel);
            CreateDataGrid(contentPanel);
            CreatePagination(contentPanel);

            LoadLabor();
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
                Text = "Labor Management",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 10)
            };

            var subtitleLabel = new Label
            {
                Text = "Manage your workforce efficiently - track workers, their details, and advance payments",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(0, 50)
            };

            totalLaborLabel = new Label
            {
                Text = "Total Labor: 0",
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
            headerContainer.Controls.Add(totalLaborLabel);
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
                Text = "🔍 Search Labor",
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
                LoadLabor();
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
                    LoadLabor();
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
                Text = "➕ Add Labor",
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
            btnRefresh.Click += (s, e) => LoadLabor();

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

            laborGrid = new DataGridView
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
                EnableHeadersVisualStyles = false,
                ScrollBars = ScrollBars.Vertical // Only vertical scrollbar
            };

            // Header style
            laborGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
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
            laborGrid.DefaultCellStyle = new DataGridViewCellStyle
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
            laborGrid.AlternatingRowsDefaultCellStyle = laborGrid.DefaultCellStyle;

            // Add columns with consistent styling
            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Visible = false
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Name",
                DataPropertyName = "Name",
                FillWeight = 18,
                MinimumWidth = 120
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Area",
                HeaderText = "Area",
                DataPropertyName = "Area",
                FillWeight = 15,
                MinimumWidth = 100
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "City",
                HeaderText = "City",
                DataPropertyName = "City",
                FillWeight = 15,
                MinimumWidth = 100
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PhoneNumber",
                HeaderText = "Phone",
                DataPropertyName = "PhoneNumber",
                FillWeight = 15,
                MinimumWidth = 110
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cost",
                HeaderText = "Daily Cost",
                DataPropertyName = "Cost",
                FillWeight = 12,
                MinimumWidth = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C0",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    SelectionBackColor = Color.FromArgb(230, 230, 230),
                    SelectionForeColor = Color.Black,
                    Font = new Font("Segoe UI", 11),
                    Padding = new Padding(15, 8, 15, 8)
                }
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CurrentAdvance",
                HeaderText = "Advance",
                DataPropertyName = "CurrentAdvance",
                FillWeight = 12,
                MinimumWidth = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C0",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
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
                FillWeight = 8,
                MinimumWidth = 70,
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
                FillWeight = 8,
                MinimumWidth = 70,
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

            laborGrid.Columns.Add(editColumn);
            laborGrid.Columns.Add(deleteColumn);

            // Ensure all columns inherit the same base style
            foreach (DataGridViewColumn column in laborGrid.Columns)
            {
                if (column.Name != "Edit" && column.Name != "Delete" &&
                    column.Name != "Cost" && column.Name != "CurrentAdvance")
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

            laborGrid.CellClick += LaborGrid_CellClick;

            // Force consistent row styling
            laborGrid.RowsAdded += (s, e) =>
            {
                for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                {
                    if (i < laborGrid.Rows.Count)
                    {
                        var row = laborGrid.Rows[i];
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

            gridContainer.Controls.Add(laborGrid);
            contentPanel.Controls.Add(gridContainer);

            contentPanel.Resize += (s, e) =>
            {
                if (gridContainer != null && laborGrid != null)
                {
                    gridContainer.Width = contentPanel.Width - 60;
                    laborGrid.Width = gridContainer.Width - 4;
                }
            };
        }

        private void CreatePagination(Panel contentPanel)
        {
            pagination = new PaginationComponent();
            pagination.PageChanged += (s, page) => LoadLabor();
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

        private void LoadLabor()
        {
            try
            {
                var searchTerm = txtSearch?.Text?.Trim() ?? "";
                var currentPage = pagination?.CurrentPage ?? 1;
                var pageSize = pagination?.PageSize ?? 10;

                var laborList = laborService.GetLabor(currentPage, pageSize, searchTerm);
                var totalCount = laborService.GetTotalLaborCount(searchTerm);

                laborGrid.DataSource = laborList.Select(l => new
                {
                    Id = l.Id,
                    Name = l.Name,
                    Area = l.Area,
                    City = l.City,
                    PhoneNumber = LaborService.FormatPhoneNumber(l.PhoneNumber),
                    Cost = l.Cost,
                    CurrentAdvance = l.CurrentAdvance
                }).ToList();

                pagination.UpdatePagination(totalCount, currentPage, pageSize);
                totalLaborLabel.Text = $"Total Labor: {totalCount}";

                if (laborList.Count == 0 && !string.IsNullOrEmpty(searchTerm))
                {
                    totalLaborLabel.Text = $"No labor found for '{searchTerm}'";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading labor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LaborGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var laborId = (int)laborGrid.Rows[e.RowIndex].Cells["Id"].Value;

            if (laborGrid.Columns[e.ColumnIndex].Name == "Edit")
            {
                EditLabor(laborId);
            }
            else if (laborGrid.Columns[e.ColumnIndex].Name == "Delete")
            {
                DeleteLabor(laborId);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var dialog = new LaborFormDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadLabor();
            }
        }

        private void EditLabor(int laborId)
        {
            try
            {
                var labor = laborService.GetLaborById(laborId);
                if (labor != null)
                {
                    var dialog = new LaborFormDialog(labor);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadLabor();
                    }
                }
                else
                {
                    MessageBox.Show("Labor not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading labor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteLabor(int laborId)
        {
            try
            {
                var labor = laborService.GetLaborById(laborId);
                if (labor == null)
                {
                    MessageBox.Show("Labor not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{labor.Name}'?\n\nThis action cannot be undone and will affect all related advance and payment records.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (laborService.DeleteLabor(laborId))
                    {
                        MessageBox.Show("Labor deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadLabor();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete labor.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting labor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnMenuItemClicked(object sender, string menuItem)
        {
            if (menuItem.ToLower() == "labor")
            {
                return;
            }

            base.OnMenuItemClicked(sender, menuItem);
        }
    }
}