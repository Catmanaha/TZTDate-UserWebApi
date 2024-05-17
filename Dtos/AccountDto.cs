using TZTDate_UserWebApi.Models;

namespace TZTDate_UserWebApi.Dtos;

public class AccountDto
{
    public User User { get; set; }
    public string Interests { get; set; }
    public List<string> ImageUris { get; set; }
}
