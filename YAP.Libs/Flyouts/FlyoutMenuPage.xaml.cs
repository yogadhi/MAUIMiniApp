using YAP.Libs.Models;
using YAP.Libs.Views;

namespace YAP.Libs.Flyouts;

public partial class FlyoutMenuPage : ContentPage
{ 
    RootItem RootItem { get; set; }

    public FlyoutMenuPage(RootItem rootItem)
    {
        try
        {
            InitializeComponent();
            RootItem = rootItem;
            collectionView.ItemsSource = RootItem.MenuItemList;
        }
        catch (Exception ex)
        {
            Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(FlyoutMenuPage) + " - " + ex.Message);
        }
    }

    private async void btnLogout_Clicked(object sender, EventArgs e)
    {
        SecureStorage.Remove("hasAuth");
        //await Navigation.PushModalAsync(new LoginPage(RootItem));
        Application.Current.MainPage = new LoginPage(RootItem);
    }
}