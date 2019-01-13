using Acr.UserDialogs;
using Bit.ViewModel;
using Bit.ViewModel.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using ToDoLineApp.Resources.Strings;

namespace ToDoLineApp.ViewModels
{
    public class LoginViewModel : BitViewModelBase
    {
        public BitDelegateCommand LoginCommand { get; set; }

        public ISecurityService SecurityService { get; set; }

        public IUserDialogs UserDialogs { get; set; }

        public string UserName { get; set; }

        public bool UserName_HasError => UserName == null || !string.IsNullOrEmpty(UserName) ? false /*Valid*/ : true /*Invalid*/;

        public string UserName_ErrorMessage => Strings.UserNameIsRequired;

        public string Password { get; set; }

        public bool Password_HasError => Password == null || !string.IsNullOrEmpty(Password) ? false : true;

        public string Password_ErrorMessage => Strings.PasswordIsRequired;

        public LoginViewModel()
        {
            LoginCommand = new BitDelegateCommand(Login);
        }

        async Task Login()
        {
            try
            {
                UserName = UserName ?? "";
                Password = Password ?? "";

                if (UserName_HasError || Password_HasError)
                    return;

                using (UserDialogs.Loading(Strings.Login, out CancellationToken cancellationToken))
                {
                    await Task.Run(async () =>
                    {
                        await SecurityService.LoginWithCredentials(UserName, Password, "ToDoLineApp", "secret", cancellationToken: cancellationToken);
                    }, cancellationToken);
                }

                await NavigationService.NavigateAsync("/Master/Nav/ToDoItems");
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("invalid_grant "))
                {
                    string error_description = JToken.Parse(ex.Message.Replace("invalid_grant ", ""))["error_description"].Value<string>();

                    if (error_description == "InvalidUserNameAndOrPassword")
                    {
                        await UserDialogs.AlertAsync(Strings.InvalidUserNameAndOrPassword, Strings.Error);
                        return; // don't throw it.
                    }
                }

                throw;
            }
        }
    }
}
