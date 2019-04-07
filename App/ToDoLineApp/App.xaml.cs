using Acr.UserDialogs;
using Autofac;
using Bit;
using Bit.Model.Events;
using Bit.ViewModel.Contracts;
using Bit.ViewModel.Implementations;
using FFImageLoading;
using FFImageLoading.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prism;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ToDoLineApp.Contracts;
using ToDoLineApp.Implementations;
using ToDoLineApp.ViewModels;
using ToDoLineApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            Xamarin.Forms.Internals.Log.Listeners.Add(new Xamarin.Forms.Internals.DelegateLogListener((category, message) => throw new Exception($"{category} {message}")));
#endif
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
#if DEBUG
            LiveReload.Init();
#endif
        }

        public class User
        {
            public int id { get; set; }
            public string title { get; set; }
            public string body { get; set; }
            public int userId { get; set; }
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

            HttpClient client = Container.Resolve<HttpClient>();

            await client.GetAsync("api/swagger/docs/v1");

            HttpResponseMessage response = await client.PostAsJsonAsync("posts", new { title = "foo", body = "bar", userId = 1 });

            User user = await response.Content.ReadAsAsync<User>();

            InitializeComponent();

            bool isLoggedIn = await Container.Resolve<ITokenService>().IsLoggedIn();

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

        protected override void RegisterTypes(IContainerRegistry containerRegistry, ContainerBuilder containerBuilder, IServiceCollection services)
        {
            containerRegistry.RegisterForNav<NavigationPage>("Nav");
            containerRegistry.RegisterForNav<MasterView, MasterViewModel>("Master");
            containerRegistry.RegisterPartialView<MenuView, MenuViewModel>();
            containerRegistry.RegisterForNav<LoginView, LoginViewModel>("Login");
            containerRegistry.RegisterForNav<ToDoItemsView, ToDoItemsViewModel>("ToDoItems");

            containerBuilder.Register<IClientAppProfile>(c => new DefaultClientAppProfile
            {
                HostUri = new Uri("http://192.168.1.215:53200/api/swagger/docs/v1"),
                // HostUri = new Uri("https://jsonplaceholder.typicode.com/"),
                ODataRoute = "odata/ToDoLine/",
                AppName = "ToDoLine",
            }).SingleInstance();

            containerBuilder.RegisterRequiredServices();
            containerBuilder.RegisterHttpClient();
            containerBuilder.RegisterODataClient();
            containerBuilder.RegisterIdentityClient();
            containerBuilder.Register<HttpMessageHandler>(c =>
            {
                return new ToDoLineAuthenticatedHttpMessageHandler(c.Resolve<IEventAggregator>(), c.Resolve<ITokenService>(), c.ResolveNamed<HttpMessageHandler>(ContractKeys.DefaultHttpMessageHandler));
            })
            .Named<HttpMessageHandler>(ContractKeys.AuthenticatedHttpMessageHandler)
            .SingleInstance();

            containerBuilder.Register(c => UserDialogs.Instance).SingleInstance();
            containerBuilder.RegisterType<DefaultToDoServie>().As<IToDoService>().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues).SingleInstance();
            containerBuilder.RegisterType<ToDoLineTokenService>().As<ITokenService>().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues).SingleInstance();

#if DEBUG
            services.AddLogging(config =>
            {
                config.AddDebug();
            });
#endif

            base.RegisterTypes(containerRegistry, containerBuilder, services);
        }
    }

    public interface ITokenService
    {
        Task AcqTokens(); // send data such as phone number to server to get both access/refresh tokens and store them into preferences
        Task ReNew(); // send both tokens to get new one from server
        Task<string> GetAuthToken(); // get access token from preferences
        Task ClearTokens(); // clear both tokens
        Task<bool> IsLoggedIn(); // check if token exitst
    }

    public class ToDoLineTokenService : ITokenService
    {
        public Task AcqTokens()
        {
            return Task.CompletedTask;
        }

        public Task ClearTokens()
        {
            return Task.CompletedTask;
        }

        public Task<string> GetAuthToken()
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> IsLoggedIn()
        {
            return Task.FromResult(true);
        }

        public Task ReNew()
        {
            return Task.CompletedTask;
        }
    }

    public class ToDoLineAuthenticatedHttpMessageHandler : DelegatingHandler
    {
        readonly ITokenService _tokenService;
        readonly IEventAggregator _eventAggregator;

        public ToDoLineAuthenticatedHttpMessageHandler(IEventAggregator eventAggregator, ITokenService tokenService, HttpMessageHandler defaultHttpMessageHandler)
            : base(defaultHttpMessageHandler)
        {
            _tokenService = tokenService;
            _eventAggregator = eventAggregator;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("Request-Id"))
                request.Headers.Add("Request-Id", Guid.NewGuid().ToString());

            // request.Headers.Authorization = new AuthenticationHeaderValue("", _tokenService.GetAuthToken());

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenService.ReNew();
                // request.Headers.Authorization = new AuthenticationHeaderValue("", _tokenService.GetAuthToken());
                response = await base.SendAsync(request, cancellationToken);
                // if re new wasn't successful, _tokenService.ClearTokens() && call _eventAggregator.GetEvent<TokenExpiredEvent>().Publish(new TokenExpiredEven {}) to go to login form!
            }

            return response;
        }
    }
}
