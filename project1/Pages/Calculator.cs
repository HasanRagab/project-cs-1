namespace project1.Pages;
using System;
using System.Collections.Generic;
using Spectre.Console;
using System.Text.RegularExpressions;
using Store;
 public class Calculator 
 {
        public static readonly History InputHistory = new History();

        private static void Intro()
        {
            const string title = "<< Calculator >>";
             AnsiConsole.MarkupLine($"[bold #f9cf00]{title}\n{new string('=', title.Length)}[/]");
             AnsiConsole.MarkupLine("[#329832]Usage Example: 3+ 8-2 /32 * 2[/]");
             AnsiConsole.MarkupLine("[yellow]Available Commands:[/]");
             AnsiConsole.MarkupLine("  [red]exit[/] - Exit the program");
             AnsiConsole.MarkupLine("  [yellow]clear[/] - Clear the console");
             AnsiConsole.MarkupLine("  [yellow]history[/] - View command history\n");
         }
        public static void Start()
        {
            Intro();
            var prompt = new TextPrompt<string>("[bold yellow]$>>[/]");

            while (true)
            {
                var input = AnsiConsole.Prompt(prompt);

                if (string.Equals(input.ToLower(), "exit"))
                {
                    Console.Clear();
                    break;
                }

                if (string.Equals(input.ToLower(), "clear"))
                {
                    Console.Clear();
                    Intro();
                    continue;
                }

                if (string.Equals(input.ToLower(), "history"))
                {
                    InputHistory.DisplayHistory();
                    continue;
                }

                try
                {
                    if (string.IsNullOrEmpty(input))
                    {
                        throw new ArgumentNullException(nameof(input), "Input cannot be null.");
                    }
                    var result = EvaluateExpression(input);
                    InputHistory.AddInput($"{input} = {result}");
                    AnsiConsole.MarkupLine($"[bold blue]Result: {result}[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[bold red]Error: {ex.Message}[/]");
                }
            }
        }

        private static double EvaluateExpression(string expression)
        {
            expression = expression.Replace(" ", "");
            return PerformCalculation(expression);
        }

        private static double PerformCalculation(string expression)
        {
            var terms = new List<string>();
            var operators = new List<string>();
            var matches = Regex.Matches(expression, @"(\d+(\.\d+)?)|([+\-*/^])");

            foreach (Match match in matches)
            {
                if (match.Groups[1].Success)
                {
                    terms.Add(match.Groups[1].Value);
                }
                else if (match.Groups[3].Success)
                {
                    operators.Add(match.Groups[3].Value);
                }
            }

            HandleOperator(ref terms, ref operators, new[] { "*", "/", "^" });
            HandleOperator(ref terms, ref operators, new[] { "+", "-" });

            if (terms.Count != 1)
            {
                throw new ArgumentException("Invalid expression format.");
            }

            return double.Parse(terms[0]);
        }

        private static void HandleOperator(ref List<string> terms, ref List<string> operators, string[] validOperators)
        {
            var updatedTerms = new List<string>(terms);
            var updatedOperators = new List<string>(operators);

            for (var i = 0; i < updatedOperators.Count;)
            {
                if (Array.Exists(validOperators, op => op == updatedOperators[i]))
                {
                    var operand1 = double.Parse(updatedTerms[i]);
                    var operand2 = double.Parse(updatedTerms[i + 1]);
                    var result = PerformOperation(operand1, updatedOperators[i], operand2);

                    updatedTerms[i] = result.ToString("F2");
                    updatedTerms.RemoveAt(i + 1); 
                    updatedOperators.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            terms = updatedTerms;
            operators = updatedOperators;
        }

        private static double PerformOperation(double operand1, string operation, double operand2)
        {
            switch (operation)
            {
                case "+":
                    return operand1 + operand2;
                case "-":
                    return operand1 - operand2;
                case "*":
                    return operand1 * operand2;
                case "/":
                    if (operand2 == 0)
                    {
                        throw new ArgumentException("Math error: Division by zero");
                    }
                    return operand1 / operand2;
                case "^":
                    return Math.Pow(operand1, operand2);
                default:
                    throw new ArgumentException("Invalid operation. Use +, -, *, /, or ^.");
            }
        }
    }