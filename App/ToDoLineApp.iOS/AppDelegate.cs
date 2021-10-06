using Autofac;
using Bit.Core.Contracts;
using Bit.iOS;
using Bit.ViewModel;
using Bit.ViewModel.Implementations;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using Prism.Autofac;
using Prism.Ioc;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.XForms.iOS.BadgeView;
using Syncfusion.XForms.iOS.Buttons;
using Syncfusion.XForms.iOS.TextInputLayout;
using System.Threading.Tasks;
using ToDoLineApp.Implementations;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ToDoLineApp.iOS
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : BitFormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            BitExceptionHandler.Current = new ToDoLineExceptionHandler();

            UseDefaultConfiguration();

            SfTextInputLayoutRenderer.Init();

            SfBadgeViewRenderer.Init();

            SfButtonRenderer.Init();

            SfListViewRenderer.Init();

            Forms.Init();

            if (VersionTracking.IsFirstLaunchForCurrentVersion || VersionTracking.IsFirstLaunchForCurrentBuild
#if DEBUG
|| true
#endif
)
            {
                Task.Run(() =>
                {
                    Xamarin.Forms.Nuke.NukeController.ClearCache();
                });
            }

            LoadApplication(new App(new ToDoLinePlatformInitializer()));

            return base.FinishedLaunching(app, options);
        }
    }

    public class ToDoLinePlatformInitializer : BitPlatformInitializer
    {
        public override void RegisterTypes(IDependencyManager dependencyManager, IContainerRegistry containerRegistry, ContainerBuilder containerBuilder, IServiceCollection services)
        {
            base.RegisterTypes(dependencyManager, containerRegistry, containerBuilder, services);
        }
    }
}
