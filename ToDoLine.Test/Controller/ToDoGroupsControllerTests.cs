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
                IODataClient client = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName = Guid.NewGuid().ToString("N");

                await client.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs { userRegistration = new UserRegistrationDto { UserName = userName, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                var token = await testEnv.Server.Login(userName, "P@ssw0rd", "ToDoLine", "secret");

                client = testEnv.Server.BuildODataClient(token: token, odataRouteName: "ToDoLine");

                ToDoGroupDto addedToDoGroup = await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                   .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Test" })
                   .ExecuteAsSingleAsync();

                Assert.AreEqual(false, addedToDoGroup.IsDefault);
                Assert.AreEqual(1, addedToDoGroup.SharedByCount);
                Assert.AreEqual("Test", addedToDoGroup.Title);

                var toDoGroups = (await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .FindEntriesAsync()).ToArray();

                Assert.AreEqual(2, toDoGroups.Length);
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
        public async Task UpdateDefaultToDoGroupTitleIsDeniedTest()
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

                ToDoGroupDto toDoGroup = (await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                   .FindEntriesAsync()).Single();

                toDoGroup.Title += "?"; // Tasks?

                try
                {
                    await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                        .Key(toDoGroup.Id)
                        .Set(toDoGroup)
                        .UpdateEntryAsync();

                    Assert.Fail();
                }
                catch (WebRequestException exp) when (exp.Message == "KnownError" && JToken.Parse(exp.Response)["error"]["message"].Value<string>() == "CanNotChangeTitleOfDefaultToDoGroup")
                {

                }
            }
        }

        [TestMethod]
        public async Task DeleteToDoGroupTest()
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

                await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Key(toDoGroup.Id)
                    .DeleteEntryAsync();

                (await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .FindEntriesAsync()).Single();
            }
        }

        [TestMethod]
        public async Task DeleteDefaultToDoGroupIsDeniedTest()
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

                ToDoGroupDto defaultToDoGroup = (await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                   .FindEntriesAsync()).Single();

                try
                {
                    await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                        .Key(defaultToDoGroup.Id)
                        .DeleteEntryAsync();

                    Assert.Fail();
                }
                catch (WebRequestException exp) when (exp.Message == "KnownError" && JToken.Parse(exp.Response)["error"]["message"].Value<string>() == "CanNotDeleteDefaultToDoGroup")
                {

                }
            }
        }

        [TestMethod]
        public async Task CanNotShareDefaultToDoGroupTest()
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

                ToDoGroupDto toDoGroup = (await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .FindEntriesAsync()).Single();

                try
                {
                    await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                         .Action(nameof(ToDoGroupsController.ShareToDoGroupWithAnotherUser))
                         .Set(new ToDoGroupsController.ShareToDoGroupWithAnotherUserArgs { anotherUserId = Guid.Empty, toDoGroupId = toDoGroup.Id })
                         .ExecuteAsync();

                    Assert.Fail();
                }
                catch (WebRequestException exp) when (exp.Message == "KnownError" && JToken.Parse(exp.Response)["error"]["message"].Value<string>() == "CanNotShareDefaultToDoGroup")
                {

                }
            }
        }

        [TestMethod]
        public async Task ShareToDoGroupWithAnotherUserTest()
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

                var toDoGroups = (await client2.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .FindEntriesAsync()).ToArray();

                Assert.AreEqual(2, toDoGroups.Length);
            }
        }

        [TestMethod]
        public async Task OnlyOwnerCanDeleteTheToDoGroupTest()
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

                toDoGroup = (await client2.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .Filter(tdg => tdg.IsDefault == false)
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
                //first user
                IODataClient client1 = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName1 = Guid.NewGuid().ToString("N");

                await client1.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs {  userRegistration = new UserRegistrationDto { UserName = userName1, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                var token1 = await testEnv.Server.Login(userName1, "P@ssw0rd", "ToDoLine", "secret");

                client1 = testEnv.Server.BuildODataClient(token: token1, odataRouteName: "ToDoLine");

                //second user
                IODataClient client2 = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName2 = Guid.NewGuid().ToString("N");

                await client2.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs { userRegistration = new UserRegistrationDto { UserName = userName2, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                var token2 = await testEnv.Server.Login(userName2, "P@ssw0rd", "ToDoLine", "secret");

                client2 = testEnv.Server.BuildODataClient(token: token2, odataRouteName: "ToDoLine");

                //create ToDoGroup by first user
                ToDoGroupDto toDoGroup = await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                    .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Group1" })
                    .ExecuteAsSingleAsync();

                UserDto user2 = await client1.Controller<UsersController, UserDto>()
                     .Function(nameof(UsersController.GetAllUsers))
                     .Filter(u => u.UserName.ToLower().Contains(userName2))
                     .FindEntryAsync();

                await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                   .Action(nameof(ToDoGroupsController.ShareToDoGroupWithAnotherUser))
                   .Set(new ToDoGroupsController.ShareToDoGroupWithAnotherUserArgs { anotherUserId = user2.Id, toDoGroupId = toDoGroup.Id })
                   .ExecuteAsSingleAsync();

                UserDto user1 = await client2.Controller<UsersController, UserDto>()
                     .Function(nameof(UsersController.GetAllUsers))
                     .Filter(u => u.UserName.ToLower().Contains(userName1))
                     .FindEntryAsync();

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
                //first user
                IODataClient client1 = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName1 = Guid.NewGuid().ToString("N");

                await client1.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs { userRegistration = new UserRegistrationDto { UserName = userName1, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                var token1 = await testEnv.Server.Login(userName1, "P@ssw0rd", "ToDoLine", "secret");

                client1 = testEnv.Server.BuildODataClient(token: token1, odataRouteName: "ToDoLine");

                //second user
                IODataClient client2 = testEnv.Server.BuildODataClient(odataRouteName: "ToDoLine");

                string userName2 = Guid.NewGuid().ToString("N");

                await client2.Controller<UserRegistrationController, UserRegistrationDto>()
                    .Action(nameof(UserRegistrationController.Register))
                    .Set(new UserRegistrationController.RegisterArgs { userRegistration = new UserRegistrationDto { UserName = userName2, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                //create ToDoGroup by first user
                ToDoGroupDto toDoGroup = await client1.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Action(nameof(ToDoGroupsController.CreateToDoGroup))
                    .Set(new ToDoGroupsController.CreateToDoGroupArgs { title = "Group1" })
                    .ExecuteAsSingleAsync();

                UserDto user2 = await client1.Controller<UsersController, UserDto>()
                     .Function(nameof(UsersController.GetAllUsers))
                     .Filter(u => u.UserName.ToLower().Contains(userName2))
                     .FindEntryAsync();

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
    }
}
