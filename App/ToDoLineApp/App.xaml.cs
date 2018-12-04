using Autofac;
using Bit;
using Bit.ViewModel.Contracts;
using Bit.ViewModel.Implementations;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using System.Threading.Tasks;
using ToDoLineApp.ViewModels;
using ToDoLineApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Navigation;
using System;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace ToDoLineApp
{
    public partial class App : BitApplication
    {
        public new static App Current
        {
            get { return (App)Application.Current; }
        }

        public App()
            : this(null)
        {
        }

        public App(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
        {
#if DEBUG
            LiveReload.Init();
#endif
        }

        protected async override Task OnInitializedAsync()
        {
            InitializeComponent();

            /*
            
            Copy from XamApp => Splash Screen (Android) + ProGurad(Android) + Linker (iOS/Android)

            bool isLoggedIn = await Container.Resolve<ISecurityService>().IsLoggedInAsync();

            Add AcrUserDialogs & Loading for async-await

            if (isLoggedIn)
            {
                await NavigationService.NavigateAsync("/Nav/Main");
            }
            else
            {
                await NavigationService.NavigateAsync("/Login");
            }

            IEventAggregator eventAggregator = Container.Resolve<IEventAggregator>();

            eventAggregator.GetEvent<TokenExpiredEvent>()
                .SubscribeAsync(async tokenExpiredEvent => await NavigationService.NavigateAsync("/Login"), ThreadOption.UIThread); */

            await NavigationService.NavigateAsync("/Nav/Test", animated: false);

            await base.OnInitializedAsync();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ContainerBuilder containerBuilder = containerRegistry.GetBuilder();

            containerRegistry.RegisterForNav<NavigationPage>("Nav");
            containerRegistry.RegisterForNav<TestView, TestViewModel>("Test");

            containerBuilder.Register<IClientAppProfile>(c => new DefaultClientAppProfile
            {
                HostUri = new Uri("http://192.168.1.28:53149/"),
                ODataRoute = "odata/ToDoLine/",
                AppName = "ToDoLine",
            }).SingleInstance();

            containerBuilder.RegisterRequiredServices();
            containerBuilder.RegisterHttpClient();
            containerBuilder.RegisterODataClient();
            containerBuilder.RegisterIdentityClient();

            base.RegisterTypes(containerRegistry);
        }
    }
}
