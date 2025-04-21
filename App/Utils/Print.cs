namespace App.Utils
{
    using Spectre.Console;
    using System;
    using System.Collections.Generic;

    public static class Print
    {
        
        public static void Pause()
        {
            OutLine("Press any key to continue...", ConsoleColor.Yellow);
            Console.ReadKey();
        }

        
        public static void OutLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }

        
        public static void Out(string message, ConsoleColor color = ConsoleColor.White)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = originalColor;
        }

        
        public static bool AskYesNo(string message, bool defaultYes = true)
        {
            Out(message + (defaultYes ? " [Y/n]" : " [y/N]"), ConsoleColor.Cyan);
            Console.Write("> ");
            var input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrEmpty(input)) return defaultYes;

            return input == "y" || input == "yes";
        }

        
        public static string Ask(string message, ConsoleColor color = ConsoleColor.Cyan)
        {
            Out(message, color);
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        
        public static int AskNumber(string message, int? min = null, int? max = null)
        {
            while (true)
            {
                var rangeMsg = (min != null || max != null)
                    ? $" ({min ?? int.MinValue} - {max ?? int.MaxValue})"
                    : "";

                Out($"{message}{rangeMsg}", ConsoleColor.Cyan);
                var input = Console.ReadLine();

                if (int.TryParse(input, out int number))
                {
                    if ((min == null || number >= min) && (max == null || number <= max))
                        return number;
                }

                OutLine("Invalid number. Please enter a valid number.", ConsoleColor.Red);
            }
        }

        
        public static string AskChoice(string prompt, List<string> options)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(prompt)
                    .AddChoices(options)
            );

            return choice;
        }

        
        public static bool AskConfirmation(string message, string confirmText = "Confirm", string cancelText = "Cancel", bool defaultConfirm = true)
        {
            OutLine(message, ConsoleColor.Cyan);

            var choice = AskChoice($"Choose an option: [{confirmText}/{cancelText}]", new List<string> { confirmText, cancelText });

            return choice.Equals(confirmText, StringComparison.OrdinalIgnoreCase) ? true : false;
        }

        
        public static void PrintHeader(string header, ConsoleColor color = ConsoleColor.Green)
        {
            var border = new string('*', header.Length + 4);
            OutLine(border, color);
            OutLine($"* {header} *", color);
            OutLine(border, color);
        }

        
        public static void OutMultiLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            var lines = message.Split('\n');
            foreach (var line in lines)
            {
                OutLine(line, color);
            }
        }
    }
}
