using App.Data;
using App.Models;
using System.Linq;

namespace App.Services
{
    public static class AuthService
    {
        private static AppDbContext _dbContext = DbContextManager.GetDbContext();

        public static User? GetUserById(int id)
        {
            return _dbContext.Users.Find(id);
        }

        public static User? GetUserByEmail(string email)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Email == email);
        }

        public static void RegisterUser(string email, string password)
        {
            var user = new User { Email = email, Password = password };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var cart = new Cart { UserId = user.Id };
            _dbContext.Carts.Add(cart);
            _dbContext.SaveChanges();
        }

        public static void Login(string email, string password)
        {
            var user = GetUserByEmail(email) ?? throw new UnauthorizedAccessException("User not found.");
            if (user.Password == password)
            {
                GlobalStore.Instance.CurrentUser = user;
                return;
            }
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        public static void UpdateUser(User user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }

        public static void DeleteUser(int id)
        {
            var user = _dbContext.Users.Find(id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
            }
        }

        public static void ChangePassword(int id, string newPassword)
        {
            var user = _dbContext.Users.Find(id);
            if (user != null)
            {
                user.Password = newPassword;
                _dbContext.SaveChanges();
            }
        }

        public static void SetAdmin(int id, bool isAdmin)
        {
            var user = _dbContext.Users.Find(id);
            if (user != null)
            {
                user.IsAdmin = isAdmin;
                _dbContext.SaveChanges();
            }
        }

        public static bool UserExists(string email)
        {
            return _dbContext.Users.Any(u => u.Email == email);
        }
    }
}
