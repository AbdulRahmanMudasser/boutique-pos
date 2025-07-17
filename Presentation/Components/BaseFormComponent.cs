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
                case "labor":
                    navigationService.ShowLaborForm();
                    break;
                case "payment":
                    // navigationService.ShowPaymentForm(); // TODO: Create payment form
                    MessageBox.Show("Payment management coming soon!", "Coming Soon",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "products":
                    // navigationService.ShowProductsForm(); // TODO: Create products form
                    MessageBox.Show("Products management coming soon!", "Coming Soon",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "sales":
                    // navigationService.ShowSalesForm(); // TODO: Create sales form
                    MessageBox.Show("Sales management coming soon!", "Coming Soon",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "expenses":
                    // navigationService.ShowExpensesForm(); // TODO: Create expenses form
                    MessageBox.Show("Expenses management coming soon!", "Coming Soon",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "stock":
                    // navigationService.ShowStockForm(); // TODO: Create stock form
                    MessageBox.Show("Stock management coming soon!", "Coming Soon",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "reports":
                    // navigationService.ShowReportsForm(); // TODO: Create reports form
                    MessageBox.Show("Reports coming soon!", "Coming Soon",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case "settings":
                    // navigationService.ShowSettingsForm(); // TODO: Create settings form
                    MessageBox.Show("Settings coming soon!", "Coming Soon",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                default:
                    MessageBox.Show($"Navigation for {menuItem} not implemented yet.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseFormComponent
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "BaseFormComponent";
            this.Load += new System.EventHandler(this.BaseFormComponent_Load);
            this.ResumeLayout(false);

        }

        private void BaseFormComponent_Load(object sender, EventArgs e)
        {

        }
    }
}