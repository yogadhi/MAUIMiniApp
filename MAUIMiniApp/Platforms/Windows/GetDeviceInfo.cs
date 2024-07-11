
using System.Net.NetworkInformation;


namespace MAUIMiniApp
{
    public partial class GetDeviceInfo
    {
        public partial string GetDeviceID()
        {
            try
            {
                string macAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                     where nic.OperationalStatus == OperationalStatus.Up
                                     select nic.GetPhysicalAddress().ToString()
                    ).FirstOrDefault();
                return macAddress;
            }
            catch (Exception ex)
            {
                YAP.Libs.Logger.Log.Write(YAP.Libs.Logger.Log.LogEnum.Error, nameof(GetDeviceID), ex);
                return string.Empty;
            }
        }
    }
}
