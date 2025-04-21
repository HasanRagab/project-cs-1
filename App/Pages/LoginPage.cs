using Terminal.Gui;
using App.Routes;
using App.Utils;
using Figgle;
using App.Services;

namespace App.Pages;

public class LoginPage : Page
{
    public override Window Display(Router router)
    {
        var win = CreateWindow(router, "Login");

        var (emailLabel, emailInput) = UIHelper.CreateLabeledInput("Email:", 4);
        var (passwordLabel, passwordInput) = UIHelper.CreateLabeledInput("Password:", 6);
        passwordInput.Secret = true;

        var loginButton = UIHelper.CreateButton("Login", 8, () =>
        {
            var email = emailInput.Text.ToString() ?? string.Empty;
            var password = passwordInput.Text.ToString() ?? string.Empty;

            if (!ValidateInput(email, emailInput, "Email") || !ValidateInput(password, passwordInput, "Password"))
            {
                return;
            }

            try
            {
                AuthService.Login(email, password);
                router.Navigate("/home");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.ErrorQuery("Error", ex.Message, "OK");
            }
        });

        win.Add(emailLabel, emailInput);
        win.Add(passwordLabel, passwordInput);
        win.Add(loginButton);

        emailInput.SetFocus();

        return win;
    }
}