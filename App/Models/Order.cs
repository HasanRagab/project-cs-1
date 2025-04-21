namespace App.Models;
using System.ComponentModel.DataAnnotations;



public class Order
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public virtual User? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public override string ToString()
    {
        return $"Order(id:{Id}, userId:{UserId})";
    }
}

public class OrderItem
{
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }
    public virtual Order? Order { get; set; }

    [Required]
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public int UnitPrice { get; set; }
}