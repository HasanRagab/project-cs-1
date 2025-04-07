using project2.Pages;
using project2.Services;
using Spectre.Console;


static void Intro() {
    AnsiConsole.MarkupLine("[bold #14b898]Home Page :[/]");
    AnsiConsole.MarkupLine("  [red] exit [/],[yellow] clear [/]");
    AnsiConsole.MarkupLine("[#3ffb46] 1. Buyer [/]");
    AnsiConsole.MarkupLine("[#f9cf00] 2. Seller [/]");
}

var prompt = new TextPrompt<string>("[bold yellow]$>>[/]")
    .AddChoices(["1", "2", "3", "exit", "clear"])
    .HideChoices();

Auth.Start();
Intro();
while (true)
{
    var input = AnsiConsole.Prompt(prompt);
    
    if (input == "exit")
    {
        Auth.Logout();
        break;
    }
    switch (input)
    {   
        case "clear":
            Console.Clear();
            Intro();
            break;
        case "1":
            Console.Clear();
            Buyer.Start();
            Intro();
            break;
        case "2":
            Console.Clear();
            Seller.Start();
            Intro();
            break;
        default:
            AnsiConsole.MarkupLine("[red]Invalid input. Please try again.[/]");
            break;
    }
}

