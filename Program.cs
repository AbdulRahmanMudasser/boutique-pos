using MaqboolFashion.Presentation.Forms;
using MaqboolFashion.Presentation.Services;
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
            var signupForm = new SignupForm();
            NavigationService.Instance(signupForm); // Initialize NavigationService with the starting form
            Application.Run(signupForm);
        }
    }
}