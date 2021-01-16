using Bit.Core.Contracts;
using Bit.Data.Contracts;
using Bit.OData.ODataControllers;
using System;
using System.Linq;
using System.Web.Http;
using ToDoLine.Dto;
using ToDoLine.Model;

namespace ToDoLine.Controller
{
    public class UsersController : DtoController<UserDto>
    {
        public virtual IRepository<User> UsersRepository { get; set; }
        public virtual IUserInformationProvider UserInformationProvider { get; set; }

        [Function]
        public virtual IQueryable<UserDto> GetAllUsers()
        {
            return UsersRepository.GetAll()
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName
                });
        }

        [Function]
        public virtual SingleResult<UserDto> GetCurrentUser()
        {
            Guid userId = Guid.Parse(UserInformationProvider.GetCurrentUserId());

            return SingleResult(GetAllUsers()
                .Where(u => u.Id == userId));
        }
    }
}
