namespace project2.Pages;

using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using System.Text.RegularExpressions;
using project2.Models;
using project2.Services;

public class Buyer
{
    private static readonly ProductService _productService = new ProductService();

    private static void Intro()
    {
        const string title = "<< Buyer >>";

        AnsiConsole.MarkupLine($"[bold #f9cf00]{title}\n{new string('=', title.Length)}[/]");
    }

    public static void Start()
    {
        var shoppingCart = new Dictionary<int, int>();

        while (true)
        {
            Console.Clear();
            Intro();

            var commandChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Select an option[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                    "List Products",
                    "List Categories",
                    "Search Products",
                    "View Category",
                    "Buy Product",
                    "View Cart",
                    "Checkout",
                    "Clear",
                    "Exit"
                    }));

            switch (commandChoice)
            {
                case "List Products":
                    ListProducts();
                    break;

                case "List Categories":
                    ListCategories();
                    break;

                case "Search Products":
                    var searchTerm = AnsiConsole.Ask<string>("[green]Enter search term:[/]");
                    SearchProducts(searchTerm);
                    break;

                case "View Category":
                    var catId = AnsiConsole.Ask<int>("[green]Enter category ID:[/]");
                    ViewCategory(catId);
                    break;

                case "Buy Product":
                    var productId = AnsiConsole.Ask<int>("[green]Enter product ID:[/]");
                    var quantity = AnsiConsole.Ask<int>("[green]Enter quantity:[/]");
                    BuyProduct(productId, quantity, shoppingCart);
                    break;

                case "View Cart":
                    ViewCart(shoppingCart);
                    break;

                case "Checkout":
                    Checkout(shoppingCart);
                    shoppingCart.Clear();
                    break;

                case "Clear":
                    Console.Clear();
                    Intro();
                    break;

                case "Exit":
                    Console.Clear();
                    return;

                default:
                    AnsiConsole.MarkupLine("[red]Unknown selection[/]");
                    break;
            }

            AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }
    }

    private static void ListProducts()
    {
        var products = _productService.GetAllProducts();

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Category");
        table.AddColumn("Price");
        table.AddColumn("In Stock");

        foreach (var product in products)
        {
            table.AddRow(
                product.Id.ToString(),
                product.Name,
                product.Category?.Name ?? "Unknown",
                $"${product.Price:F2}",
                product.Quantity.ToString()
            );
        }

        AnsiConsole.Write(table);
    }

    private static void ListCategories()
    {
        var categories = _productService.GetAllCategories();

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Description");

        foreach (var category in categories)
        {
            table.AddRow(
                category.Id.ToString(),
                category.Name,
                category.Description ?? ""
            );
        }

        AnsiConsole.Write(table);
    }

    private static void SearchProducts(string searchTerm)
    {
        var products = _productService.SearchProducts(searchTerm);

        if (products.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]No products found matching '{searchTerm}'[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Found {products.Count} products matching '{searchTerm}':[/]");

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Category");
        table.AddColumn("Price");
        table.AddColumn("In Stock");

        foreach (var product in products)
        {
            table.AddRow(
                product.Id.ToString(),
                product.Name,
                product.Category?.Name ?? "Unknown",
                $"${product.Price:F2}",
                product.Quantity.ToString()
            );
        }

        AnsiConsole.Write(table);
    }

    private static void ViewCategory(int categoryId)
    {
        var category = _productService.GetCategoryById(categoryId);

        if (category == null)
        {
            AnsiConsole.MarkupLine($"[red]Category with ID {categoryId} not found[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Products in category '{category.Name}':[/]");

        var products = _productService.GetProductsByCategory(categoryId);

        if (products.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No products in this category[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Price");
        table.AddColumn("In Stock");

        foreach (var product in products)
        {
            table.AddRow(
                product.Id.ToString(),
                product.Name,
                $"${product.Price:F2}",
                product.Quantity.ToString()
            );
        }

        AnsiConsole.Write(table);
    }

    private static void BuyProduct(int productId, int quantity, Dictionary<int, int> cart)
    {
        var product = _productService.GetProductById(productId);

        if (product == null)
        {
            AnsiConsole.MarkupLine($"[red]Product with ID {productId} not found[/]");
            return;
        }

        if (product.Quantity < quantity)
        {
            AnsiConsole.MarkupLine($"[red]Not enough stock! Only {product.Quantity} available.[/]");
            return;
        }

        if (cart.ContainsKey(productId))
        {
            cart[productId] += quantity;
        }
        else
        {
            cart[productId] = quantity;
        }

        AnsiConsole.MarkupLine($"[green]Added {quantity} x {product.Name} to your cart[/]");
    }

    private static void ViewCart(Dictionary<int, int> cart)
    {
        if (cart.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Your shopping cart is empty[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("Product");
        table.AddColumn("Quantity");
        table.AddColumn("Price");
        table.AddColumn("Total");

        double totalAmount = 0;

        foreach (var item in cart)
        {
            var product = _productService.GetProductById(item.Key);
            if (product != null)
            {
                double itemTotal = product.Price * item.Value;
                totalAmount += itemTotal;

                table.AddRow(
                    product.Name,
                    item.Value.ToString(),
                    $"${product.Price:F2}",
                    $"${itemTotal:F2}"
                );
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[bold]Total: ${totalAmount:F2}[/]");
    }

    private static void Checkout(Dictionary<int, int> cart)
    {
        if (cart.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Your shopping cart is empty[/]");
            return;
        }

        double totalAmount = 0;
        bool success = true;

        foreach (var item in cart)
        {
            var product = _productService.GetProductById(item.Key);
            if (product != null)
            {
                if (product.Quantity < item.Value)
                {
                    AnsiConsole.MarkupLine($"[red]Not enough stock for {product.Name}! Only {product.Quantity} available.[/]");
                    success = false;
                    break;
                }

                totalAmount += product.Price * item.Value;

                _productService.UpdateProduct(
                    product.Id,
                    product.Name,
                    product.Description ?? string.Empty,
                    product.Price,
                    product.Quantity - item.Value,
                    product.CategoryId
                );
            }
        }

        if (success)
        {
            string orderId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            AnsiConsole.MarkupLine($"[green]Order {orderId} completed successfully![/]");
            AnsiConsole.MarkupLine($"[green]Total amount: ${totalAmount:F2}[/]");
            AnsiConsole.MarkupLine("[green]Thank you for your purchase![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Checkout failed. Please check your cart and try again.[/]");
        }
    }
}