using MaqboolFashion.Presentation.Forms;
using System;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConnectionTestForm());
        }
    }
}