using MAUIMiniApp.ViewModels;
using YAP.Libs.Logger;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;
using MAUIMiniApp.Models;

namespace MAUIMiniApp.Views;
//https://svgtrace.com/svg-to-png

public partial class OTPPage : ContentPage
{
    OTPViewModel vm { get; set; }

    public OTPPage()
    {
        try
        {
            InitializeComponent();
            vm = BindingContext as OTPViewModel;

            WeakReferenceMessenger.Default.Register<MyMessage>(this, async (r, m) =>
            {
                if (m.Value != null)
                {
                    if (m.Value.Key == "ClosePopUp")
                    {
                        vm.timer.Stop();
                        vm.LoadCommand.Execute(null);
                    }
                    else if (m.Value.Key == "ScanResult")
                    {
                        if (m.Value.CustomObject is string)
                        {
                            var scanRes = (string)m.Value.CustomObject;
                            vm.DecodeScanResultCommand.Execute(scanRes);
                        }
                    }
                    else if (m.Value.Key == "RefreshList")
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            YAP.Libs.Alerts.Toasts.Show("New account successfully added");
                            vm.timer.Stop();
                            vm.LoadCommand.Execute(null);
                        });

                    }
                }
            });
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OTPPage), ex);
        }
    }

    protected override void OnAppearing()
    {
        try
        {
            base.OnAppearing();
            //if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            //{
            //    WeakReferenceMessenger.Default.Send(new MyMessage(new MessageContainer { Key = "MainScreenLoaded" }));
            //}
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OnAppearing), ex);
        }
    }
}