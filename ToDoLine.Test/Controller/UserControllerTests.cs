using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.OData.Client;
using System.Threading.Tasks;
using ToDoLine.Dto;

namespace ToDoLine.Test.Controller
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public async Task GetCurrentUserTest()
        {
            using ToDoLineTestEnv testEnv = new ToDoLineTestEnv();

            ToDoLineClient toDoLineClient = await testEnv.LoginInToApp(registerNewUserByRandomUserName: true);

            UserDto currentUser = await toDoLineClient.ODataClient.Users()
                .GetCurrentUser()
                .FindEntryAsync();

            Assert.AreEqual(toDoLineClient.UserName, currentUser.UserName);
        }
    }
}
