using TZTDate_UserWebApi.Services;
using TZTDate_UserWebApi.Services.Base;
using TZTDate_UserWebApi.Filters;
using TZTDate_UserWebApi.Middlewares;
using TZTDate_UserWebApi.Data;
namespace TZTDate_UserWebApi.Extensions;

public static class InjectionExtensions
{
    public static void Inject(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAzureBlobService, AzureBlobService>();
        serviceCollection.AddScoped<IInterestsService, InterestsMongoService>();
        serviceCollection.AddScoped<ISearchDataService, SearchDataService>();

        serviceCollection.AddTransient<ExceptionHandlingMiddleware>();
        serviceCollection.AddScoped<ValidationFilterAttribute>();

        serviceCollection.AddScoped<UserDbContext>();
    }
}
