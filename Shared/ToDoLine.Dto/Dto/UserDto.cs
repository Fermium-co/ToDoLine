using Bit.Model;
using Bit.Model.Contracts;
using System;

namespace ToDoLine.Dto
{
    public partial class UserDto : IDto
    {
        public virtual Guid Id { get; set; }

        public virtual string UserName { get; set; }
    }
}
