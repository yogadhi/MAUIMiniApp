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
    double width = 0;
    NewAccountViewModel vm;
    public NewAccountPage()
    {
        try
        {
            InitializeComponent();
            BindingContext = vm = new NewAccountViewModel(Application.Current.MainPage.Navigation);

            InitDisplay();

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

    private void InitDisplay()
    {
        try
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                btnAddAccount.ImageSource = ImageSource.FromFile("person_add_light.png");
                btnScanQRCode.ImageSource = ImageSource.FromFile("qr_code_scanner.png");
                btnClose.Source = ImageSource.FromFile("close_light.png");
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

    private void txtCompanyCode_TextChanged(object sender, TextChangedEventArgs e)
    {
        // If the text field is empty or null then leave.
        string regex = e.NewTextValue;
        if (String.IsNullOrEmpty(regex))
            return;

        // If the text field only contains numbers then leave.
        if (!Regex.Match(regex, "^[0-9]+$").Success)
        {
            // This returns to the previous valid state.
            var entry = sender as Entry;
            entry.Text = (string.IsNullOrEmpty(e.OldTextValue)) ?
                    string.Empty : e.OldTextValue;
        }
    }
}