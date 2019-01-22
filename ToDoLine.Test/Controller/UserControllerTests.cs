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
                var loginResult = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

                UserDto currentUser = await loginResult.ODataClient.Controller<UsersController, UserDto>()
                    .Function(nameof(UsersController.GetCurrentUser))
                    .FindEntryAsync();

                Assert.AreEqual(loginResult.UserName, currentUser.UserName);
            }
        }
    }
}
