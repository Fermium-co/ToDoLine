using Bit.ViewModel;
using Prism.Navigation;
using Prism.Regions;
using System.Threading.Tasks;

namespace ToDoLineApp.ViewModels
{
    public class MasterViewModel : BitViewModelBase
    {
        public async override Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            await RegionManager.RequestNavigateAsync("MenuRegion", "Menu");

            await base.OnNavigatedToAsync(parameters);
        }
    }
}
