using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TZTDate.Infrastructure.Data.DateUser.Commands;
using TZTDate_UserWebApi.Data;
using TZTDate_UserWebApi.Dtos;
using TZTDate_UserWebApi.Enums;
using TZTDate_UserWebApi.Filters;
using TZTDate_UserWebApi.Models;
using TZTDate_UserWebApi.Services;
using TZTDate_UserWebApi.Services.Base;

namespace TZTDate_UserWebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[ServiceFilter(typeof(ValidationFilterAttribute))]

public class UserController : ControllerBase
{
    private const int pageItemsCount = 12;
    private readonly ISender sender;
    private readonly UserDbContext context;
    private readonly IAzureBlobService azureBlobService;
    private readonly IInterestsService interestsService;
    private readonly ISearchDataService searchDataService;

    public UserController(ISender sender,
                          UserDbContext context,
                          IAzureBlobService azureBlobService,
                          IInterestsService interestsService,
                          ISearchDataService searchDataService)
    {
        this.sender = sender;
        this.context = context;
        this.azureBlobService = azureBlobService;
        this.interestsService = interestsService;
        this.searchDataService = searchDataService;
    }

    [HttpGet()]
    public async Task<ActionResult<User>> Account(int id)
    {
        var userWithAddress = await context.Users
            .Include(x => x.Address)
            .FirstOrDefaultAsync(o => o.Id == id);

        var interests = await interestsService.GetInterestsAsync(id);

        if (userWithAddress == null)
        {
            return NotFound($"User with id '{id}' doesn't exist!");
        }

        var ImageUris = new List<string>();

        foreach (var path in userWithAddress.ProfilePicPaths)
        {
            var securePath = azureBlobService.GetBlobItemSAS(path);

            ImageUris.Add(securePath);
        }

        return Ok(new AccountDto
        {
            User = userWithAddress,
            Interests = interests ?? "",
            ImageUris = ImageUris
        });
    }

    [HttpGet()]
    public async Task<ActionResult<User>> Details(int currentUserId, int viewedUserId)
    {
        var user = await sender.Send(new FindByIdCommand
        {
            Id = viewedUserId

        });

        if (user == null)
        {
            return NotFound($"User with id '{viewedUserId}' doesn't exist!");
        }

        var currentUser = await sender.Send(new FindByIdCommand
        {
            Id = currentUserId

        });

        var interests = await interestsService.GetInterestsAsync(viewedUserId);

        if (currentUser == null)
        {
            return NotFound($"User with id '{currentUserId}' doesn't exist!");
        }

        var ImageUris = new List<string>();

        foreach (var path in user.ProfilePicPaths)
        {
            var securePath = azureBlobService.GetBlobItemSAS(path);

            ImageUris.Add(securePath);
        }

        return Ok(new { User = user, Interests = interests ?? "", ImageUris, MyUser = currentUser });
    }



    [HttpGet()]
    public async Task<IActionResult> Profiles(int userId, string? searchByName, int? startAge, int? endAge, string? interests, Gender? searchGender)
    {
        var me = await sender.Send(new FindByIdCommand
        {
            Id = userId
        });

        if (me == null)
        {
            return NotFound($"User with id '{userId}' doesn't exist!");
        }

        var users = await context.Users.ToListAsync();

        users.ForEach(user =>
        {
            for (int i = 0; i < user?.ProfilePicPaths.Count(); i++)
            {
                user.ProfilePicPaths[i] = azureBlobService.GetBlobItemSAS(user.ProfilePicPaths[i]);
            }
        });

        SearchData searchData = new SearchData()
        {
            Me = me,
            Users = users,
            SearchingGender = searchGender,
            SearchingStartAge = startAge,
            SearchingEndAge = endAge,
            SearchingInterests = interests,
            SearchingUsername = searchByName
        };

        users = await searchDataService.ProfilesFilter(searchData);

        return Ok(new
        {
            SearchingStartAge = me.SearchingAgeStart,
            SearchingEndAge = me.SearchingAgeEnd,
            SearchingGender = me.SearchingGender.ToString(),
            Users = users
        });

        // return Ok(new
        // {
        //     SearchingStartAge = me.SearchingAgeStart,
        //     SearchingEndAge = me.SearchingAgeEnd,
        //     SearchingGender = me.SearchingGender.ToString(),
        //     Users = users.Take(pageItemsCount)
        // });
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsersAsync()
    {
        return Ok(await context?.Users?.ToListAsync());
    }


    [HttpGet]
    public async Task<IActionResult> LoadMoreProfiles(int userId, int skip, string? searchByName, int? startAge, int? endAge, string? interests, string? searchGender)
    {
        var me = await sender.Send(new FindByIdCommand
        {
            Id = userId
        });

        if (me == null)
        {
            return NotFound($"User with id '{userId}' doesn't exist!");
        }

        var users = context.Users.AsQueryable();

        users = await searchDataService.MoreProfilesFilter(new SearchData()
        {
            Me = me,
            Users = users,
            SearchingGender = searchGender == "0" ? Gender.Male : Gender.Female,
            SearchingStartAge = startAge,
            SearchingEndAge = endAge,
            SearchingInterests = interests,
            SearchingUsername = searchByName
        });

        var usersList = users.ToList();

        if (skip < usersList.Count)
        {
            usersList = usersList.Skip(skip).Take(pageItemsCount).ToList();
        }
        else
        {
            return NotFound("No more users to load.");
        }

        return Ok(usersList);
    }

    [HttpGet]
    public async Task<IActionResult> Followers(int userId)
    {
        var currentUser = await context.Users
            .Include(u => u.Followers)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (currentUser == null)
        {
            return NotFound();
        }

        return Ok(currentUser.Followers ?? new List<User>());
    }

    [HttpGet]
    public async Task<IActionResult> Followed(int userId)
    {
        var currentUser = await context.Users
            .Include(u => u.Followed)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (currentUser == null)
        {
            return NotFound();
        }

        return Ok(currentUser.Followed ?? new List<User>());
    }

    [HttpPost]
    public async Task<IActionResult> MembershipAction(int userToActionId, int currentUserId)
    {
        var followActionCommand = new FollowActionCommand
        {
            currentUserId = currentUserId,
            userToActionId = userToActionId
        };
        await sender.Send(followActionCommand);

        return Ok();
    }
}
