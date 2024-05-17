using MongoDB.Bson.Serialization.Attributes;

namespace TZTDate_UserWebApi.Models;

public class UserInterests
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; }
    public IEnumerable<string> Interests { get; set; }
    public int UserId { get; set; }
}
