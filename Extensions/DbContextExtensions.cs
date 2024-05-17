using Microsoft.EntityFrameworkCore;
using TZTDate_UserWebApi.Data;

namespace TZTDate_UserWebApi.Extensions;

public static class DbContextExtensions
{
    public static void InitDbContext(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<UserDbContext>(options =>
        {
            string connectionStringKey = "DefaultConnectionString";
            string? connectionString = configuration.GetConnectionString(connectionStringKey);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new NullReferenceException($"No connection string found in appsettings.json with a key '{connectionStringKey}'");
            }

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            options.UseNpgsql(connectionString);
        });
    }
}
