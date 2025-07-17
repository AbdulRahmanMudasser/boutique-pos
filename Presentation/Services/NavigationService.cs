using System;
using System.Windows.Forms;

namespace MaqboolFashion.Presentation.Services
{
    public class NavigationService
    {
        private static NavigationService _instance;
        private Form _currentForm;
        private static readonly object _lock = new object();

        private NavigationService()
        {
        }

        public static NavigationService Instance(Form currentForm = null)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new NavigationService();
                    }
                }
            }

            if (currentForm != null)
            {
                _instance._currentForm = currentForm;
            }

            return _instance;
        }

        public void ShowLoginForm()
        {
            try
            {
                var loginForm = new Forms.LoginForm();
                NavigateToForm(loginForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Login form: {ex.Message}", "Navigation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowSignupForm()
        {
            try
            {
                var signupForm = new Forms.SignupForm();
                NavigateToForm(signupForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Signup form: {ex.Message}", "Navigation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowCategoriesForm()
        {
            try
            {
                var categoriesForm = new Forms.CategoriesForm();
                NavigateToForm(categoriesForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Categories form: {ex.Message}", "Navigation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowDashboardForm()
        {
            try
            {
                var dashboardForm = new Forms.DashboardForm();
                NavigateToForm(dashboardForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Dashboard form: {ex.Message}", "Navigation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NavigateToForm(Form newForm)
        {
            if (newForm == null) return;

            // Close and dispose of the current form properly
            if (_currentForm != null && _currentForm != newForm)
            {
                var formToClose = _currentForm;
                _currentForm = null;

                // Show new form first
                newForm.Opacity = 0;
                newForm.Show();

                // Animate new form in
                var fadeInTimer = new Timer { Interval = 10 };
                int opacity = 0;

                fadeInTimer.Tick += (s, e) =>
                {
                    opacity += 5;
                    newForm.Opacity = opacity / 100.0;

                    if (opacity >= 100)
                    {
                        fadeInTimer.Stop();
                        fadeInTimer.Dispose();

                        // Close and dispose old form after animation
                        try
                        {
                            if (formToClose != null && !formToClose.IsDisposed)
                            {
                                formToClose.Hide();
                                formToClose.Close();
                                formToClose.Dispose();
                            }
                        }
                        catch { }
                    }
                };

                fadeInTimer.Start();
            }
            else
            {
                newForm.Show();
            }

            _currentForm = newForm;
            _instance._currentForm = newForm;
        }

        public void ExitApplication()
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to exit?", "Exit Application",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (Form openForm in Application.OpenForms)
                    {
                        if (openForm.InvokeRequired)
                        {
                            openForm.Invoke(new Action(() => openForm.Close()));
                        }
                        else
                        {
                            openForm.Close();
                        }
                    }

                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing application: {ex.Message}", "Exit Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                Environment.Exit(0);
            }
        }

        public Form GetCurrentForm()
        {
            return _currentForm;
        }

        public void CloseAllFormsExcept(Form exceptForm)
        {
            try
            {
                var formsToClose = new System.Collections.Generic.List<Form>();

                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm != exceptForm)
                    {
                        formsToClose.Add(openForm);
                    }
                }

                foreach (var form in formsToClose)
                {
                    if (form.InvokeRequired)
                    {
                        form.Invoke(new Action(() => form.Close()));
                    }
                    else
                    {
                        form.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing forms: {ex.Message}", "Navigation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RefreshCurrentForm()
        {
            try
            {
                if (_currentForm != null)
                {
                    _currentForm.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing form: {ex.Message}", "Refresh Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void MinimizeCurrentForm()
        {
            try
            {
                if (_currentForm != null)
                {
                    _currentForm.WindowState = FormWindowState.Minimized;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error minimizing form: {ex.Message}", "Window Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void RestoreCurrentForm()
        {
            try
            {
                if (_currentForm != null)
                {
                    _currentForm.WindowState = FormWindowState.Normal;
                    _currentForm.BringToFront();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring form: {ex.Message}", "Window Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}