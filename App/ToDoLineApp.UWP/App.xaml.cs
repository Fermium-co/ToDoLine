﻿using Acr.UserDialogs;
using Bit.ViewModel;
using FFImageLoading.Forms;
using FFImageLoading.Forms.Platform;
using Syncfusion.ListView.XForms.UWP;
using Syncfusion.XForms.UWP.BadgeView;
using Syncfusion.XForms.UWP.Buttons;
using Syncfusion.XForms.UWP.TextInputLayout;
using System.Linq;
using System.Reflection;
using ToDoLineApp.Implementations;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ToDoLineApp.UWP
{
    sealed partial class App
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

                CachedImageRenderer.Init();

                Xamarin.Forms.Forms.Init(e, new Assembly[]
                {   typeof(CachedImage).GetTypeInfo().Assembly,
                    typeof(CachedImageRenderer).GetTypeInfo().Assembly,
                    typeof(SfTextInputLayoutRenderer).Assembly,
                    typeof(SfBadgeViewRenderer).GetTypeInfo().Assembly,
                    typeof(SfButtonRenderer).GetTypeInfo().Assembly,
                    typeof(UserDialogs).Assembly,
                    typeof(SfListViewRenderer).GetTypeInfo().Assembly
                }.Union(GetBitRendererAssemblies()));

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
