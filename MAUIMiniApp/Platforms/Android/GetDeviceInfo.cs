using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidApp = Android.App.Application;
using Setting = Android.Provider.Settings;
using static Android.Provider.Settings;

namespace MAUIMiniApp
{
    public partial class GetDeviceInfo
    {
        public partial string GetDeviceID()
        {
            var context = AndroidApp.Context;

            string id = Setting.Secure.GetString(context.ContentResolver, Secure.AndroidId);

            return id;
        }
    }
}
