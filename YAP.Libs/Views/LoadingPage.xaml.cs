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
#if DEBUG
            SecureStorage.RemoveAll();
#endif
            //hardcoded because CQ Auth no need login page
            await SecureStorage.SetAsync("hasAuth", "true");

            if (await isAuthenticated())
            {
                if (RootItem != null)
                {
                    if (RootItem.MenuItemList != null)
                    {
                        if (RootItem.MenuItemList.Count > 1)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Application.Current.MainPage = new AppFlyout(RootItem);
                            });
                        }
                        else
                        {
                            var hasAuth = await SecureStorage.GetAsync("hasAuth");
                            var hasAcceptToS = await SecureStorage.GetAsync("hasAcceptToS");

                            if (!string.IsNullOrEmpty(hasAuth) && string.IsNullOrEmpty(hasAcceptToS))
                            {
                                WeakReferenceMessenger.Default.Send(new MyMessage(new MessageContainer { Key = "hasAcceptedToS", CustomObject = false }));
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
                    }
                }
            }
            else
            {
                Application.Current.MainPage = new LoginPage(RootItem);
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
            var hasAuth = await SecureStorage.GetAsync("hasAuth");
            return !(hasAuth == null);
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(isAuthenticated), ex);
            return false;
        }
    }
}