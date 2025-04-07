namespace project2.Models;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public int Quantity { get; set; }

    public int CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public Product() { }

    public Product(string name, string? description, double price, int quantity)
    {
        Name = name;
        Description = description;
        Price = price;
        Quantity = quantity;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Price: ${Price:F2}, Quantity: {Quantity}, Category: {Category?.Name}";
    }
}