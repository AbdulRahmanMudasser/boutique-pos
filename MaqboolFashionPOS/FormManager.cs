using System;
using System.Windows.Forms;

namespace MaqboolFashionPOS
{
    // Manages navigation between forms with smooth fade-in/fade-out transitions
    // Static class to ensure a single instance handles all form switches
    public static class FormManager
    {
        // Tracks the currently active form
        private static Form currentForm;

        // Switches to a new form with fade-out (current form) and fade-in (new form) animations
        public static void SwitchForm(Form newForm)
        {
            if (currentForm != null && !currentForm.IsDisposed)
            {
                // Fade out the current form
                Timer fadeOutTimer = new Timer { Interval = 50 };
                fadeOutTimer.Tick += (s, e) =>
                {
                    currentForm.Opacity -= 0.05;
                    if (currentForm.Opacity <= 0)
                    {
                        fadeOutTimer.Stop();
                        currentForm.Hide();
                        ShowNewForm(newForm);
                    }
                };
                fadeOutTimer.Start();
            }
            else
            {
                // No current form; directly show the new form
                ShowNewForm(newForm);
            }
        }

        // Shows the new form with a fade-in animation
        private static void ShowNewForm(Form newForm)
        {
            currentForm = newForm;
            currentForm.Opacity = 0;
            currentForm.Show();
            Timer fadeInTimer = new Timer { Interval = 50 };
            fadeInTimer.Tick += (s, e) =>
            {
                currentForm.Opacity += 0.05;
                if (currentForm.Opacity >= 1) fadeInTimer.Stop();
            };
            fadeInTimer.Start();
        }
    }
}