using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Autofac;
using Bit.Droid;
using Bit.ViewModel;
using Bit.ViewModel.Implementations;
using Prism.Autofac;
using Prism.Ioc;
using ToDoLineApp.Implementations;
using Xamarin.Forms;

namespace ToDoLineApp.Droid
{
    [Activity(Label = "ToDoLineApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : BitFormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            BitExceptionHandler.Current = new ToDoLineExceptionHandler();

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            SetTheme(Resource.Style.MainTheme);

            base.OnCreate(savedInstanceState);

            UseDefaultConfiguration(savedInstanceState);

            UserDialogs.Init(this);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

            Forms.Init(this, savedInstanceState);

            LoadApplication(new App(new ToDoLinePlatformInitializer(this)));
        }
    }

    public class ToDoLinePlatformInitializer : BitPlatformInitializer
    {
        public ToDoLinePlatformInitializer(Activity activity)
            : base(activity)
        {
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ContainerBuilder containerBuilder = containerRegistry.GetBuilder();

            base.RegisterTypes(containerRegistry);
        }
    }
}
