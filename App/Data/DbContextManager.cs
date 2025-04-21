using Microsoft.EntityFrameworkCore;
using App.Data;

namespace App.Services
{
    public static class DbContextManager
    {
        private static AppDbContext? _dbContext;

        public static AppDbContext GetDbContext()
        {
            if (_dbContext == null)
            {
                var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite("Data Source=app.db") 
                    .Options;

                _dbContext = new AppDbContext(dbContextOptions);
            }

            return _dbContext;
        }
    }
}
