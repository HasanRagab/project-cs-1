using App.Data;
using App.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace App.Services
{
    public static class WarehouseService
    {
        private static readonly AppDbContext _dbContext = DbContextManager.GetDbContext();

        

        public static void AddProduct(Product product)
        {
            if (_dbContext.Products.Any(p => p.Name == product.Name))
                throw new InvalidOperationException("A product with this name already exists.");
            if (product.Price <= 0)
                throw new ArgumentException("Price must be a positive number.");
            if (product.Stock < 0)
                throw new ArgumentException("Stock cannot be negative.");
            if (!_dbContext.Categories.Any(c => c.Id == product.CategoryId))
                throw new ArgumentException($"Category with ID {product.CategoryId} does not exist.");
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
        }

        public static void UpdateProduct(int id, Product product)
        {
            var existingProduct = _dbContext.Products.Find(id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.CategoryId = product.CategoryId;
                _dbContext.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Product not found.");
            }
        }

        public static void DeleteProduct(int productId)
        {
            var product = _dbContext.Products.Find(productId);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                _dbContext.SaveChanges();
            }
        }

        public static void RestockProduct(int productId, int quantity)
        {
            var product = _dbContext.Products.Find(productId);
            if (product != null)
            {
                product.Stock += quantity;
                _dbContext.SaveChanges();
            }
        }

        

        public static void AddCategory(string name)
        {
            if (_dbContext.Categories.Any(c => c.Name == name))
            {
                throw new InvalidOperationException("A category with this name already exists.");
            }
            var category = new Category(name);
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
        }

        public static void UpdateCategory(int categoryId, string name)
        {
            var category = _dbContext.Categories.Find(categoryId);
            if (category != null)
            {
                category.Name = name;
                _dbContext.SaveChanges();
            }
        }

        public static void DeleteCategory(int categoryId)
        {
            var category = _dbContext.Categories.Find(categoryId);
            if (category != null)
            {
                _dbContext.Categories.Remove(category);
                _dbContext.SaveChanges();
            }
        }

        

        public static List<User> GetAllUsers()
        {
            return _dbContext.Users.ToList();
        }

        public static void SetUserAsAdmin(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            if (user != null)
            {
                user.IsAdmin = true;
                _dbContext.SaveChanges();
            }
        }

        public static void RemoveAdminRights(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            if (user != null)
            {
                user.IsAdmin = false;
                _dbContext.SaveChanges();
            }
        }

        

        public static List<Order> GetAllOrders()
        {
            return _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }

        public static Order? GetOrderById(int orderId)
        {
            return _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.Id == orderId);
        }

        
        public static List<Order> GetOrdersByUser(int userId)
        {
            return _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }

        

        public static decimal GetTotalSales()
        {
            return _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Sum(o => o.Items.Sum(i => i.Quantity * i.UnitPrice));
        }

        public static decimal GetCategorySales(int categoryId)
        {
            return _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Items.Any(i => i.Product != null && i.Product.CategoryId == categoryId))  
                .Sum(o => o.Items
                    .Where(i => i.Product != null && i.Product.CategoryId == categoryId)  
                    .Sum(i => i.Quantity * i.UnitPrice));
        }

        public static decimal GetProductSales(int productId)
        {
            return _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.Items.Any(i => i.ProductId == productId))
                .Sum(o => o.Items
                    .Where(i => i.ProductId == productId)
                    .Sum(i => i.Quantity * i.UnitPrice));
        }

        public static List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return _dbContext.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }
    }
}
