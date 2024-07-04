using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAP.Libs.Logger
{
    public class Log
    {
        public enum LogEnum
        {
            Error = 0,
            Success = 1,
            Log = 2
        }

        public static void Write(LogEnum logEnum, string functionName, object input)
        {
            string message = functionName + " - ";

            if (input is Exception)
            {
                var ex = (Exception)input;
                message += ex.Source + " - " + ex.StackTrace + " - " + ex.Message;
            }
            else if (input is string)
            {
                message += (string)input;
            }

            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + @"Log\";
                var fileName = logEnum.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                var filePathName = Path.Combine(filePath, fileName);

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                StreamWriter sw = new StreamWriter(filePathName, true);
                sw.WriteLine(message);
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android || DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {

            }
        }
    }
}
