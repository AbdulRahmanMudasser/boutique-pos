namespace MaqboolFashionPOS
{
    // Minimal designer file for RegisterForm, supporting programmatic control setup
    partial class RegisterForm
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
            this.Text = "Register - Maqbool Fashion POS";
        }
    }
}