namespace project2.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Category
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    
    public override string ToString()
    {
        return $"Category: {Name}";
    }
}
