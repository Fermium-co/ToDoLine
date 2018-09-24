using Bit.Core.Contracts;
using Bit.Data.Contracts;
using Bit.OData.ODataControllers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ToDoLine.Dto;
using ToDoLine.Enum;
using ToDoLine.Model;
using ToDoLine.Util;

namespace ToDoLine.Controller
{
    public class UserRegistrationController : DtoController<UserRegistrationDto>
    {
        public virtual IRepository<User> UsersRepository { get; set; }

        public virtual IDateTimeProvider DateTimeProvider { get; set; }

        public class RegisterArgs
        {
            public UserRegistrationDto userRegistration { get; set; }
        }

        [Action]
        [AllowAnonymous]
        public virtual async Task Register(RegisterArgs args, CancellationToken cancellationToken)
        {
            Guid newUserId = Guid.NewGuid();

            await UsersRepository.AddAsync(new User
            {
                Id = newUserId,
                Password = HashUtility.Hash(args.userRegistration.Password),
                UserName = args.userRegistration.UserName,
                OwnedToDoGroups = new List<ToDoGroup>
                {
                    new ToDoGroup
                    {
                         Id = Guid.NewGuid(),
                         CreatedOn = DateTimeProvider.GetCurrentUtcDateTime(),
                         ModifiedOn = DateTimeProvider.GetCurrentUtcDateTime(),
                         Title = "Tasks",
                         IsDefault = true,
                         CreatedById = newUserId,
                         Options = new List<ToDoGroupOptions>
                         {
                            new ToDoGroupOptions
                            {
                                Id = Guid.NewGuid(),
                                SortedBy = SortBy.CreatedDate,
                                Theme = "Green",
                                UserId = newUserId
                            }
                         }
                    }
                }
            }, cancellationToken);
        }
    }
}
