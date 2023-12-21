using YAP.Libs.Alerts;
using YAP.Libs.Flyouts;
using YAP.Libs.Interfaces;
using YAP.Libs.Models;

namespace YAP.Libs.Views;

public partial class LoginPage : ContentPage
{
    IAlertService AlertSvc { get; set; }
    RootItem RootItem { get; set; }

    public LoginPage(RootItem rootItem)
	{
		InitializeComponent();

        RootItem = rootItem;
        AlertSvc = RootItem.Provider.GetService<IAlertService>();
    }

    protected override bool OnBackButtonPressed()
    {
        Application.Current.Quit();
        return true;
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        if (IsCredentialCorrect(Username.Text, Password.Text))
        {
            Toasts.Show("Login success");
            await SecureStorage.SetAsync("hasAuth", "true");
            //await Shell.Current.GoToAsync("///home");
            //await Navigation.PushModalAsync(new AppFlyout(RootItem));
            Application.Current.MainPage = new AppFlyout(RootItem);
        }
        else
        {
            await AlertSvc.ShowAlertAsync("Login failed", "Username or password is invalid", "Try again");
            //await DisplayAlert("Login failed", "Username or password is invalid", "Try again");
        }
    }


    bool IsCredentialCorrect(string username, string password)
    {
        return Username.Text == "admin" && Password.Text == "123456";
    }
}