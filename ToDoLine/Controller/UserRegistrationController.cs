﻿using Bit.Core.Contracts;
using Bit.Data.Contracts;
using Bit.OData.ODataControllers;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ToDoLine.Dto;
using ToDoLine.Model;
using ToDoLine.Util;

namespace ToDoLine.Controller
{
    public class UserRegistrationController : DtoController<UserRegistrationDto>
    {
        public virtual IRepository<User> UsersRepository { get; set; }

        public virtual IDateTimeProvider DateTimeProvider { get; set; }

        [Action]
        [AllowAnonymous]
        public virtual async Task Register(UserRegistrationDto userInfo, CancellationToken cancellationToken)
        {
            Guid newUserId = Guid.NewGuid();

            await UsersRepository.AddAsync(new User
            {
                Id = newUserId,
                Password = HashUtility.Hash(userInfo.Password),
                UserName = userInfo.UserName
            }, cancellationToken);
        }
    }
}
