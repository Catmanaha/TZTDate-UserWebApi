using MediatR;
using TZTDate_UserWebApi.Models;

namespace TZTDate.Infrastructure.Data.DateUser.Commands
{
    public class FindByIdCommand : IRequest<User>
    {
        public int Id { get; set; }
    }
}