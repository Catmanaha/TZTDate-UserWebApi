using MediatR;
using TZTDate_UserWebApi.Models;

namespace TZTDate.Infrastructure.Data.DateUser.Commands;

public class GetUserRolesCommand : IRequest<IEnumerable<Role>>
{
    public int UserId { get; set; }
}
