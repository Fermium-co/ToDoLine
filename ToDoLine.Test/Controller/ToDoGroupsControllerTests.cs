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
                var loginResult = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto addedToDoGroup = await loginResult.ODataClient.ToDoGroups()
                   .CreateToDoGroup("Test")
                   .ExecuteAsSingleAsync();

                Assert.AreEqual(1, addedToDoGroup.SharedByCount);
                Assert.AreEqual("Test", addedToDoGroup.Title);

                var toDoGroups = (await loginResult.ODataClient.ToDoGroups()
                    .GetMyToDoGroups()
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
                var loginResult = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await loginResult.ODataClient.ToDoGroups()
                   .CreateToDoGroup("Test")
                   .ExecuteAsSingleAsync();

                toDoGroup.Title += "?"; // Test?
                toDoGroup.SortedBy = SortBy.Importance;

                toDoGroup = await loginResult.ODataClient.ToDoGroups()
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
                var loginResult = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await loginResult.ODataClient.ToDoGroups()
                   .CreateToDoGroup("test")
                   .ExecuteAsSingleAsync();

                await loginResult.ODataClient.ToDoGroups()
                    .Key(toDoGroup.Id)
                    .DeleteEntryAsync();

                Assert.AreEqual(0, (await loginResult.ODataClient.ToDoGroups()
                    .GetMyToDoGroups()
                    .FindEntriesAsync()).Count());
            }
        }

        [TestMethod]
        public async Task ShareToDoGroupWithAnotherUserTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var loginResult1 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await loginResult1.ODataClient.ToDoGroups()
                   .CreateToDoGroup("Test")
                   .ExecuteAsSingleAsync();

                var loginResult2 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                UserDto user2 = await loginResult1.ODataClient.GetUserByUserName(loginResult2.UserName);

                await loginResult1.ODataClient.ToDoGroups()
                     .ShareToDoGroupWithAnotherUser(anotherUserId: user2.Id, toDoGroupId: toDoGroup.Id)
                     .ExecuteAsync();

                var toDoGroups = (await loginResult2.ODataClient.ToDoGroups()
                    .GetMyToDoGroups()
                    .FindEntriesAsync()).ToArray();

                Assert.AreEqual(1, toDoGroups.Length);
            }
        }

        [TestMethod]
        public async Task SharingToDoGroupWithAnotherUserShouldShareItsToDoItemsTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var loginResult1 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await loginResult1.ODataClient.ToDoGroups()
                   .CreateToDoGroup("Test")
                   .ExecuteAsSingleAsync();

                ToDoItemDto toDoItem = await loginResult1.ODataClient.ToDoItems()
                   .Set(new ToDoItemDto { Title = "Test1", ToDoGroupId = toDoGroup.Id })
                   .InsertEntryAsync();

                var loginResult2 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                UserDto user2 = await loginResult1.ODataClient.GetUserByUserName(loginResult2.UserName);

                await loginResult1.ODataClient.ToDoGroups()
                     .ShareToDoGroupWithAnotherUser(anotherUserId: user2.Id, toDoGroupId: toDoGroup.Id)
                     .ExecuteAsync();

                ToDoGroupDto[] toDoGroups = (await loginResult2.ODataClient.ToDoGroups()
                    .GetMyToDoGroups()
                    .FindEntriesAsync()).ToArray();

                ToDoItemDto[] toDoItems = (await loginResult2.ODataClient.ToDoItems()
                    .GetMyToDoItems()
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
                var loginResult1 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                ToDoGroupDto toDoGroup = await loginResult1.ODataClient.ToDoGroups()
                   .CreateToDoGroup("Test")
                   .ExecuteAsSingleAsync();

                var loginResult2 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                UserDto user2 = await loginResult1.ODataClient.GetUserByUserName(loginResult2.UserName);

                await loginResult1.ODataClient.ToDoGroups()
                     .ShareToDoGroupWithAnotherUser(anotherUserId: user2.Id, toDoGroupId: toDoGroup.Id)
                     .ExecuteAsync();

                toDoGroup = (await loginResult2.ODataClient.ToDoGroups()
                    .GetMyToDoGroups()
                    .FindEntriesAsync()).Single();

                try
                {
                    await loginResult2.ODataClient.ToDoGroups()
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
                var loginResult1 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                var loginResult2 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                //create ToDoGroup by first user
                ToDoGroupDto toDoGroup = await loginResult1.ODataClient.ToDoGroups()
                    .CreateToDoGroup("Group1")
                    .ExecuteAsSingleAsync();

                UserDto user2 = await loginResult1.ODataClient.GetUserByUserName(loginResult2.UserName);

                await loginResult1.ODataClient.ToDoGroups()
                   .ShareToDoGroupWithAnotherUser(anotherUserId: user2.Id, toDoGroupId: toDoGroup.Id)
                   .ExecuteAsSingleAsync();

                UserDto user1 = await loginResult2.ODataClient.GetUserByUserName(loginResult2.UserName);

                try
                {
                    await loginResult2.ODataClient.ToDoGroups()
                        .KickUserFromToDoGroup(userId: user1.Id, toDoGroupId: toDoGroup.Id)
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
                var loginResult1 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                string userName2 = await testEnv.RegisterNewUser();

                //create ToDoGroup by first user
                ToDoGroupDto toDoGroup = await loginResult1.ODataClient.ToDoGroups()
                    .CreateToDoGroup("Group1")
                    .ExecuteAsSingleAsync();

                UserDto user2 = await loginResult1.ODataClient.GetUserByUserName(userName2);

                await loginResult1.ODataClient.ToDoGroups()
                   .ShareToDoGroupWithAnotherUser(anotherUserId: user2.Id, toDoGroupId: toDoGroup.Id)
                   .ExecuteAsSingleAsync();

                await loginResult1.ODataClient.ToDoGroups()
                  .KickUserFromToDoGroup(userId: user2.Id, toDoGroupId: toDoGroup.Id)
                  .ExecuteAsSingleAsync();

                ToDoGroupDto toDoGroupK = await loginResult1.ODataClient.ToDoGroups()
                   .GetMyToDoGroups()
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
                var loginResult1 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                var loginResult2 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                //create ToDoGroup by first user
                ToDoGroupDto toDoGroup = await loginResult1.ODataClient.ToDoGroups()
                    .CreateToDoGroup("Group1")
                    .ExecuteAsSingleAsync();

                ToDoItemDto toDoItem = await loginResult1.ODataClient.ToDoItems()
                   .Set(new ToDoItemDto { Title = "Test1", ToDoGroupId = toDoGroup.Id })
                   .InsertEntryAsync();

                UserDto user2 = await loginResult1.ODataClient.GetUserByUserName(loginResult2.UserName);

                await loginResult1.ODataClient.ToDoGroups()
                   .ShareToDoGroupWithAnotherUser(toDoGroupId: toDoGroup.Id, anotherUserId: user2.Id)
                   .ExecuteAsSingleAsync();

                await loginResult1.ODataClient.ToDoGroups()
                  .KickUserFromToDoGroup(userId: user2.Id, toDoGroupId: toDoGroup.Id)
                  .ExecuteAsSingleAsync();

                ToDoGroupDto toDoGroupK = await loginResult1.ODataClient.ToDoGroups()
                   .GetMyToDoGroups()
                   .Filter(tdg => toDoGroup.Id == tdg.Id)
                   .FindEntryAsync();

                bool hasToDoItem = (await loginResult2.ODataClient.ToDoItems()
                    .GetMyToDoItems()
                    .FindEntriesAsync()).Any();

                Assert.AreEqual(1, toDoGroupK.SharedByCount);
                Assert.AreEqual(false, hasToDoItem);
            }
        }
    }
}
