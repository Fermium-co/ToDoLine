using Acr.UserDialogs;
using Autofac;
using Bit;
using Bit.View;
using FFImageLoading;
using FFImageLoading.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Prism;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Prism.Services;
using ToDoLineApp.Contracts;
using ToDoLineApp.Implementations;
using ToDoLineApp.ViewModels;
using ToDoLineApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Bit.Core.Models.Events;
using Bit.Http.Contracts;
using Bit.Core.Contracts;
using Bit.Core.Implementations;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace ToDoLineApp
{
    public partial class App : IMiniLogger
    {
        public void Debug(string message)
        {
        }

        public void Error(string errorMessage)
        {
            System.Diagnostics.Debugger.Break();
        }

        public void Error(string errorMessage, Exception ex)
        {
            System.Diagnostics.Debugger.Break();
        }
    }

    public partial class App : BitApplication
    {
        static App()
        {
#if DEBUG
            Xamarin.Forms.Internals.Log.Listeners.Add(new Xamarin.Forms.Internals.DelegateLogListener((category, message) => Console.WriteLine($"{category} {message}")));
#endif
            BitCSharpClientControls.XamlInit();
        }

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
        }

        protected async override Task OnInitializedAsync()
        {
            ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration
            {
                HttpClient = Container.Resolve<HttpClient>(),
                AllowUpscale = false,
                ClearMemoryCacheOnOutOfMemory = true,
#if DEBUG
                Logger = this
#endif
            });

            InitializeComponent();

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

            const string developerMachineIp = "192.168.43.153";

            containerBuilder.Register<IClientAppProfile>(c => new DefaultClientAppProfile
            {
                HostUri = new Uri((Device.RuntimePlatform == Device.Android && Xamarin.Essentials.DeviceInfo.DeviceType == DeviceType.Virtual) ? "http://10.0.2.2:53200" : Device.RuntimePlatform == Device.UWP ? "http://127.0.0.1:53200" : $"http://{developerMachineIp}:53200"),
                ODataRoute = "odata/ToDoLine/",
                AppName = "ToDoLine",
            }).SingleInstance();

            dependencyManager.RegisterRequiredServices();
            dependencyManager.RegisterHttpClient();
            dependencyManager.RegisterODataClient();
            dependencyManager.RegisterIdentityClient();

            containerBuilder.Register(c => UserDialogs.Instance).SingleInstance();
            dependencyManager.Register<IToDoService, DefaultToDoServie>(lifeCycle: DependencyLifeCycle.SingleInstance);

            base.RegisterTypes(dependencyManager, containerRegistry, containerBuilder, services);
        }
    }
}