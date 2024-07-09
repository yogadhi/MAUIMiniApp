using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YAP.Libs.Services
{
    public class RESTAPI
    {
        public static async Task<string> POST(double timeOutMinute, string contAPIUrl, string contFunctionName, string jsonParam, string accessToken = "")
        {
            try
            {
                using (var Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(contAPIUrl);
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    Client.Timeout = TimeSpan.FromMinutes(timeOutMinute);

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    }

                    var content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                    HttpResponseMessage responce = await Client.PostAsync(contFunctionName, content);
                    if (responce.IsSuccessStatusCode)
                    {
                        var Json = await responce.Content.ReadAsStringAsync();
                        return Json;
                    }
                    else
                    {
                        Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(POST), responce.StatusCode.ToString());
                        return "";
                    }
                }
            }
            catch (TaskCanceledException taskeX)
            {
                Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(POST), taskeX);
                return "";
            }
            catch (WebException webEx)
            {
                Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(POST), webEx);
                return "";
            }
            catch (Exception ex)
            {
                Logger.Log.Write(Logger.Log.LogEnum.Error, nameof(POST), ex);
                return "";
            }
        }
    }
}
