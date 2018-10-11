using Bit.Core.Contracts;
using Bit.Test;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoLine.Controller;
using ToDoLine.Dto;
using ToDoLine.Model;

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
                IODataClient client = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName = Guid.NewGuid().ToString("N");

                await client.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs { userRegistration = new UserRegistrationDto { UserName = userName, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                var token = await testEnv.Server.Login(userName, "P@ssw0rd", "ToDoLine", "secret");

                client = testEnv.Server.BuildODataClient(token: token, odataRouteName: "ToDoLine");

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
                IODataClient client1 = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName1 = Guid.NewGuid().ToString("N");

                await client1.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs { userRegistration = new UserRegistrationDto { UserName = userName1, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                var token1 = await testEnv.Server.Login(userName1, "P@ssw0rd", "ToDoLine", "secret");

                client1 = testEnv.Server.BuildODataClient(token: token1, odataRouteName: "ToDoLine");

                ToDoGroupDto toDoGroup = await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                   .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                   .ExecuteAsSingleAsync();

                IODataClient client2 = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName2 = Guid.NewGuid().ToString("N");

                await client2.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs { userRegistration = new UserRegistrationDto { UserName = userName2, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                UserDto user2 = (await client1.Controller<UsersController, UserDto>()
                    .Function(nameof(UsersController.GetAllUsers))
                    .Filter(u => u.UserName.ToLower().Contains(userName2.ToLower()))
                    .FindEntriesAsync()).Single();

                await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                     .Action(nameof(ToDoGroupsController.ShareToDoGroupWithAnotherUser))
                     .Set(new ToDoGroupsController.ShareToDoGroupWithAnotherUserArgs { anotherUserId = user2.Id, toDoGroupId = toDoGroup.Id })
                     .ExecuteAsync();

                var token2 = await testEnv.Server.Login(userName2, "P@ssw0rd", "ToDoLine", "secret");

                client2 = testEnv.Server.BuildODataClient(token: token2, odataRouteName: "ToDoLine");

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
                IODataClient client = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName = Guid.NewGuid().ToString("N");

                await client.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs { userRegistration = new UserRegistrationDto { UserName = userName, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                var token = await testEnv.Server.Login(userName, "P@ssw0rd", "ToDoLine", "secret");

                client = testEnv.Server.BuildODataClient(token: token, odataRouteName: "ToDoLine");

                UserDto user = await client.Controller<UsersController, UserDto>()
                    .Function(nameof(UsersController.GetAllUsers))
                    .Filter(u => u.UserName.ToLower().Contains(userName))
                    .FindEntryAsync();

                ToDoGroupDto toDoGroup = await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                                  .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                                  .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                                  .ExecuteAsSingleAsync();

                ToDoItemDto toDoItem = await client.Controller<ToDoItemsController, ToDoItemDto>()
                   .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample",ShowInMyDay = true , ToDoGroupId = toDoGroup.Id})
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
                IODataClient client = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName = Guid.NewGuid().ToString("N");

                await client.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs { userRegistration = new UserRegistrationDto { UserName = userName, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                var token = await testEnv.Server.Login(userName, "P@ssw0rd", "ToDoLine", "secret");

                client = testEnv.Server.BuildODataClient(token: token, odataRouteName: "ToDoLine");

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
                IODataClient client = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName = Guid.NewGuid().ToString("N");

                await client.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs { userRegistration = new UserRegistrationDto { UserName = userName, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                var token = await testEnv.Server.Login(userName, "P@ssw0rd", "ToDoLine", "secret");

                client = testEnv.Server.BuildODataClient(token: token, odataRouteName: "ToDoLine");

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
