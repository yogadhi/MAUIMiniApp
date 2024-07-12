using Newtonsoft.Json;
using YAP.Libs.Logger;
using YAP.Libs.Services;
using MAUIMiniApp.Models;
using MAUIMiniApp.Data;

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

        public static async Task<string> BindUserDevice(ReqBindUserDevice input, string accessToken)
        {
            //ResBindUserDevice res = new ResBindUserDevice();
            string res = string.Empty;

            try
            {
                if (input == null)
                {
                    return null;
                }

                var jsonRequest = JsonConvert.SerializeObject(input);
                var resStr = await RESTAPI.POST(1, Helpers.Constants.APIUrl, "BindUserDevice", jsonRequest, accessToken);
                if (!string.IsNullOrEmpty(resStr))
                {
                    res = JsonConvert.DeserializeObject<string>(resStr);
                }

                return res;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(BindUserDevice), ex);
                return null;
            }
        }

        public static async Task<bool> BindUserNewAccount(Account input)
        {
            try
            {
                if (input == null)
                {
                    return false;
                }

                var reqAuth = new ReqAuthenticate
                {
                    Accode = input.Accode,
                    CompanyCode = input.CompanyCode,
                    UserLogin = Helpers.Constants.UserLogin,
                    Password = Helpers.Constants.Password,
                };

                var resAuth = await Authenticate(reqAuth);
                if (!string.IsNullOrEmpty(resAuth))
                {
                    var reqBind = new ReqBindUserDevice
                    {
                        Channel = DeviceInfo.Current.Platform == DevicePlatform.iOS ? "10" : "11",
                        DeviceId = new GetDeviceInfo().GetDeviceID(),
                        EnteredKey = input.SecretKey
                    };

                    var resBind = await BindUserDevice(reqBind, resAuth);
                    if (!string.IsNullOrEmpty(resBind))
                    {
                        if (resBind == "1")
                        {
                            var resSave = await AccountDatabase.SaveItemAsync(input);
                            if (resSave == 1)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(BindUserNewAccount), ex);
                return false;
            }
        }
    }
}
