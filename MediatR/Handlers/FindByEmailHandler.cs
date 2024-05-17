using MediatR;
using Microsoft.EntityFrameworkCore;
using TZTDate.Infrastructure.Data.DateUser.Commands;
using TZTDate_UserWebApi.Data;
using TZTDate_UserWebApi.Models;

namespace TZTDate.Infrastructure.Data.DateUser.Handlers
{
    public class FindByEmailHandler : IRequestHandler<FindByEmailCommand, User>
    {
        private readonly UserDbContext context;

        public FindByEmailHandler(UserDbContext context)
        {
            this.context = context;
        }

        public async Task<User> Handle(FindByEmailCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                throw new NullReferenceException($"{nameof(request.Email)} cannot be empty");

            }

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            
            return user;
        }
    }
}