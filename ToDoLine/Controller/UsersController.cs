using Bit.Data.Contracts;
using Bit.OData.ODataControllers;
using System.Linq;
using ToDoLine.Dto;
using ToDoLine.Model;

namespace ToDoLine.Controller
{
    public class UsersController : DtoController<UserDto>
    {
        public virtual IRepository<User> UsersRepository { get; set; }

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
    }
}
