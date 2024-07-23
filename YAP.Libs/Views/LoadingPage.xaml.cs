using YAP.Libs.Flyouts;
using YAP.Libs.Models;
using YAP.Libs.Logger;
using CommunityToolkit.Mvvm.Messaging;

namespace YAP.Libs.Views;

public partial class LoadingPage : ContentPage
{
    RootItem RootItem;

    public LoadingPage(RootItem rootItem)
    {
        try
        {
            InitializeComponent();
            RootItem = rootItem;
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(LoadingPage), ex);
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {

    }

    protected override async void OnAppearing()
    {
        try
        {
            //#if DEBUG
            //            Preferences.Default.Clear();
            //#endif

            //hardcoded because CQ Auth no need login page
            Log.Write(Log.LogEnum.Error, nameof(OnAppearing), "test logger");
            //var logs = Log.ReadLogs();

            Preferences.Default.Set("hasAuth", true);

            if (await isAuthenticated())
            {
                if (RootItem == null)
                {
                    return;
                }

                if (RootItem.MenuItemList == null)
                {
                    return;
                }

                if (RootItem.MenuItemList.Count > 1)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = new AppFlyout(RootItem);
                    });
                }
                else
                {
                    var page = RootItem.MenuItemList[0].TargetPage;

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = new NavigationPage(page);
                    });
                }
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new LoginPage(RootItem);
                });
            }
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OnAppearing), ex);
        }
    }

    async Task<bool> isAuthenticated()
    {
        try
        {
            await Task.Delay(1000);
            var hasAuth = Preferences.Default.Get("hasAuth", false);
            return !(hasAuth == false);
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(isAuthenticated), ex);
            return false;
        }
    }
}