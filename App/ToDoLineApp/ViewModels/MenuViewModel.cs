using Bit.ViewModel;
using Prism.Navigation;
using System.Threading;
using System.Threading.Tasks;
using ToDoLineApp.Contracts;

namespace ToDoLineApp.ViewModels
{
    public class MenuViewModel : BitViewModelBase
    {
        public virtual IToDoService ToDoService { get; set; }

        public async override Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await base.OnNavigatedToAsync(parameters);

            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                await ToDoService.LoadDataAsync(CancellationToken.None);
            }
        }
    }
}
