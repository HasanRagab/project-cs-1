namespace project2.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using project2.Models;

public class UserService
{
    private readonly ProductContext _context;

    public UserService(ProductContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _context.Database.EnsureCreated();
    }

    public User CreateUser(string email, string password)
    {
        var user = new User
        {
            Email = email,
            Password = password
        };

        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public User? GetUserById(int id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
    }

    public User? GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }
}