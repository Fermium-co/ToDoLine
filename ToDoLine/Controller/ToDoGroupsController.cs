using Bit.Core.Contracts;
using Bit.Data.Contracts;
using Bit.OData.ODataControllers;
using Bit.Owin.Exceptions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToDoLine.Dto;
using ToDoLine.Enum;
using ToDoLine.Model;

namespace ToDoLine.Controller
{
    public class ToDoGroupsController : DtoController<ToDoGroupDto>
    {
        public virtual IDateTimeProvider DateTimeProvider { get; set; }

        public virtual IRepository<ToDoGroup> ToDoGroupsRepository { get; set; }

        public virtual IRepository<ToDoGroupOptions> ToDoGroupOptionsListRepository { get; set; }

        public virtual IUserInformationProvider UserInformationProvider { get; set; }

        public virtual IRepository<User> UsersRepository { get; set; }

        public virtual IRepository<ToDoItemOptions> ToDoItemOptionsListRepository { get; set; }

        [Function]
        public virtual IQueryable<ToDoGroupDto> GetMyToDoGroups()
        {
            Guid userId = Guid.Parse(UserInformationProvider.GetCurrentUserId());

            return ToDoGroupOptionsListRepository.GetAll().Where(tdgo => tdgo.UserId == userId)
                  .Select(tdgo => new ToDoGroupDto
                  {
                      Id = tdgo.ToDoGroup.Id,
                      CreatedBy = tdgo.ToDoGroup.CreatedBy.UserName,
                      CreatedOn = tdgo.ToDoGroup.CreatedOn,
                      HideCompletedToDoItems = tdgo.HideCompletedToDoItems,
                      IsDefault = tdgo.ToDoGroup.IsDefault,
                      ModifiedOn = tdgo.ToDoGroup.ModifiedOn,
                      SharedByCount = tdgo.ToDoGroup.Options.Count,
                      SortedBy = tdgo.SortedBy,
                      Theme = tdgo.Theme,
                      Title = tdgo.ToDoGroup.Title
                  });
        }

        public class CreateToDoGroupArgs
        {
            public string title { get; set; }
        }

        [Action]
        public virtual async Task<ToDoGroupDto> CreateToDoGroup(CreateToDoGroupArgs args, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(args.title))
                throw new BadRequestException("TitleMayNotBeEmpty");

            Guid userId = Guid.Parse(UserInformationProvider.GetCurrentUserId());

            ToDoGroup addedToDoGroup = await ToDoGroupsRepository.AddAsync(new ToDoGroup
            {
                Id = Guid.NewGuid(),
                CreatedById = userId,
                CreatedOn = DateTimeProvider.GetCurrentUtcDateTime(),
                IsDefault = false,
                ModifiedOn = DateTimeProvider.GetCurrentUtcDateTime(),
                Title = args.title,
                Options = new List<ToDoGroupOptions>
                {
                   new ToDoGroupOptions
                   {
                       HideCompletedToDoItems = false,
                       SortedBy = SortBy.CreatedDate,
                       Id = Guid.NewGuid(),
                       Theme = "Green",
                       UserId = userId
                   }
                }
            }, cancellationToken);

            return await GetMyToDoGroups()
                .FirstAsync(tdg => tdg.Id == addedToDoGroup.Id, cancellationToken);
        }

        public class UpdateToDoGroupExamplesProvider : IExamplesProvider
        {
            public object GetExamples()
            {
                return new
                {
                    Title = "Test",
                    Theme = "Green",
                    HideCompletedToDoItems = false,
                    SortedBy = SortBy.Importance
                };
            }
        }

        [PartialUpdate]
        [SwaggerRequestExample(typeof(ToDoGroupDto), typeof(UpdateToDoGroupExamplesProvider), jsonConverter: typeof(StringEnumConverter))]
        public virtual async Task<ToDoGroupDto> UpdateToDoGroup(Guid key, ToDoGroupDto toDoGroup, CancellationToken cancellationToken)
        {
            if (toDoGroup == null)
                throw new BadRequestException("ToDoGroupMayNotBeNull");

            Guid userId = Guid.Parse(UserInformationProvider.GetCurrentUserId());

            ToDoGroup toDoGroupToBeModified = await ToDoGroupsRepository.GetByIdAsync(cancellationToken, key);

            if (toDoGroupToBeModified == null)
                throw new ResourceNotFoundException("ToDoGroupCouldNotBeFound");

            if (toDoGroupToBeModified.IsDefault == true && toDoGroup.Title != "Tasks")
                throw new BadRequestException("CanNotChangeTitleOfDefaultToDoGroup");

            ToDoGroupOptions toDoGroupOptionsToBeModified = await ToDoGroupOptionsListRepository.GetAll()
                .FirstAsync(tdgo => tdgo.UserId == userId && tdgo.ToDoGroupId == key, cancellationToken);

            if (toDoGroupOptionsToBeModified == null)
                throw new ResourceNotFoundException("ToDoGroupCouldNotBeFound");

            toDoGroupToBeModified.Title = toDoGroup.Title;
            toDoGroupToBeModified.ModifiedOn = DateTimeProvider.GetCurrentUtcDateTime();

            toDoGroupOptionsToBeModified.Theme = toDoGroup.Theme;
            toDoGroupOptionsToBeModified.HideCompletedToDoItems = toDoGroup.HideCompletedToDoItems;
            toDoGroupOptionsToBeModified.SortedBy = toDoGroup.SortedBy;

            await ToDoGroupsRepository.UpdateAsync(toDoGroupToBeModified, cancellationToken);
            await ToDoGroupOptionsListRepository.UpdateAsync(toDoGroupOptionsToBeModified, cancellationToken);

            return await GetMyToDoGroups()
                .FirstAsync(tdg => tdg.Id == key, cancellationToken);
        }

        [Delete]
        public virtual async Task DeleteToDoGroup(Guid key, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(UserInformationProvider.GetCurrentUserId());

            ToDoGroup toDoGroupToBeDeleted = await ToDoGroupsRepository.GetByIdAsync(cancellationToken, key);

            if (toDoGroupToBeDeleted == null)
                throw new ResourceNotFoundException("ToDoGroupCouldNotBeFound");

            if (toDoGroupToBeDeleted.IsDefault == true)
                throw new BadRequestException("CanNotDeleteDefaultToDoGroup");

            if (toDoGroupToBeDeleted.CreatedById != userId)
                throw new DomainLogicException("OnlyOwnerCanDeleteTheToDoGroup");

            await ToDoGroupsRepository.DeleteAsync(toDoGroupToBeDeleted, cancellationToken);
        }

        public class ShareToDoGroupWithAnotherUserArgs
        {
            public Guid toDoGroupId { get; set; }

            public Guid anotherUserId { get; set; }
        }

        [Action]
        public virtual async Task<ToDoGroupDto> ShareToDoGroupWithAnotherUser(ShareToDoGroupWithAnotherUserArgs args, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(UserInformationProvider.GetCurrentUserId());

            ToDoGroup toDoGroup = await ToDoGroupsRepository.GetAll()
                .Where(tdg => tdg.CreatedById == userId && tdg.Id == args.toDoGroupId)
                .Select(tdg => new ToDoGroup
                {
                    Id = tdg.Id,
                    IsDefault = tdg.IsDefault,
                    Items = tdg.Items.Select(tdi => new ToDoItem
                    {
                        Id = tdi.Id
                    }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);

            if (toDoGroup == null)
                throw new ResourceNotFoundException("ToDoGroupCouldNotBeFound");

            if (toDoGroup.IsDefault == true)
                throw new DomainLogicException("CanNotShareDefaultToDoGroup");

            await ToDoGroupOptionsListRepository.AddAsync(new ToDoGroupOptions
            {
                HideCompletedToDoItems = false,
                Id = Guid.NewGuid(),
                SortedBy = SortBy.CreatedDate,
                Theme = "Green",
                ToDoGroupId = args.toDoGroupId,
                UserId = args.anotherUserId
            }, cancellationToken);

            foreach (ToDoItem toDoItem in toDoGroup.Items)
            {
                await ToDoItemOptionsListRepository.AddAsync(new ToDoItemOptions
                {
                    Id = Guid.NewGuid(),
                    ToDoItemId = toDoItem.Id,
                    UserId = args.anotherUserId,
                    ShowInMyDayOn = null
                }, cancellationToken);
            }

            return await GetMyToDoGroups().FirstAsync(tdg => tdg.Id == args.toDoGroupId, cancellationToken);
        }

        public class KickAnotherUserFromMyToDoGroupArge
        {
            public Guid userId { get; set; }
            public Guid toDoGroupId { get; set; }
        }

        [Action]
        public virtual async Task KickUserFromToDoGroup(KickAnotherUserFromMyToDoGroupArge args, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(UserInformationProvider.GetCurrentUserId());

            ToDoGroup toDoGroupsToBeKickFrom = await ToDoGroupsRepository.GetAll()
                .Where(tdg => tdg.Id == args.toDoGroupId)
                .Select(tdg => new ToDoGroup
                {
                    Id = tdg.Id,
                    CreatedById = tdg.CreatedById,
                    Items = tdg.Items.Select(tdi => new ToDoItem
                    {
                        Id = tdi.Id,
                        Options = tdi.Options.Where(tdio => tdio.UserId == args.userId).Select(tdio => new ToDoItemOptions
                        {
                            Id = tdio.Id,
                            UserId = tdio.UserId
                        }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync(cancellationToken); ;

            if (toDoGroupsToBeKickFrom == null)
                throw new ResourceNotFoundException("ToDoGroupCouldNotBeFound");

            if (toDoGroupsToBeKickFrom.CreatedById != userId)
                throw new DomainLogicException("OnlyOwnerCanKickOtherUsers");

            User kickedUser = await UsersRepository.GetByIdAsync(cancellationToken, args.userId);

            if (kickedUser == null)
                throw new ResourceNotFoundException("UserCouldNotBeFoundToBeKicked");

            foreach (ToDoItemOptions toDoItemOptionsToBeDeleted in toDoGroupsToBeKickFrom.Items.SelectMany(tdi => tdi.Options) /* We've loaded options of to be kicked user only! */)
            {
                await ToDoItemOptionsListRepository.DeleteAsync(toDoItemOptionsToBeDeleted, cancellationToken);
            }

            ToDoGroupOptions toDoGroupOptionsToBeDeleted = await ToDoGroupOptionsListRepository.GetAll().FirstOrDefaultAsync(tdo => tdo.ToDoGroupId == args.toDoGroupId && tdo.UserId == args.userId);

            await ToDoGroupOptionsListRepository.DeleteAsync(toDoGroupOptionsToBeDeleted, cancellationToken);
        }
    }
}
