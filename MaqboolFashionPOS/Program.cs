using System;
using System.Windows.Forms;

namespace MaqboolFashionPOS
{
    // Application entry point, launches the LoginForm
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}