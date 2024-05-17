using TZTDate_UserWebApi.Models;
using TZTDate_UserWebApi.Services.Base;

namespace TZTDate_UserWebApi.Services;

public class SearchDataService : ISearchDataService
{
    private readonly IInterestsService interestsService;
    public SearchDataService(IInterestsService interestsService)
    {
        this.interestsService = interestsService;

    }

    public async Task<List<User>> ProfilesFilter(SearchData searchData)
    {

        var users = searchData.Users.AsEnumerable();
        var me = searchData.Me;

        if (me == null)
        {
            throw new ArgumentNullException(nameof(me));
        }

        users = users.Where(u => u.Id != me.Id);

        if (searchData.SearchingUsername is null && searchData.SearchingStartAge is null && searchData.SearchingEndAge is null && searchData.SearchingInterests is null && searchData.SearchingGender is null)
        {
            users = users.Where(u => u.Age >= me.SearchingAgeStart && u.Age <= me.SearchingAgeEnd && u.Gender == me.SearchingGender);
        }
        else
        {
            if (!string.IsNullOrEmpty(searchData.SearchingUsername))
            {
                users = users.Where(u => u.Username.Contains(searchData.SearchingUsername));
            }

            if (searchData.SearchingStartAge.HasValue && searchData.SearchingStartAge != 0)
            {
                users = users.Where(u => u.Age >= searchData.SearchingStartAge);
            }

            if (searchData.SearchingEndAge.HasValue && searchData.SearchingEndAge != 0)
            {
                users = users.Where(u => u.Age <= searchData.SearchingEndAge);
            }

            if (searchData.SearchingGender is not null)
            {
                users = users.Where(u => u.Gender == searchData.SearchingGender);
            }

            if (searchData.SearchingInterests is not null)
            {
                string[] interestsArray = searchData.SearchingInterests.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var filteredUsers = new List<User>();

                foreach (var user in users)
                {
                    var interests = await interestsService.GetInterestsAsync(user.Id);
                    var userInterestsArray = interests?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (userInterestsArray != null && userInterestsArray.Intersect(interestsArray).Any())
                    {
                        filteredUsers.Add(user);
                    }
                }

                users = filteredUsers;
            }
        }

        return users.ToList();
    }

    public async Task<IQueryable<User>> MoreProfilesFilter(SearchData searchData)
    {
        var users = searchData.Users;
        var me = searchData.Me;

        if (me == null)
        {
            throw new ArgumentNullException(nameof(me));
        }

        users = users.Where(u => u.Id != me.Id).AsQueryable();

        if (!string.IsNullOrEmpty(searchData.SearchingUsername))
        {
            users = users.Where(u => u.Username.Equals(searchData.SearchingUsername, StringComparison.OrdinalIgnoreCase)).AsQueryable();
        }

        if (searchData.SearchingStartAge.HasValue && searchData.SearchingStartAge != 0)
        {
            users = users.Where(u => u.Age >= searchData.SearchingStartAge).AsQueryable();
        }

        if (searchData.SearchingEndAge.HasValue && searchData.SearchingEndAge != 0)
        {
            users = users.Where(u => u.Age <= searchData.SearchingEndAge).AsQueryable();
        }

        if (searchData.SearchingGender is not null)
        {
            users = users.Where(u => u.Gender == searchData.SearchingGender).AsQueryable();
        }

        if (!string.IsNullOrWhiteSpace(searchData.SearchingInterests))
        {
            string[] interestsArray = searchData.SearchingInterests.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var filteredUsers = new List<User>();

            foreach (var user in users)
            {
                var interests = await interestsService.GetInterestsAsync(user.Id);
                var userInterestsArray = interests?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (userInterestsArray != null && userInterestsArray.Intersect(interestsArray).Any())
                {
                    filteredUsers.Add(user);
                }
            }

            users = filteredUsers;
        }

        return users.AsQueryable();
    }
}