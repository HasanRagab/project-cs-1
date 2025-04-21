namespace App.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public User() { }

    public User(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public bool IsAdmin { get; set; } = false;

    public virtual Cart? Cart { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public override string ToString() => $"User(id: {Id}, email: {Email})";
}
