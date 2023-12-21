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

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        //if (await isAuthenticated())
        //{
        //    //await Shell.Current.GoToAsync("otp");
        //    await Navigation.PushModalAsync(new AppFlyout(MenuItemList));
        //}
        //else
        //{
        //    //await Shell.Current.GoToAsync("login");
        //    await Navigation.PushModalAsync(new LoginPage());
        //}
        //base.OnNavigatedTo(args);
    }

    protected override async void OnAppearing()
    {
        if (await isAuthenticated())
        {
            //await Shell.Current.GoToAsync("otp");
            //await Navigation.PushModalAsync(new AppFlyout(RootItem));
            Application.Current.MainPage = new AppFlyout(RootItem);
        }
        else
        {
            //await Shell.Current.GoToAsync("login");
            //await Navigation.PushModalAsync(new LoginPage(RootItem));
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