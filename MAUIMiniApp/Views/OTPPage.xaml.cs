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
            BindingContext = vm = new OTPViewModel(Navigation);

            if (Application.Current.UserAppTheme == AppTheme.Dark || Application.Current.UserAppTheme == AppTheme.Unspecified)
            {
                btnChangeTheme.IconImageSource = ImageSource.FromFile(YAP.Libs.Converters.SVGToPNGConverter.ConvertSVGToPNG("light_mode.svg"));
            }
            else if (Application.Current.UserAppTheme == AppTheme.Light)
            {
                btnChangeTheme.IconImageSource = ImageSource.FromFile(YAP.Libs.Converters.SVGToPNGConverter.ConvertSVGToPNG("dark_mode.svg"));
            }

            WeakReferenceMessenger.Default.Register<MyMessage>(this, (r, m) =>
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
                }
            });
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OTPPage), ex);
        }
    }

    private async void OTPPage_Loaded(object sender, EventArgs e)
    {
        try
        {
            var hasAcceptToS = await SecureStorage.GetAsync("hasAcceptToS");
            if (string.IsNullOrEmpty(hasAcceptToS))
            {
                await YAP.Libs.Helpers.NavigationServices.PushModalAsync(Navigation, new ToSPage());
            }
            else
            {
                vm.endTime = DateTime.Now;
                vm.timer.Start();
            }
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OTPPage_Loaded), ex);
        }
    }

    protected override void OnAppearing()
    {
        try
        {
            base.OnAppearing();
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
                btnChangeTheme.IconImageSource = ImageSource.FromFile(YAP.Libs.Converters.SVGToPNGConverter.ConvertSVGToPNG("dark_mode.png"));
                Application.Current.UserAppTheme = AppTheme.Light;
                await SecureStorage.SetAsync("userAppTheme", "Light");
            }
            else if (Application.Current.UserAppTheme == AppTheme.Light)
            {
                btnChangeTheme.IconImageSource = ImageSource.FromFile(YAP.Libs.Converters.SVGToPNGConverter.ConvertSVGToPNG("light_mode.png"));
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
            Helpers.Globals.HandleExitApp();
            return true;
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(OnBackButtonPressed), ex);
            return false;
        }
    }

    private async void btnShowQRCode_Clicked(object sender, EventArgs e)
    {
        try
        {
            vm.IsBusy = true;

            var resQRCode = await Controllers.CQAuth.GetQRCode(new Models.Account { Accode = "ASDFGH", CompanyCode = "1" }, new Models.ReqGetQRCode { SysID = 1, Username = "ASDFGH" });
            if (resQRCode == null)
                return;

            if (resQRCode.data == null)
                return;

            if (!string.IsNullOrEmpty(resQRCode.data.Based64QRImg))
                return;

            var strSplit = resQRCode.data.Based64QRImg.Split("data:image/png; base64,");
            await Application.Current.MainPage.ShowPopupAsync(new YAP.Libs.Views.QRCodePage(YAP.Libs.Views.QRCodePage.QRCodeMode.Image, strSplit[1]));
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(btnShowQRCode_Clicked), ex);
        }
        finally
        {
            vm.IsBusy = false;
        }
    }
}