using Bit.ViewModel;
using Bit.ViewModel.Contracts;
using Prism.Navigation;
using Simple.OData.Client;
using System;
using System.Threading.Tasks;
using ToDoLine.Dto;

namespace ToDoLineApp.ViewModels
{
    public class TestViewModel : BitViewModelBase
    {
        public virtual BitDelegateCommand TestCommand { get; set; }

        public virtual ISecurityService SecurityService { get; set; }

        public virtual IODataClient ODataClient { get; set; }

        public TestViewModel()
        {
            TestCommand = new BitDelegateCommand(Test);
        }

        async Task Test()
        {
            if (await SecurityService.IsLoggedInAsync() == false)
            {
                string userName = Guid.NewGuid().ToString("N");

                await ODataClient.For<UserRegistrationDto>("UserRegistration")
                    .Action("Register")
                    .Set(new { userRegistration = new UserRegistrationDto { UserName = userName, Password = "P@ssw0rd" } })
                    .ExecuteAsync();

                await SecurityService.LoginWithCredentials(userName, "P@ssw0rd", "ToDoLineApp", "secret");
            }
        }
    }
}
