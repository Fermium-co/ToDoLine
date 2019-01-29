using Bit.Model;
using Bit.Model.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ToDoLine.Dto
{
    public partial class UserRegistrationDto : IDto
    {
        [Key]
        public virtual string UserName { get; set; }

        public virtual string Password { get; set; }
    }
}
