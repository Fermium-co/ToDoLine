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
    public class UserRegistrationControllerTests
    {
        [TestMethod]
        public async Task UserAfterRegistrationShouldHaveOneDefaultToDoGroupTest()
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

                var defaultToDoGroup = (await client.Controller<ToDoGroupsController, ToDoGroupDto>()
                    .Function(nameof(ToDoGroupsController.GetMyToDoGroups))
                    .FindEntriesAsync()).Single();

                Assert.AreEqual(true, defaultToDoGroup.IsDefault);
                Assert.AreEqual(1, defaultToDoGroup.SharedByCount);
                Assert.AreEqual("Tasks", defaultToDoGroup.Title);
            }
        }
    }
}
