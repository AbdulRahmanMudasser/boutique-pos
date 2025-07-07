namespace MaqboolFashionPOS
{
    // Minimal designer file for LoginForm, supporting programmatic control setup
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        // Standard Dispose method for resource cleanup
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // Minimal InitializeComponent for form properties
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Text = "Maqbool Fashion POS - Login";
            this.Load += new System.EventHandler(this.LoginForm_Load);
        }
    }
}