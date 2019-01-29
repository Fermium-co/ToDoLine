﻿using Acr.UserDialogs;
using Autofac;
using Bit;
using Bit.Model.Events;
using Bit.ViewModel.Contracts;
using Bit.ViewModel.Implementations;
using FFImageLoading;
using FFImageLoading.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Prism;
using Prism.Autofac;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Net.Http;
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

        protected override void RegisterTypes(IContainerRegistry containerRegistry, ContainerBuilder containerBuilder, IServiceCollection services)
        {
            containerRegistry.RegisterForNav<NavigationPage>("Nav");
            containerRegistry.RegisterForNav<MasterView, MasterViewModel>("Master");
            containerRegistry.RegisterPartialView<MenuView, MenuViewModel>();
            containerRegistry.RegisterForNav<LoginView, LoginViewModel>("Login");
            containerRegistry.RegisterForNav<ToDoItemsView, ToDoItemsViewModel>("ToDoItems");

            containerBuilder.Register<IClientAppProfile>(c => new DefaultClientAppProfile
            {
                HostUri = new Uri("http://192.168.1.215:53200/"),
                ODataRoute = "odata/ToDoLine/",
                AppName = "ToDoLine",
            }).SingleInstance();

            containerBuilder.RegisterRequiredServices();
            containerBuilder.RegisterHttpClient();
            containerBuilder.RegisterODataClient();
            containerBuilder.RegisterIdentityClient();

            containerBuilder.Register(c => UserDialogs.Instance).SingleInstance();
            containerBuilder.RegisterType<DefaultToDoServie>().As<IToDoService>().PropertiesAutowired(PropertyWiringOptions.PreserveSetValues).SingleInstance();

            base.RegisterTypes(containerRegistry, containerBuilder, services);
        }
    }
}
