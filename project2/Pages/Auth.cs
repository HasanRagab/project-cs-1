namespace project2.Pages;

using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using System.Text.RegularExpressions;
using project2.Models;
using project2.Services;

public class Auth
{
    private static readonly UserService _userService = new UserService(new ProductContext());
    private static readonly ProductService _productService = new ProductService();
    public static string? currentUserEmail;
    private static string? _currentUserPassword;

    private static void Intro()
    {
        const string title = "<< Auth >>";

        AnsiConsole.MarkupLine($"[bold #f9cf00]{title}\n{new string('=', title.Length)}[/]");
    }


    public static void Start()
    {
        while (true)
        {
            Console.Clear();

            if (currentUserEmail != null) return;

            Intro();

            var commandChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Select an option[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Register",
                        "Login",
                        "Logout",
                        "View Profile",
                        "Clear",
                    }));

            switch (commandChoice)
            {
                case "Register":
                    Register();
                    break;

                case "Login":
                    Login();
                    break;

                case "Logout":
                    Logout();
                    break;

                case "View Profile":
                    ViewProfile();
                    break;

                case "Clear":
                    Console.Clear();
                    break;
            }

            AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }
    }

    private static void Register()
    {
        var email = AnsiConsole.Ask<string>("[green]Enter your email:[/]");
        var password = AnsiConsole.Ask<string>("[green]Enter your password:[/]");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            AnsiConsole.MarkupLine("[red]Email and password cannot be empty.[/]");
            return;
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            AnsiConsole.MarkupLine("[red]Invalid email format.[/]");
            Register();
            return;
        }

        var user = _userService.CreateUser(email, password);
        AnsiConsole.MarkupLine($"[green]User {user.Email} registered successfully![/]");
        System.Threading.Thread.Sleep(1000);
        Console.Clear();
        Login();
        return;
    }
    private static void Login()
    {
        var email = AnsiConsole.Ask<string>("[green]Enter your email:[/]");
        var password = AnsiConsole.Ask<string>("[green]Enter your password:[/]");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            AnsiConsole.MarkupLine("[red]Email and password cannot be empty.[/]");
            return;
        }

        var user = _userService.GetUserByEmail(email);
        if (user == null || user.Password != password)
        {
            AnsiConsole.MarkupLine("[red]Invalid email or password.[/]");
            var registerChoice = AnsiConsole.Confirm("[green]Do you want to register?[/]");
            Console.Clear();
            if (registerChoice) Register();
            Console.Clear();
            var loginChoice = AnsiConsole.Confirm("[green]Do you want to try logging in again?[/]");
            if (loginChoice) Login();
            return;
        }

        currentUserEmail = user.Email;
        _currentUserPassword = user.Password;
        AnsiConsole.MarkupLine($"[green]Welcome, {currentUserEmail}![/]");
        return;
    }

    public static void Logout()
    {
        currentUserEmail = null;
        _currentUserPassword = null;
        AnsiConsole.MarkupLine("[green]Logged out successfully![/]");
    }
    private static void ViewProfile()
    {
        if (currentUserEmail == null)
        {
            AnsiConsole.MarkupLine("[red]You need to log in first.[/]");
            return;
        }

        var user = _userService.GetUserByEmail(currentUserEmail);
        if (user != null)
        {
            AnsiConsole.MarkupLine($"[green]User Profile:[/]");
            AnsiConsole.MarkupLine($"[green]Email: {user.Email}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]User not found.[/]");
        }
    }
}