using MediatR;
using TZTDate_UserWebApi.Models;

namespace TZTDate.Infrastructure.Data.DateUser.Commands;

public class FindByEmailCommand : IRequest<User>
{
    public string Email { get; set; }
}
