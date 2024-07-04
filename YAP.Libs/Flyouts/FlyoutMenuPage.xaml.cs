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
            Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(FlyoutMenuPage), ex);
        }
    }

    private void btnLogout_Clicked(object sender, EventArgs e)
    {
        try
        {
            SecureStorage.Remove("hasAuth");
            SecureStorage.Remove("hasAcceptToS");
            Application.Current.MainPage = new LoginPage(RootItem);
        }
        catch (Exception ex)
        {
            Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(btnLogout_Clicked), ex);
        }
    }
}