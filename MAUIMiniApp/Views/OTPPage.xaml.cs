using YAP.Libs.Logger;

namespace MAUIMiniApp.Views;

public partial class OTPPage : ContentPage
{
    public OTPPage()
    {
        try
        {
            InitializeComponent();

            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                circularProgress.IsEnabled = false;
                circularProgress.IsVisible = false;
            }
            else
            {
                circularProgress.IsEnabled = true;
                circularProgress.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OTPPage) + " - " + ex.Message);
        }
    }
}