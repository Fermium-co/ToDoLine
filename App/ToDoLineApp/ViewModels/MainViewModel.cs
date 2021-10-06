using Bit.ViewModel;
using Prism.Navigation;
using Prism.Regions;
using System.Threading.Tasks;

namespace ToDoLineApp.ViewModels
{
    public class MainViewModel : BitViewModelBase
    {
        public async override Task OnNavigatedToAsync(INavigationParameters parameters)
        {
            if (parameters.TryGetNavigationMode(out NavigationMode navigationMode) && navigationMode == NavigationMode.New)
            {
                await RegionManager.NavigateAsync("FlyoutRegion", "Menu", parameters);
            }

            await base.OnNavigatedToAsync(parameters);
        }

        public async override Task OnDestroyAsync()
        {
            RegionManager.DestroyRegion("FlyoutRegion");

            await base.OnDestroyAsync();
        }
    }
}
