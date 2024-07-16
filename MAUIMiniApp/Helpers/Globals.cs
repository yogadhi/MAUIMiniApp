using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
                YAP.Libs.Logger.Log.Write(YAP.Libs.Logger.Log.LogEnum.Error, nameof(HandleExitApp), ex);
            }
        }
    }
}
