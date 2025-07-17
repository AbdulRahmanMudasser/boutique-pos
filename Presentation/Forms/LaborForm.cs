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
        private Label totalLaborsLabel;
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
            Panel contentPanel = GetContentPanel();

            CreateHeaderSection(contentPanel);
            CreateSearchAndActionPanel(contentPanel);
            CreateDataGrid(contentPanel);
            CreatePagination(contentPanel);

            LoadLabors();
        }

        private void CreateHeaderSection(Panel contentPanel)
        {
            Panel headerContainer = new Panel
            {
                Size = new Size(contentPanel.Width - 60, 100),
                Location = new Point(0, 0),
                BackColor = Color.White
            };

            Label headerLabel = new Label
            {
                Text = "Labor Management",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 10)
            };

            Label subtitleLabel = new Label
            {
                Text = "Manage your workforce effectively with complete employee records",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(0, 50)
            };

            totalLaborsLabel = new Label
            {
                Text = "Total Workers: 0",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 75)
            };

            Panel separatorLine = new Panel
            {
                Height = 2,
                Width = headerContainer.Width,
                Location = new Point(0, 95),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            headerContainer.Controls.Add(headerLabel);
            headerContainer.Controls.Add(subtitleLabel);
            headerContainer.Controls.Add(totalLaborsLabel);
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
                Rectangle rect = searchPanel.ClientRectangle;
                using (Pen pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }
            };

            // Main container for better layout control
            Panel mainContainer = new Panel
            {
                Size = new Size(searchPanel.Width - 40, 80),
                Location = new Point(20, 10),
                BackColor = Color.Transparent
            };

            // Left section - Search
            Panel searchSection = new Panel
            {
                Size = new Size(450, 70),
                Location = new Point(0, 10),
                BackColor = Color.Transparent
            };

            Label lblSearch = new Label
            {
                Text = "🔍 Search Workers",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(5, 0)
            };

            Panel searchInputContainer = new Panel
            {
                Size = new Size(400, 45),
                Location = new Point(5, 25),
                BackColor = Color.White
            };

            searchInputContainer.Paint += (s, e) =>
            {
                Rectangle rect = searchInputContainer.ClientRectangle;
                using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 2))
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
                ForeColor = Color.Black
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
                LoadLabors();
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
                    LoadLabors();
                    e.SuppressKeyPress = true; // Prevent beep sound
                }
            };

            searchInputContainer.Controls.Add(txtSearch);
            searchSection.Controls.Add(lblSearch);
            searchSection.Controls.Add(searchInputContainer);

            // Right section - Actions
            actionPanel = new Panel
            {
                Size = new Size(300, 70),
                Location = new Point(mainContainer.Width - 300, 10),
                BackColor = Color.Transparent
            };

            Label lblActions = new Label
            {
                Text = "⚙️ Actions",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(5, 0)
            };

            Panel buttonContainer = new Panel
            {
                Size = new Size(290, 45),
                Location = new Point(5, 25),
                BackColor = Color.Transparent
            };

            btnAdd = new Button
            {
                Text = "➕ Add Worker",
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
                Rectangle rect = btnAdd.ClientRectangle;
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                }
                using (SolidBrush brush = new SolidBrush(btnAdd.BackColor))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
                using (SolidBrush brush = new SolidBrush(btnAdd.ForeColor))
                {
                    StringFormat stringFormat = new StringFormat
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
            btnRefresh.Click += (s, e) => LoadLabors();

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

            contentPanel.Resize += (s, e) =>
            {
                if (searchPanel != null && mainContainer != null && actionPanel != null)
                {
                    searchPanel.Width = contentPanel.Width - 60;
                    mainContainer.Width = searchPanel.Width - 40;
                    actionPanel.Location = new Point(mainContainer.Width - 300, 10);
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
                Font = new Font("Segoe UI", 10),
                GridColor = Color.FromArgb(240, 240, 240),
                RowTemplate = { Height = 45 },
                EnableHeadersVisualStyles = false
            };

            laborGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(10, 8, 10, 8),
                SelectionBackColor = Color.Black,
                SelectionForeColor = Color.White
            };

            laborGrid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                SelectionBackColor = Color.FromArgb(230, 230, 230),
                SelectionForeColor = Color.Black,
                Font = new Font("Segoe UI", 10),
                Padding = new Padding(10, 6, 10, 6),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            laborGrid.AlternatingRowsDefaultCellStyle = laborGrid.DefaultCellStyle;

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
                FillWeight = 15,
                MinimumWidth = 120
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Area",
                HeaderText = "Area",
                DataPropertyName = "Area",
                FillWeight = 12,
                MinimumWidth = 100
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "City",
                HeaderText = "City",
                DataPropertyName = "City",
                FillWeight = 12,
                MinimumWidth = 100
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PhoneNumber",
                HeaderText = "Phone",
                DataPropertyName = "PhoneNumber",
                FillWeight = 12,
                MinimumWidth = 110
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CNIC",
                HeaderText = "CNIC",
                DataPropertyName = "CNIC",
                FillWeight = 13,
                MinimumWidth = 130
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Caste",
                HeaderText = "Caste",
                DataPropertyName = "Caste",
                FillWeight = 10,
                MinimumWidth = 80
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cost",
                HeaderText = "Cost (Rs.)",
                DataPropertyName = "Cost",
                FillWeight = 10,
                MinimumWidth = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    SelectionBackColor = Color.FromArgb(230, 230, 230),
                    SelectionForeColor = Color.Black,
                    Font = new Font("Segoe UI", 10),
                    Padding = new Padding(10, 6, 10, 6)
                }
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "JoiningDate",
                HeaderText = "Joining Date",
                DataPropertyName = "JoiningDate",
                FillWeight = 12,
                MinimumWidth = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy",
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    SelectionBackColor = Color.FromArgb(230, 230, 230),
                    SelectionForeColor = Color.Black,
                    Font = new Font("Segoe UI", 10),
                    Padding = new Padding(10, 6, 10, 6)
                }
            });

            laborGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CurrentAdvance",
                HeaderText = "Advance (Rs.)",
                DataPropertyName = "CurrentAdvance",
                FillWeight = 10,
                MinimumWidth = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    SelectionBackColor = Color.FromArgb(230, 230, 230),
                    SelectionForeColor = Color.Black,
                    Font = new Font("Segoe UI", 10),
                    Padding = new Padding(10, 6, 10, 6)
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
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(3)
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
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(3)
                }
            };

            laborGrid.Columns.Add(editColumn);
            laborGrid.Columns.Add(deleteColumn);

            foreach (DataGridViewColumn column in laborGrid.Columns)
            {
                if (column.Name != "Edit" && column.Name != "Delete" &&
                    column.Name != "JoiningDate" && column.Name != "Cost" && column.Name != "CurrentAdvance")
                {
                    column.DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.White,
                        ForeColor = Color.Black,
                        SelectionBackColor = Color.FromArgb(230, 230, 230),
                        SelectionForeColor = Color.Black,
                        Font = new Font("Segoe UI", 10),
                        Padding = new Padding(10, 6, 10, 6),
                        Alignment = DataGridViewContentAlignment.MiddleLeft
                    };
                }
            }

            laborGrid.CellClick += LaborGrid_CellClick;

            laborGrid.RowsAdded += (s, e) =>
            {
                for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                {
                    if (i < laborGrid.Rows.Count)
                    {
                        DataGridViewRow row = laborGrid.Rows[i];
                        row.DefaultCellStyle = new DataGridViewCellStyle
                        {
                            BackColor = Color.White,
                            ForeColor = Color.Black,
                            SelectionBackColor = Color.FromArgb(230, 230, 230),
                            SelectionForeColor = Color.Black,
                            Font = new Font("Segoe UI", 10),
                            Padding = new Padding(10, 6, 10, 6),
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
            pagination.PageChanged += (s, page) => LoadLabors();
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

        private void LoadLabors()
        {
            try
            {
                var searchTerm = txtSearch?.Text?.Trim() ?? "";
                var currentPage = pagination?.CurrentPage ?? 1;
                var pageSize = pagination?.PageSize ?? 10;

                var labors = laborService.GetLabors(currentPage, pageSize, searchTerm);
                var totalCount = laborService.GetTotalLaborsCount(searchTerm);

                laborGrid.DataSource = labors.Select(l => new
                {
                    Id = l.Id,
                    Name = l.Name,
                    Area = l.Area,
                    City = l.City,
                    PhoneNumber = l.PhoneNumber,
                    CNIC = l.CNIC,
                    Caste = l.Caste,
                    Cost = l.Cost,
                    JoiningDate = l.JoiningDate,
                    CurrentAdvance = l.CurrentAdvance
                }).ToList();

                pagination.UpdatePagination(totalCount, currentPage, pageSize);
                totalLaborsLabel.Text = $"Total Workers: {totalCount}";

                if (labors.Count == 0 && !string.IsNullOrEmpty(searchTerm))
                {
                    totalLaborsLabel.Text = $"No workers found for '{searchTerm}'";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading labors: {ex.Message}", "Error",
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
                LoadLabors();
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
                        LoadLabors();
                    }
                }
                else
                {
                    MessageBox.Show("Worker not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading worker: {ex.Message}", "Error",
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
                    MessageBox.Show("Worker not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to delete worker '{labor.Name}'?\n\nThis action cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    if (laborService.DeleteLabor(laborId))
                    {
                        MessageBox.Show("Worker deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadLabors();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete worker.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting worker: {ex.Message}", "Error",
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