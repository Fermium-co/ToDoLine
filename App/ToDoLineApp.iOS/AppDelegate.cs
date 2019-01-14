using Autofac;
using Bit.iOS;
using Bit.ViewModel;
using Bit.ViewModel.Implementations;
using FFImageLoading.Forms.Platform;
using Foundation;
using Prism.Autofac;
using Prism.Ioc;
using Syncfusion.XForms.iOS.BadgeView;
using Syncfusion.XForms.iOS.Buttons;
using Syncfusion.XForms.iOS.TextInputLayout;
using ToDoLineApp.Implementations;
using UIKit;
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

            CachedImageRenderer.Init();

            Forms.Init();

            LoadApplication(new App(new ToDoLinePlatformInitializer()));

            return base.FinishedLaunching(app, options);
        }
    }

    public class ToDoLinePlatformInitializer : BitPlatformInitializer
    {
        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ContainerBuilder containerBuilder = containerRegistry.GetBuilder();

            base.RegisterTypes(containerRegistry);
        }
    }
}
