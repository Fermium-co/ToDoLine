using Bit.Core.Contracts;
using Bit.Test;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.OData.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoLine.Controller;
using ToDoLine.Dto;

namespace ToDoLine.Test.Controller
{
    [TestClass]
    public class ToDoItemsControllerTest
    {
        [TestMethod]
        public async Task CreateToDoItemTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName, client) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                   .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                   .ExecuteAsSingleAsync();

                ToDoItemDto toDoItem = await client.Controller<ToDoItemsController, ToDoItemDto>()
                   .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ToDoGroupId = toDoGroup.Id })
                   .InsertEntryAsync();

                Assert.AreEqual("Task1", toDoItem.Title);
            }
        }

        [TestMethod]
        public async Task CreatingToDoItemShouldCreateToDoItemOptionsPerUserTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName1, client1) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                   .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                   .ExecuteAsSingleAsync();

                var (userName2, client2) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                UserDto user2 = await client1.GetUserByUserName(userName2);

                await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                     .Action(nameof(ToDoGroupsController.ShareToDoGroupWithAnotherUser))
                     .Set(new ToDoGroupsController.ShareToDoGroupWithAnotherUserArgs { anotherUserId = user2.Id, toDoGroupId = toDoGroup.Id })
                     .ExecuteAsync();

                ToDoItemDto toDoItem = await client2.Controller<ToDoItemsController, ToDoItemDto>()
                   .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ToDoGroupId = toDoGroup.Id })
                   .InsertEntryAsync();

                bool hasToDoItem = (await client1.Controller<ToDoItemsController, ToDoItemDto>()
                    .Function(nameof(ToDoItemsController.GetMyToDoItems))
                    .FindEntriesAsync()).Any();

                Assert.AreEqual(true, hasToDoItem);
            }
        }

        [TestMethod]
        public async Task ActiveShowInMyDayToDoItemTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv(new TestEnvironmentArgs
            {
                AdditionalDependencies = (dependencyManager, services) =>
                {
                    IDateTimeProvider dateTimeProvider = A.Fake<IDateTimeProvider>();

                    A.CallTo(() => dateTimeProvider.GetCurrentUtcDateTime())
                        .Returns(new DateTimeOffset(2018, 1, 1, 10, 10, 10, TimeSpan.Zero));

                    dependencyManager.RegisterInstance(dateTimeProvider);
                }
            }))

            {
                var (userName, client) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                UserDto user = await client.GetUserByUserName(userName);

                ToDoGroupDto toDoGroup = await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                                  .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                                  .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                                  .ExecuteAsSingleAsync();

                ToDoItemDto toDoItem = await client.Controller<ToDoItemsController, ToDoItemDto>()
                   .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ShowInMyDay = true, ToDoGroupId = toDoGroup.Id })
                   .InsertEntryAsync();
                Assert.AreEqual("Task1", toDoItem.Title);
                Assert.AreEqual(true, toDoItem.ShowInMyDay);
            }
        }

        [TestMethod]
        public async Task UpdateToDoItemTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName, client) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                   .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                   .ExecuteAsSingleAsync();

                ToDoItemDto toDoItem = await client.Controller<ToDoItemsController, ToDoItemDto>()
                   .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ToDoGroupId = toDoGroup.Id })
                   .InsertEntryAsync();

                toDoItem.Title += "!";
                toDoItem.IsCompleted = true;

                ToDoItemDto updatedToDoItem = await client.Controller<ToDoItemsController, ToDoItemDto>()
                   .Key(toDoItem.Id)
                   .Set(toDoItem)
                   .UpdateEntryAsync();

                Assert.AreEqual("Task1!", updatedToDoItem.Title);
                Assert.AreEqual(true, updatedToDoItem.IsCompleted);
                Assert.AreEqual(userName, updatedToDoItem.CompletedBy);
            }
        }

        [TestMethod]
        public async Task DeleteToDoItemTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName, client) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                   .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                   .ExecuteAsSingleAsync();

                ToDoItemDto toDoItem = await client.Controller<ToDoItemsController, ToDoItemDto>()
                   .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ToDoGroupId = toDoGroup.Id })
                   .InsertEntryAsync();

                await client.Controller<ToDoItemsController, ToDoItemDto>()
                    .Key(toDoItem.Id)
                    .DeleteEntryAsync();
            }
        }
    }
}
