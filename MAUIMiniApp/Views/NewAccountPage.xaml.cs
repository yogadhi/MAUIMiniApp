using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using YAP.Libs.Logger;
using YAP.Libs.Models;
using YAP.Libs.Views;
using YAP.Libs.Helpers;
using MAUIMiniApp.ViewModels;
using System.Text.RegularExpressions;
using MAUIMiniApp.Models;

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

            Globals.InitPopUpPageDisplay(mainFrame, this, false);

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
            //var account = ParseQRCode("cq2faauth://totp/ASDFGH?secret=NVKGMTTYKZRVOSSLNZ4DC5Z5HU&issuer=10&companycode=10");
            //if(account != null)
            //{
            //}

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

    public static Account ParseQRCode(string scanResult)
    {
        Account account = null;
        try
        {
            string[] splitDomainStr = scanResult.Split(new string[] { "://" }, StringSplitOptions.None);

            if (splitDomainStr[0] != "cq2faauth")
            {
                throw new Exception("Invalid or missing scheme in uri");
            }

            if (!splitDomainStr[1].Contains("totp"))
            {
                throw new Exception("Invalid or missing authority in uri");
            }

            string[] data = splitDomainStr[1].Replace("totp/", "").Split(new string[] { "?" }, StringSplitOptions.None);
            if (data.Length < 2)
            {
                throw new Exception("Missing user id in uri");
            }

            string AccountName = data[0];
            string[] param = data[1].Split(new string[] { "&" }, StringSplitOptions.None);

            string KeyParam = param.ToList().Where(x => x.Contains("secret")).FirstOrDefault();
            if (String.IsNullOrEmpty(KeyParam))
            {
                throw new Exception("Secret key not found in URI");
            }

            string[] splitKeyStr = KeyParam.Split(new string[] { "=" }, StringSplitOptions.None);
            string Key = splitKeyStr[1];

            if (String.IsNullOrEmpty(YAP.Libs.Helpers.Globals.Base32Decode(Key)))
            {
                throw new Exception("ERROR QR");
            }

            string companycodeParam = param.ToList().Where(x => x.Contains("companycode")).FirstOrDefault();
            if (String.IsNullOrEmpty(companycodeParam))
            {
                throw new Exception("Missing user id in uri");
            }

            string[] splitcompanycodeStr = companycodeParam.Split(new string[] { "=" }, StringSplitOptions.None);
            string CompanyCode = splitcompanycodeStr[1];

            account = new Account()
            {
                Accode = CompanyCode,
                SecretKey = Key.ToUpper(),
                CompanyCode = CompanyCode.ToUpper(),
            };
        }
        catch (Exception ex)
        {
            return null;
        }

        return account;
    }
}