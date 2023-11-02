#if __ANDROID__
using Android.Content.Res;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif

using YAP.Libs.Interfaces;

namespace MAUIMiniApp
{
    public partial class App : Application
    {
        public static IServiceProvider Services;
        public static IAlertService AlertSvc;

        public App(IServiceProvider provider)
        {
            InitializeComponent();

            Services = provider;
            AlertSvc = Services.GetService<IAlertService>();


            MainPage = new AppShell();
            //MainPage = new AppFlyout();
            //MainPage = new AppTabbed();
            //MainPage = new NavigationPage(new AppFlyout());

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderLine", (handler, view) =>
            {
#if __ANDROID__
                (handler.PlatformView as Android.Views.View).SetBackgroundColor(Microsoft.Maui.Graphics.Colors.Transparent.ToAndroid());
#endif
            });
        }

        protected override async void OnStart()
        {
            base.OnStart();
            var versionInfo = await GetVersionInfoList();
            if (versionInfo != null)
            {

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
                string exMsg = ex.Message;
            }
            return infoList;
        }
    }
}