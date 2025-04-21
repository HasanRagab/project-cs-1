using Terminal.Gui;
using App.Routes;
using App.Utils;
using App.Services;

namespace App.Pages;


public class SignupPage : Page
{
    public override Window Display(Router router)
    {
        var win = CreateWindow(router, "Sign Up");

        var (emailLabel, emailInput) = UIHelper.CreateLabeledInput("Email:", 4);
        var (passwordLabel, passwordInput) = UIHelper.CreateLabeledInput("Password:", 6);
        var (confirmPasswordLabel, confirmPasswordInput) = UIHelper.CreateLabeledInput("Confirm Password:", 8);

        passwordInput.Secret = true;
        confirmPasswordInput.Secret = true;

        var signupButton = UIHelper.CreateButton("Sign Up", 10, () =>
        {
            var email = emailInput.Text.ToString() ?? string.Empty;
            var password = passwordInput.Text.ToString() ?? string.Empty;
            var confirmPassword = confirmPasswordInput.Text.ToString() ?? string.Empty;

            if (!ValidateInput(email, emailInput, "Email") || !ValidateInput(password, passwordInput, "Password") || !ValidateInput(confirmPassword, confirmPasswordInput, "Confirm Password"))
            {
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.ErrorQuery("Error", "Passwords do not match.", "OK");
                confirmPasswordInput.SetFocus();
                return;
            }

            try
            {
                AuthService.RegisterUser(email, password);
                router.Navigate("/login");
                MessageBox.Query("Success", "Sign up successful! Please login.", "OK");
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", ex.Message, "OK");
            }
        });

        win.Add(emailLabel, emailInput);
        win.Add(passwordLabel, passwordInput);
        win.Add(confirmPasswordLabel, confirmPasswordInput);
        win.Add(signupButton);

        emailInput.SetFocus();

        return win;
    }
}