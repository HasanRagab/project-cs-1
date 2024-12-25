namespace project1.Pages;

using System;
using Spectre.Console;
using Store;
using Models;

public class ShapesCalc
{
    public static readonly History InputHistory = new History();

    private static void Intro()
    {
        const string title = "<< Shapes Calculator >>";
        AnsiConsole.MarkupLine($"[bold #3ffb46]{title}\n{new string('=', title.Length)}\n[/]");
    }

    private static Shape? GetShapeFromChoice(string shapeChoice)
    {
        switch (shapeChoice)
        {
            case "Square":
                var sideLength = AnsiConsole.Ask<double>("Enter the side length: ");
                return new Square(sideLength);
            case "Triangle":
                var baseLength = AnsiConsole.Ask<double>("Enter the base length: ");
                var height = AnsiConsole.Ask<double>("Enter the height: ");
                return new Triangle(baseLength, height);
            case "Circle":
                var radius = AnsiConsole.Ask<double>("Enter the radius: ");
                return new Circle(radius);
            case "Rhombus":
                var diagonal1 = AnsiConsole.Ask<double>("Enter the first diagonal length: ");
                var diagonal2 = AnsiConsole.Ask<double>("Enter the second diagonal length: ");
                return new Rhombus(diagonal1, diagonal2);
            case "Hexagon":
                var hexagonSide = AnsiConsole.Ask<double>("Enter the side length: ");
                return new RegularHexagon(hexagonSide);
            case "Heptagon":
                var heptagonSide = AnsiConsole.Ask<double>("Enter the side length: ");
                return new RegularHeptagon(heptagonSide);
            case "Octagon":
                var octagonSide = AnsiConsole.Ask<double>("Enter the side length: ");
                return new RegularOctagon(octagonSide);
            case "Pentagon":
                var pentagonSide = AnsiConsole.Ask<double>("Enter the side length: ");
                return new RegularPentagon(pentagonSide);
            case "Cube":
                var cubeSide = AnsiConsole.Ask<double>("Enter the side length: ");
                return new Cube(cubeSide);
            case "Cylinder":
                var cylinderRadius = AnsiConsole.Ask<double>("Enter the radius: ");
                var heightCylinder = AnsiConsole.Ask<double>("Enter the height: ");
                return new Cylinder(cylinderRadius, heightCylinder);
            case "Cone":
                var coneRadius = AnsiConsole.Ask<double>("Enter the radius: ");
                var heightCone = AnsiConsole.Ask<double>("Enter the height: ");
                return new Cone(coneRadius, heightCone);
            case "Pyramid":
                var baseArea = AnsiConsole.Ask<double>("Enter the base area: ");
                var heightPyramid = AnsiConsole.Ask<double>("Enter the height: ");
                return new Pyramid(baseArea, heightPyramid);
            default:
                AnsiConsole.MarkupLine("[bold red]Invalid shape selection.[/]");
                return null;
        }
    }

    public static void Start()
    {
        Intro();
        while (true)
        {
            try
            {
                var shapeChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold yellow]Select a shape to calculate (or choose an option below):[/]")
                        .PageSize(10)
                        .AddChoices(new[]
                        {
                            "Square", "Triangle", "Circle", "Rhombus", 
                            "Hexagon", "Heptagon", "Octagon", "Pentagon", 
                            "Cube", "Cylinder", "Cone", "Pyramid", 
                            "Exit", "Clear", "History"
                        }));

                if (shapeChoice == "Clear")
                {
                    Console.Clear();
                    Intro();
                    continue;
                }
                
                if (shapeChoice == "Exit")
                {
                    Console.Clear();
                    break;
                }

                if (shapeChoice == "History")
                {
                    InputHistory.DisplayHistory();
                    continue;
                }
                var shape = GetShapeFromChoice(shapeChoice);
                if (shape == null) continue;
                InputHistory.AddInput($"{shape}");
                AnsiConsole.MarkupLine($"[bold green]Shape: {shapeChoice}[/]");
                shape.DisplayInfo();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]Error: {ex.Message}[/]");
            }
        }
    }
}
