using Bit.Core.Contracts;
using Bit.Test;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoLine.Dto;

namespace ToDoLine.Test.Controller
{
    [TestClass]
    public class ToDoItemsControllerTest
    {
        [TestMethod]
        public async Task CreateToDoItemTest()
        {
            using ToDoLineTestEnv testEnv = new ToDoLineTestEnv();

            ToDoLineClient toDoLineClient = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

            ToDoGroupDto toDoGroup = await toDoLineClient.ODataClient.ToDoGroups()
               .CreateToDoGroup("Test")
               .ExecuteAsSingleAsync();

            ToDoItemDto toDoItem = await toDoLineClient.ODataClient.ToDoItems()
               .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ToDoGroupId = toDoGroup.Id })
               .InsertEntryAsync();

            Assert.AreEqual("Task1", toDoItem.Title);
        }

        [TestMethod]
        public async Task CreateToDoItemWithoutToDoGroupTest()
        {
            using ToDoLineTestEnv testEnv = new ToDoLineTestEnv();

            ToDoLineClient toDoLineClient = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

            ToDoItemDto toDoItem = await toDoLineClient.ODataClient.ToDoItems()
               .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ToDoGroupId = null })
               .InsertEntryAsync();

            Assert.AreEqual("Task1", toDoItem.Title);
        }

        [TestMethod]
        public async Task CreatingToDoItemShouldCreateToDoItemOptionsPerUserTest()
        {
            using ToDoLineTestEnv testEnv = new ToDoLineTestEnv();

            ToDoLineClient toDoLineClient1 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

            ToDoGroupDto toDoGroup = await toDoLineClient1.ODataClient.ToDoGroups()
               .CreateToDoGroup("Test")
               .ExecuteAsSingleAsync();

            ToDoLineClient toDoLineClient2 = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

            UserDto user2 = await toDoLineClient1.ODataClient.GetUserByUserName(toDoLineClient2.UserName);

            await toDoLineClient1.ODataClient.ToDoGroups()
                 .ShareToDoGroupWithAnotherUser(anotherUserId: user2.Id, toDoGroupId: toDoGroup.Id)
                 .ExecuteAsync();

            ToDoItemDto toDoItem = await toDoLineClient2.ODataClient.ToDoItems()
               .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ToDoGroupId = toDoGroup.Id })
               .InsertEntryAsync();

            bool hasToDoItem = (await toDoLineClient1.ODataClient.ToDoItems()
                .GetMyToDoItems()
                .FindEntriesAsync()).Any();

            Assert.AreEqual(true, hasToDoItem);
        }

        [TestMethod]
        public async Task ActiveShowInMyDayToDoItemTest()
        {
            using ToDoLineTestEnv testEnv = new ToDoLineTestEnv(new TestEnvironmentArgs
            {
                AdditionalDependencies = (dependencyManager, services) =>
                {
                    IDateTimeProvider dateTimeProvider = A.Fake<IDateTimeProvider>();

                    A.CallTo(() => dateTimeProvider.GetCurrentUtcDateTime())
                        .Returns(new DateTimeOffset(2018, 1, 1, 10, 10, 10, TimeSpan.Zero));

                    dependencyManager.RegisterInstance(dateTimeProvider);
                }
            });

            ToDoLineClient toDoLineClient = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

            UserDto user = await toDoLineClient.ODataClient.GetUserByUserName(toDoLineClient.UserName);

            ToDoGroupDto toDoGroup = await toDoLineClient.ODataClient.ToDoGroups()
                              .CreateToDoGroup("Test")
                              .ExecuteAsSingleAsync();

            ToDoItemDto toDoItem = await toDoLineClient.ODataClient.ToDoItems()
               .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ShowInMyDay = true, ToDoGroupId = toDoGroup.Id })
               .InsertEntryAsync();

            Assert.AreEqual("Task1", toDoItem.Title);

            Assert.AreEqual(true, toDoItem.ShowInMyDay);
        }

        [TestMethod]
        public async Task UpdateToDoItemTest()
        {
            using ToDoLineTestEnv testEnv = new ToDoLineTestEnv();

            ToDoLineClient toDoLineClient = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

            ToDoGroupDto toDoGroup = await toDoLineClient.ODataClient.ToDoGroups()
               .CreateToDoGroup("Test")
               .ExecuteAsSingleAsync();

            ToDoItemDto toDoItem = await toDoLineClient.ODataClient.ToDoItems()
               .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ToDoGroupId = toDoGroup.Id })
               .InsertEntryAsync();

            toDoItem.Title += "!";
            toDoItem.IsCompleted = true;

            ToDoItemDto updatedToDoItem = await toDoLineClient.ODataClient.ToDoItems()
               .Key(toDoItem.Id)
               .Set(toDoItem)
               .UpdateEntryAsync();

            Assert.AreEqual("Task1!", updatedToDoItem.Title);
            Assert.AreEqual(true, updatedToDoItem.IsCompleted);
            Assert.AreEqual(toDoLineClient.UserName, updatedToDoItem.CompletedBy);
        }

        [TestMethod]
        public async Task UpdateToDoItemToDoGroupIdIsNotSupportedTest()
        {
            using ToDoLineTestEnv testEnv = new ToDoLineTestEnv();

            ToDoLineClient toDoLineClient = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

            ToDoItemDto toDoItem = await toDoLineClient.ODataClient.ToDoItems()
               .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ToDoGroupId = null })
               .InsertEntryAsync();

            toDoItem.Title += "!";
            toDoItem.IsCompleted = true;
            toDoItem.ToDoGroupId = Guid.NewGuid();

            try
            {
                await toDoLineClient.ODataClient.ToDoItems()
                   .Key(toDoItem.Id)
                   .Set(toDoItem)
                   .UpdateEntryAsync();

                Assert.Fail();
            }
            catch (WebRequestException exp) when (exp.Message == "KnownError" && JToken.Parse(exp.Response)["error"]["message"].Value<string>() == "ChangingToDoGroupIdIsNotSupportedAtTheMoment")
            {

            }
        }

        [TestMethod]
        public async Task DeleteToDoItemTest()
        {
            using ToDoLineTestEnv testEnv = new ToDoLineTestEnv();

            ToDoLineClient toDoLineClient = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

            ToDoGroupDto toDoGroup = await toDoLineClient.ODataClient.ToDoGroups()
               .CreateToDoGroup("Test")
               .ExecuteAsSingleAsync();

            ToDoItemDto toDoItem = await toDoLineClient.ODataClient.ToDoItems()
               .Set(new ToDoItemDto { Title = "Task1", Notes = "Hi this is the first sample", ToDoGroupId = toDoGroup.Id })
               .InsertEntryAsync();

            await toDoLineClient.ODataClient.ToDoItems()
                .Key(toDoItem.Id)
                .DeleteEntryAsync();
        }
    }
}
