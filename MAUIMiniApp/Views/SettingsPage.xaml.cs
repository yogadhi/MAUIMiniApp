namespace MAUIMiniApp.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    private async void LogoutButton_Clicked(object sender, EventArgs e)
    {
        await YAP.Libs.Helpers.NavigationServices.PopModalAsync(Navigation);

        //if (await App.AlertSvc.ShowConfirmationAsync("Are you sure?", "You will be logged out.", "Yes", "No"))
        //{
        //    SecureStorage.RemoveAll();
        //    await Shell.Current.GoToAsync("///login");
        //}

        //if(await DisplayAlert("Are you sure?", "You will be logged out.", "Yes", "No"))
        //{
        //    SecureStorage.RemoveAll();
        //    await Shell.Current.GoToAsync("///login");
        //}
    }
}