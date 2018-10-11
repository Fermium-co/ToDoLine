using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.OData.Client;
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
                var (userName, client) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

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
