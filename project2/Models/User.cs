namespace project2.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;


    public User() { }

    public User(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public override string ToString()
    {
        return $"User: {Email}";
    }
}