namespace project2.Pages;

using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using System.Text.RegularExpressions;
using project2.Models;
using project2.Services;

public class Seller
{
    private static readonly ProductService _productService = new ProductService();

    private static void Intro()
    {
        const string title = "<< Seller >>";
        
        AnsiConsole.MarkupLine($"[bold #3498db]{title}\n{new string('=', title.Length)}[/]");
    }

    public static void Start()
    {
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
                        "Inventory",
                        "Categories",
                        "Add Product",
                        "Update Product",
                        "Delete Product",
                        "Add Category",
                        "Low Stock",
                        "Restock",
                        "View Product",
                        "Search Products",
                        "Low Stock",
                        "Clear",
                        "Exit"
                    }));

            switch (commandChoice)
            {
                case "Inventory":
                    ListInventory();
                    break;

                case "Categories":
                    ListCategories();
                    break;

                case "Add Product":
                    AddProduct();
                    break;

                case "Update Product":
                    int updateId = AnsiConsole.Ask<int>("[green]Enter product ID to update:[/]");
                    UpdateProduct(updateId);
                    break;

                case "Delete Product":
                    int deleteId = AnsiConsole.Ask<int>("[green]Enter product ID to delete:[/]");
                    DeleteProduct(deleteId);
                    break;

                case "Add Category":
                    AddCategory();
                    break;

                case "Low Stock":
                    ShowLowStock();
                    break;

                case "Restock":
                    int restockId = AnsiConsole.Ask<int>("[green]Enter product ID to restock:[/]");
                    int quantity = AnsiConsole.Ask<int>("[green]Enter quantity to add:[/]");
                    RestockProduct(restockId, quantity);
                    break;

                case "View Product":
                    int viewId = AnsiConsole.Ask<int>("[green]Enter product ID to view details:[/]");
                    ViewProductDetails(viewId);
                    break;

                case "Search Products":
                    string searchTerm = AnsiConsole.Ask<string>("[green]Enter search term:[/]");
                    SearchProducts(searchTerm);
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
    
    private static void ListInventory()
    {
        var products = _productService.GetAllProducts();
        
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Category");
        table.AddColumn("Price");
        table.AddColumn("Stock");
        table.AddColumn("Status");
        
        foreach (var product in products)
        {
            string status = product.Quantity > 10 ? "[green]Good[/]" : 
                           (product.Quantity > 5 ? "[yellow]Medium[/]" : "[red]Low[/]");
            
            table.AddRow(
                product.Id.ToString(),
                product.Name,
                product.Category?.Name ?? "Unknown",
                $"${product.Price:F2}",
                product.Quantity.ToString(),
                status
            );
        }
        
        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[blue]Total Products: {products.Count}[/]");
    }
    
    private static void ListCategories()
    {
        var categories = _productService.GetAllCategories();
        
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Description");
        table.AddColumn("Product Count");
        
        foreach (var category in categories)
        {
            int productCount = category.Products?.Count ?? 0;
            
            table.AddRow(
                category.Id.ToString(),
                category.Name,
                category.Description ?? "",
                productCount.ToString()
            );
        }
        
        AnsiConsole.Write(table);
    }
    
    private static void AddProduct()
    {
        AnsiConsole.MarkupLine("[bold]Add New Product[/]");
        
        var categories = _productService.GetAllCategories();
        if (categories.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No categories available. Please add a category first.[/]");
            return;
        }
        
        AnsiConsole.MarkupLine("[yellow]Available Categories:[/]");
        foreach (var category in categories)
        {
            AnsiConsole.MarkupLine($"  {category.Id}: {category.Name}");
        }
        
        string name = AnsiConsole.Ask<string>("[bold]Name:[/]");
        string description = AnsiConsole.Ask<string>("[bold]Description:[/]");
        double price = AnsiConsole.Ask<double>("[bold]Price ($):[/]");
        int quantity = AnsiConsole.Ask<int>("[bold]Initial Stock Quantity:[/]");
        int categoryId = AnsiConsole.Ask<int>("[bold]Category ID:[/]");
        
        var selectedCategory = categories.FirstOrDefault(c => c.Id == categoryId);
        if (selectedCategory == null)
        {
            AnsiConsole.MarkupLine("[red]Invalid category ID. Product not added.[/]");
            return;
        }
        
        var newProduct = _productService.CreateProduct(name, description, price, quantity, categoryId);
        
        AnsiConsole.MarkupLine($"[green]Product added successfully! Product ID: {newProduct.Id}[/]");
    }
    
    private static void UpdateProduct(int productId)
    {
        var product = _productService.GetProductById(productId);
        
        if (product == null)
        {
            AnsiConsole.MarkupLine($"[red]Product with ID {productId} not found[/]");
            return;
        }
        
        AnsiConsole.MarkupLine($"[bold]Updating Product: {product.Name} (ID: {product.Id})[/]");
        AnsiConsole.MarkupLine("[grey]Press Enter to keep current values[/]");
        
        string nameInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"[bold]Name[/] [grey]Current: {product.Name}[/]: ")
                .AllowEmpty());

        string descriptionInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"[bold]Description[/] [grey]Current: {product.Description}[/]: ")
                .AllowEmpty());
        
        string priceInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"[bold]Price ($)[/] [grey]Current: {product.Price:F2}[/]: ")
                .AllowEmpty());
        
        string quantityInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"[bold]Stock Quantity[/] [grey]Current: {product.Quantity}[/]: ")
                .AllowEmpty());
        
        var categories = _productService.GetAllCategories();
        AnsiConsole.MarkupLine("[yellow]Available Categories:[/]");
        foreach (var category in categories)
        {
            AnsiConsole.MarkupLine($"  {category.Id}: {category.Name}");
        }
        
        string categoryIdInput = AnsiConsole.Prompt(
            new TextPrompt<string>($"[bold]Category ID[/] [grey]Current: {product.CategoryId}[/]: ")
                .AllowEmpty());
        
        string name = string.IsNullOrEmpty(nameInput) ? product.Name : nameInput;
        string? description = string.IsNullOrEmpty(descriptionInput) ? product.Description : descriptionInput;
        double price = string.IsNullOrEmpty(priceInput) ? product.Price : double.Parse(priceInput);
        int quantity = string.IsNullOrEmpty(quantityInput) ? product.Quantity : int.Parse(quantityInput);
        int categoryId = string.IsNullOrEmpty(categoryIdInput) ? product.CategoryId : int.Parse(categoryIdInput);
        
        if (categoryId != product.CategoryId)
        {
            var selectedCategory = categories.FirstOrDefault(c => c.Id == categoryId);
            if (selectedCategory == null)
            {
                AnsiConsole.MarkupLine("[red]Invalid category ID. Using original category.[/]");
                categoryId = product.CategoryId;
            }
        }
        
        bool success = _productService.UpdateProduct(productId, name, description ?? string.Empty, price, quantity, categoryId);

        if (success)
        {
            AnsiConsole.MarkupLine("[green]Product updated successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to update product.[/]");
        }
    }
    
    private static void DeleteProduct(int productId)
    {
        var product = _productService.GetProductById(productId);
        
        if (product == null)
        {
            AnsiConsole.MarkupLine($"[red]Product with ID {productId} not found[/]");
            return;
        }
        
        var confirm = AnsiConsole.Confirm(
            $"Are you sure you want to delete '{product.Name}' (ID: {product.Id})?", 
            false);
        
        if (!confirm)
        {
            AnsiConsole.MarkupLine("[yellow]Deletion cancelled.[/]");
            return;
        }
        
        bool success = _productService.DeleteProduct(productId);
        
        if (success)
        {
            AnsiConsole.MarkupLine("[green]Product deleted successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to delete product.[/]");
        }
    }
    
    private static void AddCategory()
    {
        AnsiConsole.MarkupLine("[bold]Add New Category[/]");
        
        string name = AnsiConsole.Ask<string>("[bold]Name:[/]");
        string description = AnsiConsole.Ask<string>("[bold]Description:[/]");
        
        var newCategory = _productService.CreateCategory(name, description);
        
        AnsiConsole.MarkupLine($"[green]Category added successfully! Category ID: {newCategory.Id}[/]");
    }
    
    private static void ShowLowStock()
    {
        var products = _productService.GetAllProducts()
            .Where(p => p.Quantity <= 5)
            .OrderBy(p => p.Quantity)
            .ToList();
        
        if (products.Count == 0)
        {
            AnsiConsole.MarkupLine("[green]No products with low stock.[/]");
            return;
        }
        
        AnsiConsole.MarkupLine($"[red]Found {products.Count} products with low stock:[/]");
        
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Category");
        table.AddColumn("Price");
        table.AddColumn("Stock");
        
        foreach (var product in products)
        {
            table.AddRow(
                product.Id.ToString(),
                product.Name,
                product.Category?.Name ?? "Unknown",
                $"${product.Price:F2}",
                $"[red]{product.Quantity}[/]"
            );
        }
        
        AnsiConsole.Write(table);
    }
    
    private static void RestockProduct(int productId, int quantity)
    {
        var product = _productService.GetProductById(productId);
        
        if (product == null)
        {
            AnsiConsole.MarkupLine($"[red]Product with ID {productId} not found[/]");
            return;
        }
        
        int newQuantity = product.Quantity + quantity;
        
        bool success = _productService.UpdateProduct(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            newQuantity,
            product.CategoryId
        );
        
        if (success)
        {
            AnsiConsole.MarkupLine($"[green]Successfully restocked {product.Name}[/]");
            AnsiConsole.MarkupLine($"[green]Previous quantity: {product.Quantity}, New quantity: {newQuantity}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to restock product.[/]");
        }
    }
    
    private static void ViewProductDetails(int productId)
    {
        var product = _productService.GetProductById(productId);
        
        if (product == null)
        {
            AnsiConsole.MarkupLine($"[red]Product with ID {productId} not found[/]");
            return;
        }
        
        var panel = new Panel($"""
            [bold]Product Details:[/]
            
            [bold]ID:[/] {product.Id}
            [bold]Name:[/] {product.Name}
            [bold]Description:[/] {product.Description}
            [bold]Price:[/] ${product.Price:F2}
            [bold]Current Stock:[/] {product.Quantity}
            [bold]Category:[/] {product.Category?.Name ?? "Unknown"} (ID: {product.CategoryId})
            """);
        
        panel.Border = BoxBorder.Rounded;
        panel.Expand = true;
        
        AnsiConsole.Write(panel);
    }
}