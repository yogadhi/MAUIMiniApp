using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace MAUIMiniApp
{
    public partial class GetDeviceInfo
    {
        public partial string GetDeviceID()
        {
            string deviceID = UIDevice.CurrentDevice.IdentifierForVendor.ToString();
            return deviceID;
        }
    }
}
