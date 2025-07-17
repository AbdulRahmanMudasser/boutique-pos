using System;
using System.Drawing;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Components
{
    public class SidebarComponent : Panel
    {
        private const int SIDEBAR_WIDTH = 280;
        //private Panel headerPanel;
        private Panel navigationPanel;
        private string currentActiveMenu = "Dashboard";

        public event EventHandler<string> MenuItemClicked;

        public SidebarComponent()
        {
            InitializeSidebar();
            //CreateSidebarHeader();
            CreateNavigationMenu();
        }

        private void InitializeSidebar()
        {
            this.Width = SIDEBAR_WIDTH;
            this.Dock = DockStyle.Left;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.None;
        }

        //private void CreateSidebarHeader()
        //{
        //    headerPanel = new Panel
        //    {
        //        Height = 10,
        //        Dock = DockStyle.Top,
        //        BackColor = Color.Black
        //    };

        //    var brandIcon = new Label
        //    {
        //        Text = "MF",
        //        Font = new Font("Segoe UI", 20, FontStyle.Bold),
        //        ForeColor = Color.White,
        //        AutoSize = true,
        //        Location = new Point(20, 20)
        //    };

        //    var brandName = new Label
        //    {
        //        Text = "MAQBOOL FASHION",
        //        Font = new Font("Segoe UI", 10, FontStyle.Regular),
        //        ForeColor = Color.FromArgb(200, 200, 200),
        //        AutoSize = true,
        //        Location = new Point(65, 28)
        //    };

        //    headerPanel.Controls.Add(brandIcon);
        //    headerPanel.Controls.Add(brandName);
        //    this.Controls.Add(headerPanel);
        //}

        private void CreateNavigationMenu()
        {
            navigationPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0, 20, 0, 0)
            };

            var menuItems = new[]
            {
                new { Text = "Dashboard", Icon = "🏠" },
                new { Text = "Categories", Icon = "🗂️" },
                new { Text = "Products", Icon = "📦" },
                new { Text = "Labor", Icon = "👷" },
                new { Text = "Payment", Icon = "💳" },
                new { Text = "Sales", Icon = "💰" },
                new { Text = "Expenses", Icon = "💸" },
                new { Text = "Stock", Icon = "📊" },
                new { Text = "Reports", Icon = "📈" },
                new { Text = "Settings", Icon = "⚙️" }
            };

            int yPosition = 0;
            foreach (var item in menuItems)
            {
                var menuButton = CreateMenuButton(item.Text, item.Icon, yPosition);
                navigationPanel.Controls.Add(menuButton);
                yPosition += 55;
            }

            this.Controls.Add(navigationPanel);
        }

        private Button CreateMenuButton(string text, string icon, int yPosition)
        {
            bool isActive = text == currentActiveMenu;

            var button = new Button
            {
                Text = $"  {icon}    {text}",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Size = new Size(SIDEBAR_WIDTH - 20, 50),
                Location = new Point(10, yPosition),
                FlatStyle = FlatStyle.Flat,
                BackColor = isActive ? Color.Black : Color.White,
                ForeColor = isActive ? Color.White : Color.FromArgb(60, 60, 60),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand,
                Padding = new Padding(15, 0, 0, 0),
                Tag = text
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = isActive ? Color.FromArgb(40, 40, 40) : Color.FromArgb(248, 249, 250);

            button.Click += (sender, e) =>
            {
                SetActiveMenu(text);
                MenuItemClicked?.Invoke(this, text);
            };

            return button;
        }

        public void SetActiveMenu(string menuName)
        {
            currentActiveMenu = menuName;

            foreach (Control control in navigationPanel.Controls)
            {
                if (control is Button button)
                {
                    bool isActive = button.Tag.ToString() == menuName;
                    button.BackColor = isActive ? Color.Black : Color.White;
                    button.ForeColor = isActive ? Color.White : Color.FromArgb(60, 60, 60);
                    button.FlatAppearance.MouseOverBackColor = isActive ? Color.FromArgb(40, 40, 40) : Color.FromArgb(248, 249, 250);
                }
            }
        }

        public int GetSidebarWidth()
        {
            return SIDEBAR_WIDTH;
        }
    }
}