using System;
using System.Drawing;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Components
{
    public class DashboardCardComponent : Panel
    {
        private Label iconLabel;
        private Label titleLabel;
        private Label valueLabel;
        private Label subValueLabel;
        private Button actionButton;
        private Panel headerStripe;

        public event EventHandler ActionClicked;

        public string CardTitle { get; set; }
        public string CardValue { get; set; }
        public string CardSubValue { get; set; }
        public string CardIcon { get; set; }
        public Color AccentColor { get; set; }
        public string ActionText { get; set; }

        public DashboardCardComponent(string title, string value, string icon, Color accentColor, string subValue = "", string actionText = "View Details")
        {
            CardTitle = title;
            CardValue = value;
            CardSubValue = subValue;
            CardIcon = icon;
            AccentColor = accentColor;
            ActionText = actionText;

            InitializeCard();
            CreateCardContent();
        }

        private void InitializeCard()
        {
            this.Size = new Size(300, 160);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.None;
            this.Cursor = Cursors.Hand;
            this.Margin = new Padding(15);

            this.Paint += (s, e) =>
            {
                var rect = this.ClientRectangle;
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
                }

                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 2, 2, rect.Width, rect.Height);
                }
            };

            this.MouseEnter += (s, e) => this.BackColor = Color.FromArgb(248, 249, 250);
            this.MouseLeave += (s, e) => this.BackColor = Color.White;
        }

        private void CreateCardContent()
        {
            headerStripe = new Panel
            {
                Height = 4,
                Dock = DockStyle.Top,
                BackColor = AccentColor
            };

            iconLabel = new Label
            {
                Text = CardIcon,
                Font = new Font("Segoe UI", 28),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            titleLabel = new Label
            {
                Text = CardTitle,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(20, 75),
                MaximumSize = new Size(200, 0)
            };

            valueLabel = new Label
            {
                Text = CardValue,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(20, 100)
            };

            if (!string.IsNullOrEmpty(CardSubValue))
            {
                subValueLabel = new Label
                {
                    Text = CardSubValue,
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.FromArgb(108, 117, 125),
                    AutoSize = true,
                    Location = new Point(20, 125)
                };
                this.Controls.Add(subValueLabel);
            }

            actionButton = new Button
            {
                Text = ActionText,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(90, 30),
                Location = new Point(180, 120),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            actionButton.FlatAppearance.BorderSize = 0;
            actionButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 40);
            actionButton.Click += (s, e) => ActionClicked?.Invoke(this, EventArgs.Empty);

            this.Controls.Add(headerStripe);
            this.Controls.Add(iconLabel);
            this.Controls.Add(titleLabel);
            this.Controls.Add(valueLabel);
            this.Controls.Add(actionButton);
        }

        public void UpdateValue(string newValue, string newSubValue = "")
        {
            if (valueLabel != null)
            {
                valueLabel.Text = newValue;
            }

            if (subValueLabel != null && !string.IsNullOrEmpty(newSubValue))
            {
                subValueLabel.Text = newSubValue;
            }
        }

        public void UpdateAccentColor(Color newColor)
        {
            AccentColor = newColor;
            if (headerStripe != null) headerStripe.BackColor = newColor;
            if (iconLabel != null) iconLabel.ForeColor = newColor;
            if (valueLabel != null) valueLabel.ForeColor = newColor;
        }

        public static DashboardCardComponent CreateSalesCard(decimal amount = 0)
        {
            return new DashboardCardComponent(
                "Total Sales",
                $"Rs. {amount:N0}",
                "💰",
                Color.FromArgb(40, 167, 69),
                "This month"
            );
        }

        public static DashboardCardComponent CreateExpensesCard(decimal amount = 0)
        {
            return new DashboardCardComponent(
                "Total Expenses",
                $"Rs. {amount:N0}",
                "💸",
                Color.FromArgb(220, 53, 69),
                "This month"
            );
        }

        public static DashboardCardComponent CreateProductsCard(int count = 0)
        {
            return new DashboardCardComponent(
                "Products",
                count.ToString(),
                "📦",
                Color.FromArgb(13, 110, 253),
                "In inventory"
            );
        }

        public static DashboardCardComponent CreateStockCard(int count = 0)
        {
            return new DashboardCardComponent(
                "Stock Items",
                count.ToString(),
                "📊",
                Color.FromArgb(111, 66, 193),
                "Total items"
            );
        }
    }
}