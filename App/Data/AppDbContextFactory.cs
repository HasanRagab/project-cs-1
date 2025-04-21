using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using App.Models;

namespace App.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlite("Data Source=app.db");

        var context = new AppDbContext(optionsBuilder.Options);
        context.Database.EnsureCreated();
        return context;
    }

    public static void SeedDatabase()
    {
        using var context = new AppDbContextFactory().CreateDbContext([]);
        var adminUser = context.Users.FirstOrDefault(u => u.Email == "admin@admin.com");

        if (adminUser == null)
        {
            var newAdminUser = new User
            {
                Email = "admin@admin.com",
                Password = "admin",
                IsAdmin = true,
            };

            context.Users.Add(newAdminUser);
            context.SaveChanges();
        }
    }
}
