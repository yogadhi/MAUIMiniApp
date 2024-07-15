using MAUIMiniApp.ViewModels;
using YAP.Libs.Logger;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Models;
using YAP.Libs.Alerts;
using CommunityToolkit.Maui.Views;
using MAUIMiniApp.Resources.Strings;
using System;
//using static Android.Icu.Text.ListFormatter;

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

            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                btnAddAccount.IconImageSource = ImageSource.FromFile("add_light.png");

                if (Application.Current.UserAppTheme == AppTheme.Dark || Application.Current.UserAppTheme == AppTheme.Unspecified)
                {
                    btnChangeTheme.IconImageSource = ImageSource.FromFile("light_mode.png");
                }
                else if (Application.Current.UserAppTheme == AppTheme.Light)
                {
                    btnChangeTheme.IconImageSource = ImageSource.FromFile("dark_mode.png");
                }
            }

            WeakReferenceMessenger.Default.Register<MyMessage>(this, async (r, m) =>
            {
                if (m.Value != null)
                {
                    if (m.Value.Key == "ClosePopUp")
                    {
                        if (!vm.timer.Enabled)
                        {
                            vm.timer.Enabled = true;
                        }
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
                    //else if (m.Value.Key == "RefreshList")
                    //{
                    //    MainThread.BeginInvokeOnMainThread(() =>
                    //    {
                    //        vm.endTime = DateTime.Now;
                    //        Toasts.Show("New account successfully added");
                    //    });
                    //}
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
                MainThread.BeginInvokeOnMainThread(async () => { await Navigation.PushModalAsync(new ToSPage()); });
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
            if (App.IsPopUpShow) { return; }
            MainThread.BeginInvokeOnMainThread(async () => { await Application.Current.MainPage.ShowPopupAsync(new NewAccountPage()); });
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(btnAddAccount_Clicked), ex);
        }
    }

    private async void btnChangeTheme_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (Application.Current.UserAppTheme == AppTheme.Dark || Application.Current.UserAppTheme == AppTheme.Unspecified)
            {
                btnChangeTheme.IconImageSource = ImageSource.FromFile("dark_mode.png");
                Application.Current.UserAppTheme = AppTheme.Light;
                await SecureStorage.SetAsync("userAppTheme", "Light");
            }
            else if (Application.Current.UserAppTheme == AppTheme.Light)
            {
                btnChangeTheme.IconImageSource = ImageSource.FromFile("light_mode.png");
                Application.Current.UserAppTheme = AppTheme.Dark;
                await SecureStorage.SetAsync("userAppTheme", "Dark");
            }
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(btnChangeTheme_Clicked), ex);
        }
    }

    protected override bool OnBackButtonPressed()
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var leave = await DisplayAlert("Exit Application?", "Are you sure you want to exit application?", "Yes", "No");
                if (leave)
                {
                    Application.Current.Quit();
                }
            });
            return true;
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OnBackButtonPressed), ex);
            return false;
        }
    }
}