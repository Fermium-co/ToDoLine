using Autofac;
using Bit.Core.Contracts;
using Bit.Core.Implementations;
using Bit.ViewModel.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Prism.Autofac;
using Prism.Ioc;
using Syncfusion.ListView.XForms.UWP;

namespace ToDoLineApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

#if DEBUG
            DebugTelemetryService.Current.Init();
#endif

            SfListViewRenderer.Init();

            LoadApplication(new ToDoLineApp.App(new ToDoLinePlatformInitializer()));
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
