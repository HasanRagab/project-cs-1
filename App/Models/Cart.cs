namespace App.Models;

using System.ComponentModel.DataAnnotations;

public class Cart
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public virtual User? User { get; set; }

    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    public Cart() { }
}

public class CartItem
{
    public int Id { get; set; }

    [Required]
    public int CartId { get; set; }
    public virtual Cart? Cart { get; set; }

    [Required]
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }

    [Required]
    public int Quantity { get; set; }
    public CartItem() { }
}