using MaqboolFashion.Presentation.Components;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Forms
{
    public class DashboardForm : BaseFormComponent
    {
        private FlowLayoutPanel dashboardGrid;
        private Label welcomeLabel;
        private Label summaryLabel;

        public DashboardForm() : base("DASHBOARD - MAQBOOL FASHION")
        {
            this.Text = "MaqboolFashion - Dashboard";
            SetActiveMenu("Dashboard");
            LoadPageContent();
        }

        protected override void CreatePageContent()
        {
            var contentPanel = GetContentPanel();

            welcomeLabel = new Label
            {
                Text = "Welcome to Maqbool Fashion Dashboard",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(0, 0),
                Margin = new Padding(0, 0, 0, 10)
            };

            summaryLabel = new Label
            {
                Text = "Overview of your business performance and key metrics",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = true,
                Location = new Point(0, 40),
                Margin = new Padding(0, 0, 0, 30)
            };

            dashboardGrid = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(0, 100),
                MaximumSize = new Size(contentPanel.Width - 60, 0),
                Padding = new Padding(0)
            };

            CreateDashboardCards();

            contentPanel.Controls.Add(welcomeLabel);
            contentPanel.Controls.Add(summaryLabel);
            contentPanel.Controls.Add(dashboardGrid);

            contentPanel.Resize += (s, e) =>
            {
                if (dashboardGrid != null)
                {
                    dashboardGrid.MaximumSize = new Size(contentPanel.Width - 60, 0);
                }
            };
        }

        private void CreateDashboardCards()
        {
            var salesCard = DashboardCardComponent.CreateSalesCard(125000);
            salesCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Sales details coming soon!", "Sales", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var expensesCard = DashboardCardComponent.CreateExpensesCard(35000);
            expensesCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Expenses details coming soon!", "Expenses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var productsCard = DashboardCardComponent.CreateProductsCard(247);
            productsCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Products management coming soon!", "Products", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var stockCard = DashboardCardComponent.CreateStockCard(89);
            stockCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Stock management coming soon!", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var balanceCard = new DashboardCardComponent(
                "Current Balance",
                "Rs. 90,000",
                "💳",
                Color.FromArgb(13, 110, 253),
                "Available funds"
            );
            balanceCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Balance details coming soon!", "Balance", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var ordersCard = new DashboardCardComponent(
                "Pending Orders",
                "23",
                "📋",
                Color.FromArgb(255, 193, 7),
                "Need attention"
            );
            ordersCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Orders management coming soon!", "Orders", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var customersCard = new DashboardCardComponent(
                "Total Customers",
                "1,234",
                "👥",
                Color.FromArgb(32, 201, 151),
                "Registered users"
            );
            customersCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Customer management coming soon!", "Customers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var profitCard = new DashboardCardComponent(
                "Monthly Profit",
                "Rs. 90,000",
                "📈",
                Color.FromArgb(40, 167, 69),
                "This month"
            );
            profitCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Profit analysis coming soon!", "Profit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var lowStockCard = new DashboardCardComponent(
                "Low Stock Alert",
                "12",
                "⚠️",
                Color.FromArgb(220, 53, 69),
                "Items need restock"
            );
            lowStockCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Low stock management coming soon!", "Low Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            var revenueCard = new DashboardCardComponent(
                "Daily Revenue",
                "Rs. 15,750",
                "💰",
                Color.FromArgb(111, 66, 193),
                "Today's earnings"
            );
            revenueCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Revenue details coming soon!", "Revenue", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var suppliersCard = new DashboardCardComponent(
                "Active Suppliers",
                "18",
                "🏪",
                Color.FromArgb(253, 126, 20),
                "Verified suppliers"
            );
            suppliersCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Supplier management coming soon!", "Suppliers", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var returnsCard = new DashboardCardComponent(
                "Returns/Refunds",
                "Rs. 2,500",
                "↩️",
                Color.FromArgb(214, 51, 132),
                "This month"
            );
            returnsCard.ActionClicked += (s, e) =>
            {
                MessageBox.Show("Returns management coming soon!", "Returns", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            dashboardGrid.Controls.Add(salesCard);
            dashboardGrid.Controls.Add(expensesCard);
            dashboardGrid.Controls.Add(productsCard);
            dashboardGrid.Controls.Add(stockCard);
            dashboardGrid.Controls.Add(balanceCard);
            dashboardGrid.Controls.Add(ordersCard);
            dashboardGrid.Controls.Add(customersCard);
            dashboardGrid.Controls.Add(profitCard);
            dashboardGrid.Controls.Add(lowStockCard);
            dashboardGrid.Controls.Add(revenueCard);
            dashboardGrid.Controls.Add(suppliersCard);
            dashboardGrid.Controls.Add(returnsCard);
        }

        protected override void OnMenuItemClicked(object sender, string menuItem)
        {
            if (menuItem.ToLower() == "dashboard")
            {
                return;
            }

            base.OnMenuItemClicked(sender, menuItem);
        }
    }
}