using System;
using System.Drawing;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Components
{
    public class PaginationComponent : Panel
    {
        private int currentPage = 1;
        private int totalPages = 1;
        private int totalRecords = 0;
        private int pageSize = 10;

        private Label recordsLabel;
        private Button firstButton;
        private Button previousButton;
        private Button nextButton;
        private Button lastButton;
        private Label pageLabel;

        public event EventHandler<int> PageChanged;

        public int CurrentPage => currentPage;
        public int PageSize => pageSize;

        public PaginationComponent()
        {
            InitializePagination();
            CreatePaginationControls();
        }

        private void InitializePagination()
        {
            this.Height = 60;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.Dock = DockStyle.Bottom;

            this.Paint += (s, e) =>
            {
                var rect = this.ClientRectangle;
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.DrawLine(pen, 0, 0, rect.Width, 0);
                }
            };
        }

        private void CreatePaginationControls()
        {
            recordsLabel = new Label
            {
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(15, 20)
            };

            firstButton = CreatePaginationButton("⏮", 0);
            previousButton = CreatePaginationButton("◀", 1);

            pageLabel = new Label
            {
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Text = "Page 1 of 1"
            };

            nextButton = CreatePaginationButton("▶", 3);
            lastButton = CreatePaginationButton("⏭", 4);

            this.Controls.Add(recordsLabel);
            this.Controls.Add(firstButton);
            this.Controls.Add(previousButton);
            this.Controls.Add(pageLabel);
            this.Controls.Add(nextButton);
            this.Controls.Add(lastButton);

            this.Resize += (s, e) => PositionControls();
            PositionControls();
        }

        private Button CreatePaginationButton(string text, int index)
        {
            var button = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(45, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 40);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(20, 20, 20);

            button.Paint += (s, e) =>
            {
                var rect = button.ClientRectangle;
                using (var brush = new SolidBrush(button.BackColor))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }

                if (button.Enabled)
                {
                    using (var pen = new Pen(Color.FromArgb(60, 60, 60), 1))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                    }
                }

                using (var brush = new SolidBrush(button.ForeColor))
                {
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(button.Text, button.Font, brush, rect, stringFormat);
                }
            };

            button.Click += (s, e) =>
            {
                switch (index)
                {
                    case 0: // First
                        GoToPage(1);
                        break;
                    case 1: // Previous
                        GoToPage(currentPage - 1);
                        break;
                    case 3: // Next
                        GoToPage(currentPage + 1);
                        break;
                    case 4: // Last
                        GoToPage(totalPages);
                        break;
                }
            };

            return button;
        }

        private void PositionControls()
        {
            var centerY = (this.Height - 35) / 2;
            var centerX = this.Width / 2;
            var buttonWidth = 45;
            var spacing = 15;
            var totalWidth = (buttonWidth * 4) + (spacing * 3) + 140; // 140 for page label

            var startX = centerX - (totalWidth / 2);

            firstButton.Location = new Point(startX, centerY);
            previousButton.Location = new Point(startX + buttonWidth + spacing, centerY);
            pageLabel.Location = new Point(startX + (buttonWidth * 2) + (spacing * 2) + 15, centerY + 8);
            nextButton.Location = new Point(startX + (buttonWidth * 2) + (spacing * 2) + 140, centerY);
            lastButton.Location = new Point(startX + (buttonWidth * 3) + (spacing * 3) + 140, centerY);
        }

        public void UpdatePagination(int totalRecordsCount, int currentPageNumber, int recordsPerPage)
        {
            totalRecords = totalRecordsCount;
            currentPage = currentPageNumber;
            pageSize = recordsPerPage;
            totalPages = Math.Max(1, (int)Math.Ceiling((double)totalRecords / pageSize));

            var startRecord = totalRecords == 0 ? 0 : (currentPage - 1) * pageSize + 1;
            var endRecord = Math.Min(currentPage * pageSize, totalRecords);

            recordsLabel.Text = totalRecords == 0
                ? "No records found"
                : $"📊 Showing {startRecord} to {endRecord} of {totalRecords} entries";

            pageLabel.Text = $"Page {currentPage} of {totalPages}";

            firstButton.Enabled = currentPage > 1;
            previousButton.Enabled = currentPage > 1;
            nextButton.Enabled = currentPage < totalPages;
            lastButton.Enabled = currentPage < totalPages;

            UpdateButtonStyles();
        }

        private void UpdateButtonStyles()
        {
            var buttons = new[] { firstButton, previousButton, nextButton, lastButton };

            foreach (var button in buttons)
            {
                if (button.Enabled)
                {
                    button.BackColor = Color.Black;
                    button.ForeColor = Color.White;
                }
                else
                {
                    button.BackColor = Color.FromArgb(200, 200, 200);
                    button.ForeColor = Color.FromArgb(120, 120, 120);
                }
                button.Invalidate(); // Force repaint with new colors
            }
        }

        private void GoToPage(int pageNumber)
        {
            if (pageNumber >= 1 && pageNumber <= totalPages && pageNumber != currentPage)
            {
                currentPage = pageNumber;
                PageChanged?.Invoke(this, currentPage);
            }
        }

        public void Reset()
        {
            currentPage = 1;
            UpdatePagination(0, 1, pageSize);
        }
    }
}