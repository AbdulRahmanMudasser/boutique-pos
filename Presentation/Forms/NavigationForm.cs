using System;
using System.Windows.Forms;
using MaqboolFashion.Presentation.Forms;

namespace MaqboolFashion.Presentation
{
    public class NavigationService
    {
        private static NavigationService _instance;
        private readonly Form _mainForm;
        private LoginForm _loginForm;
        private SignupForm _signupForm;
        private Form _currentForm;

        private NavigationService(Form mainForm)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
            _mainForm.FormClosed += (s, e) => Application.Exit();
        }

        public static NavigationService Instance(Form mainForm = null)
        {
            if (_instance == null)
            {
                if (mainForm == null)
                {
                    throw new ArgumentNullException(nameof(mainForm), "Main form must be provided when initializing NavigationService.");
                }
                _instance = new NavigationService(mainForm);
            }
            return _instance;
        }

        public void ShowLoginForm()
        {
            if (_loginForm == null || _loginForm.IsDisposed)
            {
                _loginForm = new LoginForm();
                _loginForm.FormClosed += (s, e) => HandleFormClosed(_loginForm);
            }

            SwitchToForm(_loginForm);
        }

        public void ShowSignupForm()
        {
            if (_signupForm == null || _signupForm.IsDisposed)
            {
                _signupForm = new SignupForm();
                _signupForm.FormClosed += (s, e) => HandleFormClosed(_signupForm);
            }

            SwitchToForm(_signupForm);
        }

        private void SwitchToForm(Form targetForm)
        {
            if (_currentForm != null && !_currentForm.IsDisposed)
            {
                _currentForm.Hide();
            }

            _currentForm = targetForm;
            if (!_currentForm.Visible)
            {
                _currentForm.Show();
            }
            _currentForm.BringToFront();
        }

        private void HandleFormClosed(Form form)
        {
            if (form == _currentForm)
            {
                _currentForm = null;
            }
            if (form == _loginForm)
            {
                _loginForm = null;
            }
            else if (form == _signupForm)
            {
                _signupForm = null;
            }
        }

        public void ExitApplication()
        {
            _loginForm?.Close();
            _signupForm?.Close();
            _mainForm?.Close();
            Application.Exit();
        }
    }
}