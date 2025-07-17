using MaqboolFashion.Presentation.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Components
{
    public abstract class BaseFormComponent : Form
    {
        protected SidebarComponent sidebar;
        protected HeaderComponent header;
        protected Panel contentPanel;
        protected NavigationService navigationService;

        public BaseFormComponent(string formTitle = "MAQBOOL FASHION")
        {
            navigationService = NavigationService.Instance(this);
            InitializeBaseForm();
            CreateBaseLayout(formTitle);
            SetupEventHandlers();
        }

        private void InitializeBaseForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            this.KeyPreview = true;
        }

        private void CreateBaseLayout(string formTitle)
        {
            header = new HeaderComponent(formTitle);
            this.Controls.Add(header);

            sidebar = new SidebarComponent();
            this.Controls.Add(sidebar);

            contentPanel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(30),
                AutoScroll = true
            };
            this.Controls.Add(contentPanel);

            this.Resize += (s, e) => PositionContentPanel();
            PositionContentPanel();
        }

        private void PositionContentPanel()
        {
            if (contentPanel != null && sidebar != null && header != null)
            {
                contentPanel.Location = new Point(sidebar.GetSidebarWidth(), header.GetHeaderHeight());
                contentPanel.Size = new Size(
                    this.ClientSize.Width - sidebar.GetSidebarWidth(),
                    this.ClientSize.Height - header.GetHeaderHeight()
                );
            }
        }

        private void SetupEventHandlers()
        {
            if (header != null)
            {
                header.ExitClicked += (s, e) => navigationService.ExitApplication();
                header.MinimizeClicked += (s, e) => this.WindowState = FormWindowState.Minimized;
            }

            if (sidebar != null)
            {
                sidebar.MenuItemClicked += OnMenuItemClicked;
            }

            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    navigationService.ExitApplication();
                }
            };
        }

        protected virtual void OnMenuItemClicked(object sender, string menuItem)
        {
            sidebar.SetActiveMenu(menuItem);

            switch (menuItem.ToLower())
            {
                case "dashboard":
                    navigationService.ShowDashboardForm();
                    break;
                case "categories":
                    navigationService.ShowCategoriesForm();
                    break;
                case "products":
                    // navigationService.ShowProductsForm();
                    break;
                case "sales":
                    // navigationService.ShowSalesForm();
                    break;
                case "expenses":
                    // navigationService.ShowExpensesForm();
                    break;
                case "stock":
                    // navigationService.ShowStockForm();
                    break;
                case "reports":
                    // navigationService.ShowReportsForm();
                    break;
                case "settings":
                    // navigationService.ShowSettingsForm();
                    break;
            }
        }

        protected void SetActiveMenu(string menuName)
        {
            sidebar?.SetActiveMenu(menuName);
        }

        protected void UpdateHeaderTitle(string newTitle)
        {
            header?.UpdateTitle(newTitle);
        }

        protected Panel GetContentPanel()
        {
            return contentPanel;
        }

        protected abstract void CreatePageContent();

        public void LoadPageContent()
        {
            if (contentPanel != null)
            {
                contentPanel.Controls.Clear();
                CreatePageContent();
            }
        }
    }
}