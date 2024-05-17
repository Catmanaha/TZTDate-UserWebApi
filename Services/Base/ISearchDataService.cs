using TZTDate_UserWebApi.Models;

namespace TZTDate_UserWebApi.Services.Base;

public interface ISearchDataService
{
    public Task<List<User>> ProfilesFilter(SearchData searchData);
    public Task<IQueryable<User>> MoreProfilesFilter(SearchData searchData);
}
