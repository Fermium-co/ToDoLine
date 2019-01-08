using Acr.UserDialogs;
using Bit.ViewModel;
using Syncfusion.XForms.UWP.TextInputLayout;
using System.Linq;
using System.Reflection;
using ToDoLineApp.Implementations;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ToDoLineApp.UWP
{
    sealed partial class App : Bit.UWP.BitApplication
    {
        static App()
        {
            BitExceptionHandler.Current = new ToDoLineExceptionHandler();
        }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                rootFrame = new Frame();

                UseDefaultConfiguration();

                Xamarin.Forms.Forms.Init(e, new Assembly[]
                {
                    typeof(SfTextInputLayoutRenderer).Assembly,
                    typeof(UserDialogs).Assembly
                }.Union(Rg.Plugins.Popup.Popup.GetExtraAssemblies()));

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            Window.Current.Activate();
        }
    }
}
