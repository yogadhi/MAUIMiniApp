using MAUIMiniApp.ViewModels;
using YAP.Libs.Logger;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;
using YAP.Libs.Alerts;
using CommunityToolkit.Maui.Views;

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
                        vm.endTime = DateTime.Now;
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
                            vm.endTime = DateTime.Now;
                            Toasts.Show("New account successfully added");
                        });

                    }
                    //else if (m.Value.Key == "InitScan")
                    //{
                    //    var resPermission = await YAP.Libs.Helpers.Permission.CheckAndRequestCamera();
                    //    if (resPermission == PermissionStatus.Granted)
                    //    {
                    //        MainThread.BeginInvokeOnMainThread(async () =>
                    //        {
                    //            await Navigation.PushModalAsync(new YAP.Libs.Views.ScanQRCodePage());
                    //        });
                    //    }
                    //}
                }
            });
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OTPPage), ex);
        }
    }

    protected override async void OnAppearing()
    {
        try
        {
            base.OnAppearing();
            var hasAcceptToS = await SecureStorage.GetAsync("hasAcceptToS");
            if (string.IsNullOrEmpty(hasAcceptToS))
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushModalAsync(new ToSPage());
                });
            }
            else
            {
                vm.endTime = DateTime.Now;
                vm.timer.Start();
            }
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OnAppearing), ex);
        }
    }

    private void btnAddAccount_Clicked(object sender, EventArgs e)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.ShowPopupAsync(new NewAccountPage());
            });
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(btnAddAccount_Clicked), ex);
        }
    }
}