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

namespace MAUIMiniApp
{
    public partial class App : Application
    {
        public static IAlertService AlertSvc { get; set; }
        public static RootItem RootItem { get; set; }
        public static Dictionary<string, string> InfoList { get; set; }
        public static int WindowWidth = 800;
        public static int WindowHeight = 400;

        public App(IServiceProvider provider)
        {
            try
            {
                InitializeComponent();

                RootItem = new RootItem
                {
                    Provider = provider,
                    MenuItemList = new List<FlyoutPageItem>()
                };

                AlertSvc = RootItem.Provider.GetService<IAlertService>();

                RootItem.MenuItemList = new List<FlyoutPageItem>
                {
                    new FlyoutPageItem { Title = "OTP", IconSource = "reminders.png", TargetType = typeof(OTPPage), TargetPage = new OTPPage() },
                };

                //RootItem.SelectedMenuIndex = 1;

                MainPage = new LoadingPage(RootItem);

                Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderLine", (handler, view) =>
                {
#if __ANDROID__
                (handler.PlatformView as Android.Views.View).SetBackgroundColor(Microsoft.Maui.Graphics.Colors.Transparent.ToAndroid());
#endif
                });

                WeakReferenceMessenger.Default.Register<MyMessage>(this, (r, m) =>
                {
                    if (m.Value != null)
                    {
                        if (m.Value.Key == "hasAcceptedToS")
                        {
                            if ((bool)m.Value.CustomObject)
                            {
                                var page = RootItem.MenuItemList[0].TargetPage;
                                MainPage = new NavigationPage(page);
                            }
                            else
                            {
                                MainPage = new ToSPage();
                            }
                        }
                    }
                });

//                if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
//                {
//                    WeakReferenceMessenger.Default.Register<MyMessage>(this, (r, m) =>
//                    {
//                        if (m.Value != null)
//                        {
//                            if (m.Value.Key == "MainScreenLoaded")
//                            {
//                                Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
//                                {
//#if WINDOWS
//            var mauiWindow = handler.VirtualView;
//            var nativeWindow = handler.PlatformView;
//            nativeWindow.Activate();
//            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
//            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
//            AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
//            appWindow.Resize(new SizeInt32(WindowWidth, WindowHeight));
//#endif
//                                });
//                            }
//                        }
//                    });
//                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(App), ex);

            }
        }

        protected override async void OnStart()
        {
            try
            {
                base.OnStart();

                var versionInfo = await GetVersionInfoList();
                if (versionInfo != null)
                {

                }
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(OnStart), ex);
            }
        }

        public static async Task<Dictionary<string, string>> GetVersionInfoList()
        {

            try
            {
                InfoList = new Dictionary<string, string>();

                var IsFirst = VersionTracking.Default.IsFirstLaunchEver.ToString();
                InfoList.Add("IsFirst", IsFirst);

                var CurrentVersionIsFirst = VersionTracking.Default.IsFirstLaunchForCurrentVersion.ToString();
                InfoList.Add("CurrentVersionIsFirst", CurrentVersionIsFirst);

                var CurrentBuildIsFirst = VersionTracking.Default.IsFirstLaunchForCurrentBuild.ToString();
                InfoList.Add("CurrentBuildIsFirst", CurrentBuildIsFirst);

                var CurrentVersion = VersionTracking.Default.CurrentVersion.ToString();
                InfoList.Add("CurrentVersion", CurrentVersion);

                var CurrentBuild = VersionTracking.Default.CurrentBuild.ToString();
                InfoList.Add("CurrentBuild", CurrentBuild);

                var FirstInstalledVer = VersionTracking.Default.FirstInstalledVersion.ToString();
                InfoList.Add("FirstInstalledVer", FirstInstalledVer);

                var FirstInstalledBuild = VersionTracking.Default.FirstInstalledBuild.ToString();
                InfoList.Add("FirstInstalledBuild", FirstInstalledBuild);

                var VersionHistory = String.Join(',', VersionTracking.Default.VersionHistory);
                InfoList.Add("VersionHistory", VersionHistory);

                var BuildHistory = String.Join(',', VersionTracking.Default.BuildHistory);
                InfoList.Add("BuildHistory", BuildHistory);

                // These two properties may be null if this is the first version
                var PreviousVersion = VersionTracking.Default.PreviousVersion?.ToString() ?? "none";
                InfoList.Add("PreviousVersion", PreviousVersion);

                var PreviousBuild = VersionTracking.Default.PreviousBuild?.ToString() ?? "none";
                InfoList.Add("PreviousBuild", PreviousBuild);
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(GetVersionInfoList), ex);
            }
            return InfoList;
        }
    }
}