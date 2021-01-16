using Acr.UserDialogs;
using Autofac;
using Bit.Core.Contracts;
using Bit.Core.Implementations;
using Bit.Core.Models.Events;
using Bit.Http.Contracts;
using Bit.View;
using Microsoft.Extensions.DependencyInjection;
using Prism;
using Prism.Events;
using Prism.Ioc;
using Simple.OData.Client;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ToDoLineApp.Contracts;
using ToDoLineApp.Implementations;
using ToDoLineApp.Resources.Strings;
using ToDoLineApp.ViewModels;
using ToDoLineApp.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace ToDoLineApp
{
    public partial class App
    {
        static App()
        {
            BitCSharpClientControls.XamlInit();
        }

        public new static App Current => (App)Xamarin.Forms.Application.Current;

        public App()
            : this(null)
        {
        }

        public App(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
        {
        }

        protected async override Task OnInitializedAsync()
        {
            InitializeComponent();

            Strings.Culture = Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en");

            On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            On<Windows>().SetImageDirectory("Assets");

            bool isLoggedIn = await Container.Resolve<ISecurityService>().IsLoggedInAsync();

            if (isLoggedIn)
            {
                await NavigationService.NavigateAsync("/Master/Nav/ToDoItems");
            }
            else
            {
                await NavigationService.NavigateAsync("/Nav/Login");
            }

            IEventAggregator eventAggregator = Container.Resolve<IEventAggregator>();

            eventAggregator.GetEvent<TokenExpiredEvent>()
                .SubscribeAsync(async tokenExpiredEvent => await NavigationService.NavigateAsync("/Nav/Login"), ThreadOption.UIThread);

            await base.OnInitializedAsync();
        }

        protected override void RegisterTypes(IDependencyManager dependencyManager, IContainerRegistry containerRegistry, ContainerBuilder containerBuilder, IServiceCollection services)
        {
            containerRegistry.RegisterForNav<NavigationPage>("Nav");
            containerRegistry.RegisterForNav<MasterView, MasterViewModel>("Master");
            containerRegistry.RegisterPartialView<MenuView, MenuViewModel>();
            containerRegistry.RegisterForNav<LoginView, LoginViewModel>("Login");
            containerRegistry.RegisterForNav<ToDoItemsView, ToDoItemsViewModel>("ToDoItems");

            const string developerMachineIp = "192.168.42.174";

            containerBuilder.Register<IClientAppProfile>(c => new DefaultClientAppProfile
            {
                HostUri = new Uri((Device.RuntimePlatform == Device.Android && DeviceInfo.DeviceType == DeviceType.Virtual) ? "http://10.0.2.2:53200" : Device.RuntimePlatform == Device.UWP ? "http://127.0.0.1:53200" : $"http://{developerMachineIp}:53200"),
                ODataRoute = "odata/ToDoLine/",
                AppName = "ToDoLine",
#if DEBUG
                Environment = "Development"
#else
                Environment = "Production"
#endif
            }).SingleInstance();

            dependencyManager.RegisterRequiredServices();
            dependencyManager.RegisterHttpClient();
            dependencyManager.RegisterODataClient((serviceProvider, settings) => settings.MetadataDocument = ToDoLineMetadata.MetadataString);
            dependencyManager.RegisterIdentityClient();

            containerBuilder.Register(c => UserDialogs.Instance).SingleInstance();
            dependencyManager.Register<IToDoService, DefaultToDoServie>(lifeCycle: DependencyLifeCycle.SingleInstance);

            base.RegisterTypes(dependencyManager, containerRegistry, containerBuilder, services);
        }
    }
}