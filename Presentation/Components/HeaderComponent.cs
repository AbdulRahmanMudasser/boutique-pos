using System;
using System.Drawing;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Components
{
    public class HeaderComponent : Panel
    {
        private const int HEADER_HEIGHT = 70;
        private Label titleLabel;
        private Button exitButton;
        private Button minimizeButton;

        public event EventHandler ExitClicked;
        public event EventHandler MinimizeClicked;

        public HeaderComponent(string title = "MAQBOOL FASHION")
        {
            InitializeHeader();
            CreateHeaderContent(title);
        }

        private void InitializeHeader()
        {
            this.Height = HEADER_HEIGHT;
            this.Dock = DockStyle.Top;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.None;
        }

        private void CreateHeaderContent(string title)
        {
            titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            minimizeButton = new Button
            {
                Text = "─",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(40, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(100, 100, 100),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            minimizeButton.FlatAppearance.BorderSize = 0;
            minimizeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            minimizeButton.Click += (s, e) => MinimizeClicked?.Invoke(this, EventArgs.Empty);

            exitButton = new Button
            {
                Text = "X",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(40, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(100, 100, 100),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 53, 69);
            exitButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 35, 51);
            exitButton.MouseEnter += (s, e) => exitButton.ForeColor = Color.White;
            exitButton.MouseLeave += (s, e) => exitButton.ForeColor = Color.FromArgb(100, 100, 100);
            exitButton.Click += (s, e) => ExitClicked?.Invoke(this, EventArgs.Empty);

            var borderLine = new Panel
            {
                Height = 1,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(230, 230, 230)
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(minimizeButton);
            this.Controls.Add(exitButton);
            this.Controls.Add(borderLine);

            this.Resize += (s, e) => PositionButtons();
            PositionButtons();
        }

        private void PositionButtons()
        {
            if (exitButton != null && minimizeButton != null)
            {
                exitButton.Location = new Point(this.Width - 50, 17);
                minimizeButton.Location = new Point(this.Width - 95, 17);
            }
        }

        public void UpdateTitle(string newTitle)
        {
            if (titleLabel != null)
            {
                titleLabel.Text = newTitle;
            }
        }

        public int GetHeaderHeight()
        {
            return HEADER_HEIGHT;
        }
    }
}