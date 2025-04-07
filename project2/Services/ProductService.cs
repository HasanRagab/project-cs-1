namespace project2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using project2.Models;

public class ProductService
{
    private readonly ProductContext _context;

    public ProductService()
    {
        _context = new ProductContext();
        _context.Database.EnsureCreated();
    }

    public Product CreateProduct(string name, string description, double price, int quantity, int categoryId)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Quantity = quantity,
            CategoryId = categoryId
        };

        _context.Products.Add(product);
        _context.SaveChanges();
        return product;
    }

    public Category CreateCategory(string name, string description)
    {
        var category = new Category
        {
            Name = name,
            Description = description
        };

        _context.Categories.Add(category);
        _context.SaveChanges();
        return category;
    }

    public List<Product> GetAllProducts()
    {
        return _context.Products
            .Include(p => p.Category)
            .ToList();
    }

    public Product? GetProductById(int id)
    {
        return _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.Id == id);
    }

    public List<Product> GetProductsByCategory(int categoryId)
    {
        return _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToList();
    }

    public List<Category> GetAllCategories()
    {
        return _context.Categories.ToList();
    }

    public Category? GetCategoryById(int id)
    {
        return _context.Categories
            .Include(c => c.Products)
            .FirstOrDefault(c => c.Id == id);
    }

    public Category? GetCategoryByName(string name)
    {
        return _context.Categories
            .Include(c => c.Products)
            .FirstOrDefault(c => c.Name == name);
    }

    public bool UpdateProduct(int id, string name, string? description, double price, int quantity, int categoryId)
    {
        var product = _context.Products.Find(id);
        if (product == null)
            return false;

        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.Quantity = quantity;
        product.CategoryId = categoryId;

        _context.SaveChanges();
        return true;
    }

    public bool UpdateCategory(int id, string name, string description)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
            return false;

        category.Name = name;
        category.Description = description;

        _context.SaveChanges();
        return true;
    }

    public bool DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        _context.SaveChanges();
        return true;
    }

    public bool DeleteCategory(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
            return false;

        var hasProducts = _context.Products.Any(p => p.CategoryId == id);
        if (hasProducts)
            return false;

        _context.Categories.Remove(category);
        _context.SaveChanges();
        return true;
    }

    public List<Product> SearchProducts(string searchTerm)
    {
        return _context.Products
            .Include(p => p.Category)
            .Where(p => p.Name.Contains(searchTerm) || 
                        (p.Description != null && p.Description.Contains(searchTerm)))
            .ToList();
    }
}