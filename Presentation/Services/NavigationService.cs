using System;
using System.Linq;
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

        public void ShowLaborForm()
        {
            try
            {
                var laborForm = new Forms.LaborForm();
                NavigateToForm(laborForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Labor form: {ex.Message}", "Navigation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ShowPaymentForm()
        {
            try
            {
                // TODO: Create PaymentForm once it's implemented
                MessageBox.Show("Payment module coming soon!", "Coming Soon",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // When PaymentForm is created, uncomment this:
                // var paymentForm = new Forms.PaymentForm();
                // NavigateToForm(paymentForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Payment form: {ex.Message}", "Navigation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NavigateToForm(Form newForm)
        {
            if (newForm == null) return;

            // Fade out the current form if it exists
            if (_currentForm != null && _currentForm != newForm && !_currentForm.IsDisposed)
            {
                try
                {
                    var fadeOutTimer = new Timer { Interval = 10 };
                    int opacity = 100;

                    fadeOutTimer.Tick += (s, e) =>
                    {
                        opacity -= 5;
                        if (_currentForm.InvokeRequired)
                        {
                            _currentForm.Invoke(new Action(() => _currentForm.Opacity = opacity / 100.0));
                        }
                        else
                        {
                            _currentForm.Opacity = opacity / 100.0;
                        }

                        if (opacity <= 0)
                        {
                            fadeOutTimer.Stop();
                            fadeOutTimer.Dispose();

                            // Close and dispose of the current form
                            if (_currentForm.InvokeRequired)
                            {
                                _currentForm.Invoke(new Action(() =>
                                {
                                    _currentForm.Close();
                                    _currentForm.Dispose();
                                }));
                            }
                            else
                            {
                                _currentForm.Close();
                                _currentForm.Dispose();
                            }

                            // Show the new form with fade-in
                            ShowNewFormWithFadeIn(newForm);
                        }
                    };

                    fadeOutTimer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error closing previous form: {ex.Message}", "Navigation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Fallback: show new form without animation
                    ShowNewFormWithFadeIn(newForm);
                }
            }
            else
            {
                // No current form, show new form directly
                ShowNewFormWithFadeIn(newForm);
            }
        }

        private void ShowNewFormWithFadeIn(Form newForm)
        {
            newForm.Opacity = 0;
            newForm.Show();
            _currentForm = newForm;
            _instance._currentForm = newForm;

            // Fade in the new form
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
                }
            };

            fadeInTimer.Start();
        }

        public void ExitApplication()
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to exit?", "Exit Application",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
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
                foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
                {
                    if (openForm != exceptForm && !openForm.IsDisposed)
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
                if (_currentForm != null && !_currentForm.IsDisposed)
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
                if (_currentForm != null && !_currentForm.IsDisposed)
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
                if (_currentForm != null && !_currentForm.IsDisposed)
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