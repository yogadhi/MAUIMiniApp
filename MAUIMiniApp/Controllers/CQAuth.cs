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

        public static async Task<string> GetToken(Account input)
        {
            string res = string.Empty;

            try
            {
                if (input == null)
                {
                    return res;
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
                    res = resAuth;
                }
                return res;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(GetToken), ex);
                return res;
            }
        }

        public static async Task<ResGetQRCode> GetQRCode(Account account, ReqGetQRCode input)
        {
            ResGetQRCode res = null;

            try
            {
                if (account == null)
                {
                    return res;
                }

                if (input == null)
                {
                    return res;
                }

                var resToken = await GetToken(account);
                if (string.IsNullOrEmpty(resToken))
                {
                    return res;
                }

                var jsonRequest = JsonConvert.SerializeObject(input);
                var jsonResult = await RESTAPI.POST(1, Helpers.Constants.APIUrl, "GetQRCode", jsonRequest, resToken);
                if (!string.IsNullOrEmpty(jsonResult))
                {
                    res = new ResGetQRCode();
                    res = JsonConvert.DeserializeObject<ResGetQRCode>(jsonResult);
                }

                return res;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(GetQRCode), ex);
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

                var resAuth = await GetToken(input);
                if (string.IsNullOrEmpty(resAuth))
                {
                    return false;
                }

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
                        input.SecretKey = YAP.Libs.Helpers.Globals.EncryptString(input.SecretKey, YAP.Libs.Helpers.Globals.Salt(input.CompanyCode));
                        var resSave = await AccountDatabase.SaveItemAsync(input);
                        return resSave == 1;
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

        public static async Task<int> CheckUserBinded(Account input)
        {
            int res = -1;

            try
            {
                if (input == null)
                {
                    return res;
                }

                var resAuth = await GetToken(input);
                if (string.IsNullOrEmpty(resAuth))
                {
                    return res;
                }

                var jsonRequest = string.Empty;
                var resStr = await RESTAPI.POST(1, Helpers.Constants.APIUrl, "CheckUserBinded", jsonRequest, resAuth);
                if (!string.IsNullOrEmpty(resStr))
                {
                    res = JsonConvert.DeserializeObject<int>(resStr);
                }

                return res;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(CheckUserBinded), ex);
                return res;
            }
        }

        public static async Task<ResDelink> Delink(Account account, ReqDelink input)
        {
            ResDelink res = null;

            try
            {
                if (account == null)
                {
                    return res;
                }

                if (input == null)
                {
                    return res;
                }

                var resToken = await GetToken(account);
                if (string.IsNullOrEmpty(resToken))
                {
                    return res;
                }

                var jsonRequest = JsonConvert.SerializeObject(input);
                var resStr = await RESTAPI.POST(1, Helpers.Constants.APIUrl, "Delink", jsonRequest, resToken);
                if (!string.IsNullOrEmpty(resStr))
                {
                    res = new ResDelink();
                    res = JsonConvert.DeserializeObject<ResDelink>(resStr);
                }

                return res;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(Delink), ex);
                return res;
            }
        }

        public static async Task<string> ViewUser(string accessToken)
        {
            string res = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(accessToken))
                {
                    return res;
                }

                var jsonRequest = string.Empty;
                var resStr = await RESTAPI.POST(1, Helpers.Constants.APIUrl, "ViewUser", jsonRequest, accessToken);
                if (!string.IsNullOrEmpty(resStr))
                {
                    res = JsonConvert.DeserializeObject<string>(resStr);
                }

                return res;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(ViewUser), ex);
                return res;
            }
        }

        public static async Task<ResAuthenticateOTP> AuthenticateOTP(Account account, ReqAuthenticateOTP input)
        {
            ResAuthenticateOTP res = null;

            try
            {
                if (account == null)
                {
                    return res;
                }

                if (input == null) 
                {
                    return res;
                }

                var resAuth = await GetToken(account);
                if (string.IsNullOrEmpty(resAuth))
                {
                    return res;
                }

                var jsonRequest = JsonConvert.SerializeObject(input);
                var resStr = await RESTAPI.POST(1, Helpers.Constants.APIUrl, "Authenticate", jsonRequest, resAuth);
                if (!string.IsNullOrEmpty(resStr))
                {
                    res = new ResAuthenticateOTP();
                    res = JsonConvert.DeserializeObject<ResAuthenticateOTP>(resStr);
                }

                return res;
            }
            catch (Exception ex)
            {
                Log.Write(Log.LogEnum.Error, nameof(AuthenticateOTP), ex);
                return res;
            }
        }
    }
}
