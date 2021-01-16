using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Autofac;
using Bit.Android;
using Bit.ViewModel;
using Bit.ViewModel.Implementations;
using Bumptech.Glide;
using Prism.Autofac;
using Prism.Ioc;
using System.Reflection;
using System.Threading.Tasks;
using ToDoLineApp.Implementations;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ToDoLineApp.Droid
{
    [Activity(Label = "ToDoLineApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : BitFormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            BitExceptionHandler.Current = new ToDoLineExceptionHandler();

            SetTheme(Resource.Style.MainTheme);

            base.OnCreate(savedInstanceState);

            UseDefaultConfiguration(savedInstanceState);

            UserDialogs.Init(this);

            Assembly[] toBeLoadedAssemblies = new[]
            {
                Assembly.Load("Syncfusion.Core.XForms.Android"),
                Assembly.Load("Syncfusion.SfBadgeView.XForms.Android"),
                Assembly.Load("Syncfusion.Buttons.XForms.Android"),
                Assembly.Load("Syncfusion.SfListView.XForms.Android")
            };

            Forms.Init(this, savedInstanceState);

            Android.Glide.Forms.Init(this
#if DEBUG
                , debug: true);
#else
                , debug: false);
#endif

            if (VersionTracking.IsFirstLaunchForCurrentVersion || VersionTracking.IsFirstLaunchForCurrentBuild
#if DEBUG
                || true
#endif
                )
            {
                Task.Run(() =>
                {
                    Glide.Get(this).ClearDiskCache();
                });
            }

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
