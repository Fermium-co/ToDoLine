using Bit.Core.Contracts;
using Bit.Test;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoLine.Controller;
using ToDoLine.Dto;
using ToDoLine.Enum;

namespace ToDoLine.Test.Controller
{
    [TestClass]
    public class ToDoGroupsControllerTests
    {
        [TestMethod]
        public async Task CreateToDoGroupTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName, client) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto addedToDoGroup = await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                   .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                   .ExecuteAsSingleAsync();

                Assert.AreEqual(1, addedToDoGroup.SharedByCount);
                Assert.AreEqual("Test", addedToDoGroup.Title);

                var toDoGroups = (await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .FindEntriesAsync()).ToArray();

                Assert.AreEqual(1, toDoGroups.Length);
            }
        }

        [TestMethod]
        public async Task UpdateToDoGroupTest()
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

                ToDoGroupDto toDoGroup = await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                   .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                   .ExecuteAsSingleAsync();

                toDoGroup.Title += "?"; // Test?
                toDoGroup.SortedBy = SortBy.Importance;

                toDoGroup = await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Key(toDoGroup.Id)
                    .Set(toDoGroup)
                    .UpdateEntryAsync();

                Assert.AreEqual("Test?", toDoGroup.Title);
                Assert.AreEqual(SortBy.Importance, toDoGroup.SortedBy);
                Assert.AreEqual(new DateTimeOffset(2018, 1, 1, 10, 10, 10, TimeSpan.Zero), toDoGroup.ModifiedOn);
            }
        }

        [TestMethod]
        public async Task DeleteToDoGroupTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName, client) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                   .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                   .ExecuteAsSingleAsync();

                await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Key(toDoGroup.Id)
                    .DeleteEntryAsync();

                Assert.AreEqual(0, (await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .FindEntriesAsync()).Count());
            }
        }

        [TestMethod]
        public async Task ShareToDoGroupWithAnotherUserTest()
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

                var toDoGroups = (await client2.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .FindEntriesAsync()).ToArray();

                Assert.AreEqual(1, toDoGroups.Length);
            }
        }

        [TestMethod]
        public async Task SharingToDoGroupWithAnotherUserShouldShareItsToDoItemsTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName1, client1) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                   .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                   .ExecuteAsSingleAsync();

                ToDoItemDto toDoItem = await client1.Controller<ToDoItemsController, ToDoItemDto>()
                   .Set(new ToDoItemDto { Title = "Test1", ToDoGroupId = toDoGroup.Id })
                   .InsertEntryAsync();

                var (userName2, client2) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                UserDto user2 = await client1.GetUserByUserName(userName2);

                await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                     .Action(nameof(ToDoGroupsController.ShareToDoGroupWithAnotherUser))
                     .Set(new ToDoGroupsController.ShareToDoGroupWithAnotherUserArgs { anotherUserId = user2.Id, toDoGroupId = toDoGroup.Id })
                     .ExecuteAsync();

                ToDoGroupDto[] toDoGroups = (await client2.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .FindEntriesAsync()).ToArray();

                ToDoItemDto[] toDoItems = (await client2.Controller<ToDoItemsController, ToDoItemDto>()
                    .Function(nameof(ToDoItemsController.GetMyToDoItems))
                    .FindEntriesAsync()).ToArray();


                Assert.AreEqual(1, toDoGroups.Length);
                Assert.AreEqual(1, toDoItems.Length);
            }
        }

        [TestMethod]
        public async Task OnlyOwnerCanDeleteTheToDoGroupTest()
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

                toDoGroup = (await client2.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .FindEntriesAsync()).Single();

                try
                {
                    await client2.Controller<ToDoGroupsController, ToDoGroupDto>()
                        .Key(toDoGroup.Id)
                        .DeleteEntryAsync();

                    Assert.Fail();
                }
                catch (WebRequestException exp) when (exp.Message == "KnownError" && JToken.Parse(exp.Response)["error"]["message"].Value<string>() == "OnlyOwnerCanDeleteTheToDoGroup")
                {

                }
            }
        }

        [TestMethod]
        public async Task OnlyOwnerCanKickUsersFromToDoGroupTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName1, client1) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                var (userName2, client2) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                //create ToDoGroup by first user
                ToDoGroupDto toDoGroup = await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                    .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Group1" })
                    .ExecuteAsSingleAsync();

                UserDto user2 = await client1.GetUserByUserName(userName2);

                await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.ShareToDoGroupWithAnotherUser))
                   .Set(new ToDoGroupsController.ShareToDoGroupWithAnotherUserArgs { anotherUserId = user2.Id, toDoGroupId = toDoGroup.Id })
                   .ExecuteAsSingleAsync();

                UserDto user1 = await client2.GetUserByUserName(userName2);

                try
                {
                    await client2.Controller<ToDoGroupsController, ToDoGroupDto>()
                        .Action(nameof(ToDoGroupsController.KickUserFromToDoGroup))
                        .Set(new ToDoGroupsController.KickAnotherUserFromMyToDoGroupArge { userId = user1.Id, toDoGroupId = toDoGroup.Id })
                        .ExecuteAsSingleAsync();

                    Assert.Fail();
                }
                catch (WebRequestException exp) when (exp.Message == "KnownError" && JToken.Parse(exp.Response)["error"]["message"].Value<string>() == "OnlyOwnerCanKickOtherUsers")
                {

                }
            }
        }

        [TestMethod]
        public async Task KickUserFromToDoGroupTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName1, client1) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                string userName2 = await testEnv.RegisterNewUser();

                //create ToDoGroup by first user
                ToDoGroupDto toDoGroup = await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                    .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Group1" })
                    .ExecuteAsSingleAsync();

                UserDto user2 = await client1.GetUserByUserName(userName2);

                await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.ShareToDoGroupWithAnotherUser))
                   .Set(new ToDoGroupsController.ShareToDoGroupWithAnotherUserArgs { anotherUserId = user2.Id, toDoGroupId = toDoGroup.Id })
                   .ExecuteAsSingleAsync();

                await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                  .Action(nameof(ToDoGroupsController.KickUserFromToDoGroup))
                  .Set(new ToDoGroupsController.KickAnotherUserFromMyToDoGroupArge { userId = user2.Id, toDoGroupId = toDoGroup.Id })
                  .ExecuteAsSingleAsync();

                ToDoGroupDto toDoGroupK = await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                   .Filter(tdg => toDoGroup.Id == tdg.Id)
                   .FindEntryAsync();

                Assert.AreEqual(1, toDoGroupK.SharedByCount);
            }
        }

        [TestMethod]
        public async Task KickingUserFromToDoGroupShouldRemoveItsToDoItemsTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName1, client1) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                var (userName2, client2) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                //create ToDoGroup by first user
                ToDoGroupDto toDoGroup = await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                    .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Group1" })
                    .ExecuteAsSingleAsync();

                ToDoItemDto toDoItem = await client1.Controller<ToDoItemsController, ToDoItemDto>()
                   .Set(new ToDoItemDto { Title = "Test1", ToDoGroupId = toDoGroup.Id })
                   .InsertEntryAsync();

                UserDto user2 = await client1.GetUserByUserName(userName2);

                await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.ShareToDoGroupWithAnotherUser))
                   .Set(new ToDoGroupsController.ShareToDoGroupWithAnotherUserArgs { anotherUserId = user2.Id, toDoGroupId = toDoGroup.Id })
                   .ExecuteAsSingleAsync();

                await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                  .Action(nameof(ToDoGroupsController.KickUserFromToDoGroup))
                  .Set(new ToDoGroupsController.KickAnotherUserFromMyToDoGroupArge { userId = user2.Id, toDoGroupId = toDoGroup.Id })
                  .ExecuteAsSingleAsync();

                ToDoGroupDto toDoGroupK = await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                   .Filter(tdg => toDoGroup.Id == tdg.Id)
                   .FindEntryAsync();

                bool hasToDoItem = (await client2.Controller<ToDoItemsController, ToDoItemDto>()
                    .Function(nameof(ToDoItemsController.GetMyToDoItems))
                    .FindEntriesAsync()).Any();

                Assert.AreEqual(1, toDoGroupK.SharedByCount);
                Assert.AreEqual(false, hasToDoItem);
            }
        }
    }
}
