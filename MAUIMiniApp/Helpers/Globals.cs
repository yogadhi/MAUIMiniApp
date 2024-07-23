using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MAUIMiniApp.Models;
using YAP.Libs.Logger;
using YAP.Libs.Alerts;
using YAP.Libs;

namespace MAUIMiniApp.Helpers
{
    public class Globals
    {
        public static async void HandleExitApp()
        {
            try
            {
                var leave = await App.AlertSvc.ShowConfirmationAsync("Exit Application?", "Are you sure you want to exit application?", "Yes", "No");
                if (leave)
                {
                    Application.Current.Quit();
                }
                //MainThread.BeginInvokeOnMainThread(async () =>
                //{
                //    var leave = await DisplayAlert("Exit Application?", "Are you sure you want to exit application?", "Yes", "No");
                //    if (leave)
                //    {
                //        Application.Current.Quit();
                //    }
                //});
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(HandleExitApp), ex);
            }
        }

        public static Account ParseQRCodeCQAuth(string scanResult)
        {
            Account account = null;
            try
            {
                string[] splitDomainStr = scanResult.Split(new string[] { "://" }, StringSplitOptions.None);

                if (splitDomainStr[0] != "cq2faauth")
                {
                    Toasts.Show(Resources.Strings.AppResources.Wrong_QRCode_Format);
                    return account;
                }

                if (!splitDomainStr[1].Contains("totp"))
                {
                    Toasts.Show(Resources.Strings.AppResources.Wrong_QRCode_Format);
                    return account;
                }

                string[] data = splitDomainStr[1].Replace("totp/", "").Split(new string[] { "?" }, StringSplitOptions.None);
                if (data.Length < 2)
                {
                    Toasts.Show(Resources.Strings.AppResources.Wrong_QRCode_Format);
                    return account;
                }

                string AccountName = data[0];
                string[] param = data[1].Split(new string[] { "&" }, StringSplitOptions.None);

                string KeyParam = param.ToList().Where(x => x.Contains("secret")).FirstOrDefault();
                if (String.IsNullOrEmpty(KeyParam))
                {
                    Toasts.Show(Resources.Strings.AppResources.Wrong_QRCode_Format);
                    return account;
                }

                string[] splitKeyStr = KeyParam.Split(new string[] { "=" }, StringSplitOptions.None);
                string Key = splitKeyStr[1];

                if (String.IsNullOrEmpty(YAP.Libs.Helpers.Globals.Base32Decode(Key)))
                {
                    Toasts.Show(Resources.Strings.AppResources.Wrong_QRCode_Format);
                    return account;
                }

                string companycodeParam = param.ToList().Where(x => x.Contains("companycode")).FirstOrDefault();
                if (String.IsNullOrEmpty(companycodeParam))
                {
                    Toasts.Show(Resources.Strings.AppResources.Wrong_QRCode_Format);
                    return account;
                }

                string[] splitcompanycodeStr = companycodeParam.Split(new string[] { "=" }, StringSplitOptions.None);
                string CompanyCode = splitcompanycodeStr[1];

                account = new Account
                {
                    Accode = AccountName.ToUpper(),
                    SecretKey = Key.ToUpper(),
                    CompanyCode = CompanyCode.ToUpper(),
                };
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ParseQRCodeCQAuth), ex);
                return null;
            }
            return account;
        }
    }
}
