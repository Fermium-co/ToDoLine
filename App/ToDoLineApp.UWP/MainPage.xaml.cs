using Autofac;
using Bit.ViewModel.Implementations;
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

            SfListViewRenderer.Init();

            LoadApplication(new ToDoLineApp.App(new ToDoLinePlatformInitializer()));
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
