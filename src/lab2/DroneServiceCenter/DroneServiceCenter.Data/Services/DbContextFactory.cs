using DroneServiceCenter.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace DroneServiceCenter.Data.Services
{
    public static class DbContextFactory
    {
        private static readonly string ConnectionString =
            "Server=KARISHAAA-PC;Database=DroneServiceCenterDb;Trusted_Connection=True;TrustServerCertificate=True;";

        public static AppDbContext Create()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

            return new AppDbContext(options);
        }

        public static void EnsureInitialized()
        {
            using var context = Create();
            DbInitializer.Initialize(context);
        }
    }
}