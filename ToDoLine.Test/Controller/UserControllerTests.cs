using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.OData.Client;
using System.Threading.Tasks;
using ToDoLine.Controller;
using ToDoLine.Dto;

namespace ToDoLine.Test.Controller
{

    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public async Task GetCurrentUserTest()
        {
            using (ToDoLineTestEnv testEnv = new ToDoLineTestEnv())
            {
                var (userName, client) = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                UserDto currentUser = await client.Controller<UsersController, UserDto>()
                    .Function(nameof(UsersController.GetCurrentUser))
                    .FindEntryAsync();

                Assert.AreEqual(userName, currentUser.UserName);
            }
        }
    }
}
