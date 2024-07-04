using CommunityToolkit.Maui.Views;
using YAP.Libs.Logger;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;

namespace MAUIMiniApp.Views;

public partial class NewAccountPage : Popup
{
    double width = 0;
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
            Log.Write(Log.LogEnum.Error, nameof(NewAccountPage), ex);
        }
    }

    private void InitDisplay()
    {
        try
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                width = DeviceDisplay.Current.MainDisplayInfo.Width / 4;
                this.mainFrame.WidthRequest = width;
            }
            else
            {
                width = DeviceDisplay.Current.MainDisplayInfo.Width / 3;
                this.Size = new Microsoft.Maui.Graphics.Size(width, 0);
            }
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(InitDisplay), ex);
        }
    }

    private async void btnClose_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private async void btnScanQRCode_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}