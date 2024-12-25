using project1.Pages;
using Spectre.Console;


static void Intro() {
    AnsiConsole.MarkupLine("[bold #14b898]Home Page :[/]");
    AnsiConsole.MarkupLine("  [red] exit [/],[yellow] clear [/]");
    AnsiConsole.MarkupLine("[#3ffb46] 1.program [/]");
    AnsiConsole.MarkupLine("[#f9cf00] 2.calculate [/]");
    AnsiConsole.MarkupLine("[#fb3c44] 3.history [/]");
}

var prompt = new TextPrompt<string>("[bold yellow]$>>[/]")
    .AddChoices(new List<string> {"1", "2", "3", "exit", "clear" })
    .HideChoices();

Intro();

while (true)
{
    var input = AnsiConsole.Prompt(prompt);
    if (input == "exit")
    {
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
            ShapesCalc.Start();
            Intro();
            break;
        case "2":
            Console.Clear();
            Calculator.Start();
            Intro();
            break;
        case "3":
            AnsiConsole.MarkupLine("[#f9cf00]calculate history[/]");
            Calculator.InputHistory.DisplayHistory();
            AnsiConsole.MarkupLine("[#3ffb46]program history[/]");
            ShapesCalc.InputHistory.DisplayHistory();
            break;
    }
}

