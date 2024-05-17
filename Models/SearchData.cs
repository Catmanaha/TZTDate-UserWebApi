using TZTDate_UserWebApi.Enums;

namespace TZTDate_UserWebApi.Models;

public class SearchData
{
    public User? Me { get; set; }
    public IEnumerable<User>? Users { get; set; }
    public Gender? SearchingGender { get; set; }
    public string? SearchingUsername { get; set; }
    public int? SearchingStartAge { get; set; }
    public int? SearchingEndAge { get; set; }
    public string? SearchingInterests { get; set; }
}