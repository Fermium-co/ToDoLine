using Bit.Core.Contracts;
using Bit.Data.Contracts;
using Bit.IdentityServer.Implementations;
using Bit.Owin.Exceptions;
using IdentityServer3.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using ToDoLine.Model;
using ToDoLine.Util;

namespace ToDoLine.Security
{
    public class ToDoLineUserService : UserService
    {
        public virtual IRepository<User> UsersRepository { get; set; }

        public async override Task<BitJwtToken> LocalLogin(LocalAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(context.UserName) || string.IsNullOrEmpty(context.Password))
                throw new BadRequestException("InvalidUserNameAndOrPassword");

            User user = await UsersRepository.GetAll().SingleOrDefaultAsync(u => u.UserName.ToLower() == context.UserName.ToLower(), cancellationToken);

            if (user == null)
                throw new BadRequestException("InvalidUserNameAndOrPassword");

            if (!HashUtility.VerifyHash(context.Password, user.Password))
                throw new BadRequestException("InvalidUserNameAndOrPassword");

            return new BitJwtToken { UserId = user.Id.ToString() };
        }

        protected async override Task<BitJwtToken> ExternalLogin(ExternalAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (context.ExternalIdentity.Provider == "Google")
            {

            }
            // get claims from google/facebook etc and create a user if required. Then return expected user id.
            return new BitJwtToken { UserId = "1" };
        }
    }
}
