using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;
using YAP.Libs.Logger;

namespace MAUIMiniApp.Views;

public partial class ToSPage : ContentPage
{
    public ToSPage()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(ToSPage), ex);
        }
    }

    private void btnReject_Clicked(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }
}