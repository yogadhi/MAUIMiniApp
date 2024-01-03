using YAP.Libs.Flyouts;
using YAP.Libs.Models;

namespace YAP.Libs.Views;

public partial class LoadingPage : ContentPage
{
    RootItem RootItem;

    public LoadingPage(RootItem rootItem)
    {
        InitializeComponent();
        RootItem = rootItem;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {

    }

    protected override async void OnAppearing()
    {
        if (await isAuthenticated())
        {
            //await Shell.Current.GoToAsync("otp");
            Application.Current.MainPage = new AppFlyout(RootItem);
        }
        else
        {
            //await Shell.Current.GoToAsync("login");
            Application.Current.MainPage = new LoginPage(RootItem);
        }
    }

    async Task<bool> isAuthenticated()
    {
        await Task.Delay(2000);
        var hasAuth = await SecureStorage.GetAsync("hasAuth");
        return !(hasAuth == null);
    }
}