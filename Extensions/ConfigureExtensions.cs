using TZTDate_UserWebApi.Options;

namespace TZTDate_UserWebApi.Extensions;

public static class ConfigureExtensions
{
    public static void Configure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {

        serviceCollection.Configure<JwtOption>(configuration.GetSection("JwtOption"));
        serviceCollection.Configure<BlobOption>(configuration.GetSection("BlobOption"));
        serviceCollection.Configure<MongoOption>(configuration.GetSection("MongoOption"));
    }
}
