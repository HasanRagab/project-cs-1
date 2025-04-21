namespace App.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Index(nameof(Name), IsUnique = true)]
public class Product
{
    public Product() {} 
    public Product(string name, int price, int stock, int categoryId)
    {
        Name = name;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
    }
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int Price { get; set; }

    [Required]
    public int Stock { get; set; }

    [ForeignKey("Category")]
    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public override string ToString() => $"Product(id:{Id}, name:{Name})";
}

[Index(nameof(Name), IsUnique = true)]
public class Category
{
    public Category() {}
    public Category(string name)
    {
        Name = name;
    }
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public override string ToString() => $"Category(id:{Id}, name:{Name})";
}

