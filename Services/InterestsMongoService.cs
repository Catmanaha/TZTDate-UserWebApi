using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TZTDate_UserWebApi.Models;
using TZTDate_UserWebApi.Options;
using TZTDate_UserWebApi.Services.Base;

namespace TZTDate_UserWebApi.Services;

public class InterestsMongoService : IInterestsService
{
    private readonly MongoClient client;
    private readonly IOptionsSnapshot<MongoOption> options;

    public InterestsMongoService(IOptionsSnapshot<MongoOption> options)
    {
        client = new MongoClient(options.Value.ConnectionString);
        this.options = options;
    }

    public async Task<string> GetInterestsAsync(int userId)
    {
        var userInterests = await client.GetDatabase(options.Value.DatabaseName)
            .GetCollection<UserInterests>(options.Value.CollectionName)
            .Find(x => x.UserId == userId)
            .FirstOrDefaultAsync();

        ;

        return string.Join(", ", userInterests.Interests) ?? "";
    }

    public async Task SetInterestsAsync(int userId, IEnumerable<string> interests)
    {
        var userInterests = new UserInterests
        {
            UserId = userId,
            Interests = interests.ToList()
        };

        await client.GetDatabase(options.Value.DatabaseName)
            .GetCollection<UserInterests>(options.Value.CollectionName)
            .ReplaceOneAsync(x => x.UserId == userId, userInterests, new ReplaceOptions { IsUpsert = true });
    }
}
