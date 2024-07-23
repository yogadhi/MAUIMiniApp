#if __ANDROID__
using Android.Content.Res;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

using CommunityToolkit.Mvvm.Messaging;
using MAUIMiniApp.Views;
using YAP.Libs.Interfaces;
using YAP.Libs.Logger;
using YAP.Libs.Models;
using YAP.Libs.Views;
using YAP.Libs.Helpers;

namespace MAUIMiniApp
{
    public partial class App : Application
    {
        public static IAlertService AlertSvc { get; set; }
        public static RootItem RootItem { get; set; }
        public static bool IsPopUpShow { get; set; } = false;

        public App(IServiceProvider provider)
        {
            try
            {
                InitializeComponent();

                Globals.SetAppTheme();

                RootItem = new RootItem
                {
                    Provider = provider,
                    MenuItemList = new List<FlyoutPageItem>()
                    {
                        new FlyoutPageItem { Title = "OTP", TargetType = typeof(OTPPage), TargetPage = new OTPPage() },
                    }
                };
                //RootItem.SelectedMenuIndex = 1;

                AlertSvc = RootItem.Provider.GetService<IAlertService>();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new LoadingPage(RootItem);
                });

                //                Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderLine", (handler, view) =>
                //                {
                //#if __ANDROID__
                //                (handler.PlatformView as Android.Views.View).SetBackgroundColor(Microsoft.Maui.Graphics.Colors.Transparent.ToAndroid());
                //#endif
                //                });
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(App), ex);
            }
        }

        protected override void OnStart()
        {
            try
            {
                base.OnStart();
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(OnStart), ex);
            }
        }
    }
}