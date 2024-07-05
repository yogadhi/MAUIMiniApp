using YAP.Libs.ViewModels;
using YAP.Libs.Logger;
using ZXing.Net.Maui;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;

namespace YAP.Libs.Views;

public partial class ScanQRCodePage : ContentPage
{
    BaseViewModel vm { get; set; }

    public ScanQRCodePage()
    {
        try
        {
            InitializeComponent();
            vm = BindingContext as BaseViewModel;
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(ScanQRCodePage), ex);
        }
    }

    private async void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        try
        {
            vm.IsDetecting = false;
            var first = e.Results?.FirstOrDefault();
            vm.ProcessScanQRCodeCommand.Execute(first);
            await this.Navigation.PopModalAsync();
            //await Application.Current.MainPage.Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(BarcodesDetected), ex);
        }
    }
}