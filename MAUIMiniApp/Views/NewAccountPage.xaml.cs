using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Logger;
using YAP.Libs.Models;
using YAP.Libs.Views;
using YAP.Libs.Helpers;
using MAUIMiniApp.ViewModels;
using System.Text.RegularExpressions;

namespace MAUIMiniApp.Views;

public partial class NewAccountPage : Popup
{
    NewAccountViewModel vm;
    public NewAccountPage()
    {
        try
        {
            InitializeComponent();
            BindingContext = vm = new NewAccountViewModel(Application.Current.MainPage.Navigation);

            Globals.InitPopUpPageDisplay(mainFrame, this, btnClose, false);

            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                btnAddAccount.ImageSource = ImageSource.FromFile("person_add_light.png");
                btnScanQRCode.ImageSource = ImageSource.FromFile("qr_code_scanner.png");
            }

            txtCompanyCode.TextChanged += Globals.NumericOnly_TextChanged;
            txtAccountNo.TextChanged += Globals.AlphabetOnly_TextChanged;

            WeakReferenceMessenger.Default.Register<MyMessage>(this, (r, m) =>
            {
                if (m.Value != null)
                {
                    if (m.Value.Key == "ClosePopUp")
                    {
                        App.IsPopUpShow = false;
                        MainThread.BeginInvokeOnMainThread(async () => { await CloseAsync(); });
                    }
                }
            });

            App.IsPopUpShow = true;
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(NewAccountPage), ex);
        }
    }

    private void btnClose_Clicked(object sender, EventArgs e)
    {
        try
        {
            App.IsPopUpShow = false;
            MainThread.BeginInvokeOnMainThread(async () => { await CloseAsync(); });
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(btnClose_Clicked), ex);
        }
    }

    private async void btnScanQRCode_Clicked(object sender, EventArgs e)
    {
        try
        {
            App.IsPopUpShow = false;
            MainThread.BeginInvokeOnMainThread(async () => { await CloseAsync(); });

            var resPermission = await Permission.CheckAndRequestCamera();
            if (resPermission == PermissionStatus.Granted)
            {
                MainThread.BeginInvokeOnMainThread(async () => { await vm.Navigation.PushModalAsync(new ScanQRCodePage()); });
            }
        }
        catch (Exception ex)
        {
            Log.Write(Log.LogEnum.Error, nameof(btnScanQRCode_Clicked), ex);
        }
    }
}