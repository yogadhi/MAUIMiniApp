using Newtonsoft.Json;
using YAP.Libs.Logger;
using YAP.Libs.Services;
using MAUIMiniApp.Models;

namespace MAUIMiniApp.Controllers
{
    public class CQAuth
    {
        public static async Task<string> Authenticate(ReqAuthenticate input)
        {
            string res = string.Empty;

            try
            {
                if (input == null)
                {
                    return res;
                }

                var jsonRequest = JsonConvert.SerializeObject(input);
                res = await RESTAPI.POST(1, Helpers.Constants.APIUrl, "token/authenticate", jsonRequest);
                if (!string.IsNullOrEmpty(res))
                {
                    res = JsonConvert.DeserializeObject<string>(res);
                }

                return res;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(Authenticate), ex);
                return res;
            }
        }

        public static async Task<ResBindUserDevice> BindUserDevice(ReqBindUserDevice input)
        {
            ResBindUserDevice res = new ResBindUserDevice();

            try
            {
                if (input == null)
                {
                    return null;
                }

                var jsonRequest = JsonConvert.SerializeObject(input);
                var resStr = await RESTAPI.POST(1, Helpers.Constants.APIUrl, "BindUserDevice", jsonRequest);
                if (!string.IsNullOrEmpty(resStr))
                {
                    res = JsonConvert.DeserializeObject<ResBindUserDevice>(resStr);
                }

                return res;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(BindUserDevice), ex);
                return null;
            }
        }
    }
}
