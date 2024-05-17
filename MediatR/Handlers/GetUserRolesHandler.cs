using MediatR;
using Microsoft.EntityFrameworkCore;
using TZTDate.Infrastructure.Data.DateUser.Commands;
using TZTDate_UserWebApi.Data;
using TZTDate_UserWebApi.Exceptions;
using TZTDate_UserWebApi.Models;

namespace TZTDate.Infrastructure.Data.DateUser.Handlers;

public class GetUserRolesHandler : IRequestHandler<GetUserRolesCommand, IEnumerable<Role>>
{
    private readonly UserDbContext context;
    public GetUserRolesHandler(UserDbContext context)
    {
        this.context = context;

    }

    public async Task<IEnumerable<Role>> Handle(GetUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
        {
            throw new EntityNotFoundException("User not found");
        }

        var roles = user.UserRoles.Select(ur => ur.Role).ToList();
        return roles;
    }
}
