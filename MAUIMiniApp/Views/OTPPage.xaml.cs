using YAP.Libs.Logger;

namespace MAUIMiniApp.Views;

public partial class OTPPage : ContentPage
{
    public OTPPage()
    {
        try
        {
            InitializeComponent();

            //if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            //{
            //    circularProgress.IsEnabled = false;
            //    circularProgress.IsVisible = false;

            //    lineProgress.IsEnabled = true;
            //    lineProgress.IsVisible = true;

            //    lblCounter.IsEnabled = false;
            //    lblCounter.IsVisible = false;
            //}
            //else
            //{
            //    circularProgress.IsEnabled = true;
            //    circularProgress.IsVisible = true;

            //    lineProgress.IsEnabled = false;
            //    lineProgress.IsVisible = false;

            //    lblCounter.IsEnabled = true;
            //    lblCounter.IsVisible = true;
            //}
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OTPPage) + " - " + ex.Message);
        }
    }

    protected override async void OnAppearing()
    {

    }
}