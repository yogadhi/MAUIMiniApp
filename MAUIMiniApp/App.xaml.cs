#if __ANDROID__
using Android.Content.Res;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif

using MAUIMiniApp.Views;
using YAP.Libs.Interfaces;
using YAP.Libs.Logger;
using YAP.Libs.Models;
using YAP.Libs.Views;

namespace MAUIMiniApp
{
    public partial class App : Application
    {
        public static IAlertService AlertSvc;
        private static RootItem RootItem { get; set; }

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
                    new FlyoutPageItem { Title = "OTP", IconSource = "reminders.png", TargetType = typeof(OTPPage) },
                    new FlyoutPageItem { Title = "Contacts", IconSource = "contacts.png", TargetType = typeof(MainPage) },
                    new FlyoutPageItem { Title = "Todo List", IconSource = "todo.png", TargetType = typeof(MainPage) },
                    //new FlyoutPageItem { Title = "Reminders", IconSource = "reminders.png", TargetType = typeof(MainPage) },
                };
                //MainPage = new AppFlyout(MenuItemList);

                MainPage = new LoadingPage(RootItem);

                //MainPage = new AppShell();
                //MainPage = new AppTabbed();
                //MainPage = new NavigationPage(new AppFlyout());

                Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderLine", (handler, view) =>
                {
#if __ANDROID__
                (handler.PlatformView as Android.Views.View).SetBackgroundColor(Microsoft.Maui.Graphics.Colors.Transparent.ToAndroid());
#endif
                });
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(App) + " - " + ex.Message);
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
                Log.Write(Log.LogEnum.Error, nameof(OnStart) + " - " + ex.Message);
            }
        }

        public static async Task<Dictionary<string, string>> GetVersionInfoList()
        {
            Dictionary<string, string> infoList = new Dictionary<string, string>();

            try
            {
                await Task.Delay(2000);

                var IsFirst = VersionTracking.Default.IsFirstLaunchEver.ToString();
                infoList.Add("IsFirst", IsFirst);
                var CurrentVersionIsFirst = VersionTracking.Default.IsFirstLaunchForCurrentVersion.ToString();
                infoList.Add("CurrentVersionIsFirst", CurrentVersionIsFirst);
                var CurrentBuildIsFirst = VersionTracking.Default.IsFirstLaunchForCurrentBuild.ToString();
                infoList.Add("CurrentBuildIsFirst", CurrentBuildIsFirst);
                var CurrentVersion = VersionTracking.Default.CurrentVersion.ToString();
                infoList.Add("CurrentVersion", CurrentVersion);
                var CurrentBuild = VersionTracking.Default.CurrentBuild.ToString();
                infoList.Add("CurrentBuild", CurrentBuild);
                var FirstInstalledVer = VersionTracking.Default.FirstInstalledVersion.ToString();
                infoList.Add("FirstInstalledVer", FirstInstalledVer);
                var FirstInstalledBuild = VersionTracking.Default.FirstInstalledBuild.ToString();
                infoList.Add("FirstInstalledBuild", FirstInstalledBuild);
                var VersionHistory = String.Join(',', VersionTracking.Default.VersionHistory);
                infoList.Add("VersionHistory", VersionHistory);
                var BuildHistory = String.Join(',', VersionTracking.Default.BuildHistory);
                infoList.Add("BuildHistory", BuildHistory);

                // These two properties may be null if this is the first version
                var PreviousVersion = VersionTracking.Default.PreviousVersion?.ToString() ?? "none";
                infoList.Add("PreviousVersion", PreviousVersion);
                var PreviousBuild = VersionTracking.Default.PreviousBuild?.ToString() ?? "none";
                infoList.Add("PreviousBuild", PreviousBuild);
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(GetVersionInfoList) + " - " + ex.Message);
            }
            return infoList;
        }
    }
}