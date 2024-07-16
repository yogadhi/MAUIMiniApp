using YAP.Libs.ViewModels;
using YAP.Libs.Logger;
using ZXing.Net.Maui;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;
using System.Collections.Generic;
using YAP.Libs.Helpers;

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

            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                btnBack.Source = ImageSource.FromFile("arrow_back_light.png");
            }
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
            var scanResult = e.Results?.FirstOrDefault();
            if (scanResult != null)
            {
                var format = scanResult.Format;
                var val = scanResult.Value;
                if (!string.IsNullOrWhiteSpace(val))
                {
                    await NavigationServices.PopModalAsync(Navigation);
                    WeakReferenceMessenger.Default.Send(new MyMessage(new MessageContainer { Key = "ScanResult", CustomObject = val }));
                }
            }
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(BarcodesDetected), ex);
        }
    }

    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        try
        {
            await NavigationServices.PopModalAsync(Navigation);
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(btnBack_Clicked), ex);
        }
    }
}