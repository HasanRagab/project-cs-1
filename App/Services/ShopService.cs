using App.Data;
using App.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace App.Services;

public static class ShopService
{
    private static readonly AppDbContext _dbContext = DbContextManager.GetDbContext();



    public static List<Product> GetAllProducts()
    {
        return [.. _dbContext.Products];
    }

    public static List<string> GetAllCategories()
    {
        return [.. _dbContext.Categories.Select(c => c.Name).Distinct()];
    }

    public static List<Category> GetAllCategoriesData()
    {
        return [.. _dbContext.Categories];
    }

    public static List<Product> SearchProducts(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return [.. _dbContext.Products];
        return [.. _dbContext.Products.Where(p => p.Name.Contains(keyword) || (p.Category != null && p.Category.Name.Contains(keyword)))];
    }


    public static List<Product> FilterByCategory(string categoryName)
    {
        return [.. _dbContext.Products.Where(p => p.Category != null && p.Category.Name == categoryName)];
    }

    public static Product? GetProductById(int id)
    {
        return _dbContext.Products.FirstOrDefault(p => p.Id == id);
    }

    public static Category? GetCategoryId(string categoryName)
    {
        return _dbContext.Categories.Where(c => c.Name == categoryName).FirstOrDefault();
    }

    public static List<int> GetAllCategoryIds()
    {
        return [.. _dbContext.Categories.Select(c => c.Id)];
    }



    public static Cart GetOrCreateCart(int userId)
    {
        var cart = _dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _dbContext.Carts.Add(cart);
            _dbContext.SaveChanges();
        }

        return cart;
    }

    public static void AddToCart(int userId, int productId, int quantity = 1)
    {
        var cart = GetOrCreateCart(userId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            item.Quantity += quantity;
        }
        else
        {
            item = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity
            };
            cart.Items.Add(item);
        }

        _dbContext.SaveChanges();
    }

    public static void RemoveFromCart(int userId, int productId)
    {
        var cart = GetOrCreateCart(userId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            _dbContext.CartItems.Remove(item);
            _dbContext.SaveChanges();
        }
    }

    public static void UpdateQuantity(int userId, int productId, int quantity)
    {
        var cart = GetOrCreateCart(userId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Quantity = quantity;
            _dbContext.SaveChanges();
        }
    }

    public static List<CartItem> GetCartItems(int userId)
    {
        return GetOrCreateCart(userId).Items.ToList();
    }

    public static void ClearCart(int userId)
    {
        var cart = GetOrCreateCart(userId);
        _dbContext.CartItems.RemoveRange(cart.Items);
        cart.Items.Clear();
        _dbContext.SaveChanges();
    }

    public static decimal GetCartTotal(int userId)
    {
        var cart = GetOrCreateCart(userId);
        return cart.Items
            .Where(i => i.Product != null)
            .Sum(i => i.Quantity * i.Product!.Price);
    }



    public static Order PlaceOrder(int userId)
    {
        var cart = GetOrCreateCart(userId);

        if (cart.Items.Count == 0)
        {
            throw new InvalidOperationException("Cart is empty.");
        }

        var order = new Order
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Items = [.. cart.Items.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product?.Price ?? 0
            })]
        };

        foreach (var item in order.Items)
        {
            var product = _dbContext.Products.Find(item.ProductId);
            if (product != null && product.Stock >= item.Quantity)
            {
                product.Stock -= item.Quantity;
            }
            else
            {
                throw new InvalidOperationException($"Not enough stock for product: {product?.Name}");
            }
        }

        _dbContext.Orders.Add(order);

        _dbContext.CartItems.RemoveRange(cart.Items);
        cart.Items.Clear();

        _dbContext.SaveChanges();

        return order;
    }

    public static List<Order> GetUserOrders(int userId)
    {
        return [.. _dbContext.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)];
    }

    public static Order? GetOrderById(int orderId)
    {
        return _dbContext.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(o => o.Id == orderId);
    }
}
