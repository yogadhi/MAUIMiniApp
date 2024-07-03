using CommunityToolkit.Maui.Views;
using YAP.Libs.Logger;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;

namespace MAUIMiniApp.Views;

public partial class NewAccountPage : Popup
{
    double entryWidth = 0;
    public NewAccountPage()
    {
        try
        {
            InitializeComponent();
            InitDisplay();

            WeakReferenceMessenger.Default.Register<MyMessage>(this, async (r, m) =>
            {
                if (m.Value as MessageContainer != null)
                {
                    if (m.Value.Key == "ClosePopUp")
                    {
                        await CloseAsync();
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(NewAccountPage) + " - " + ex.Message);
        }
    }

    private void InitDisplay()
    {
        try
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                entryWidth = DeviceDisplay.Current.MainDisplayInfo.Width / 4;
            }
            else
            {
                entryWidth = DeviceDisplay.Current.MainDisplayInfo.Width / 2;
            }

            txtCompanyCode.WidthRequest = entryWidth;
            txtAccountNo.WidthRequest = entryWidth;
            txtSecretKey.WidthRequest = entryWidth;
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(InitDisplay) + " - " + ex.Message);
        }
    }

    private void btnAddAccount_Clicked(object sender, EventArgs e)
    {
        try
        {

        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(InitDisplay) + " - " + ex.Message);
        }
    }
}