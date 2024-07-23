using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
            string fileName = logEnum.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            string targetFile = string.Empty;
            string mainPath = FileSystem.AppDataDirectory;

            if (input is Exception)
            {
                var ex = (Exception)input;
                message += ex.Source + " - " + ex.StackTrace + " - " + ex.Message;
            }
            else if (input is string)
            {
                message += (string)input;
            }

#if ANDROID
mainPath = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
#endif

            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
                if (!Directory.Exists(mainPath))
                {
                    Directory.CreateDirectory(mainPath);
                }
            }

            targetFile = Path.Combine(mainPath, fileName);



            if (File.Exists(targetFile))
            {
                using (FileStream fs = new FileStream(targetFile, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(message);
                }
            }
            else
            {
                //MainThread.BeginInvokeOnMainThread(async () =>
                //{
                FileStream outputStream = File.OpenWrite(targetFile);
                using StreamWriter streamWriter = new StreamWriter(outputStream);
                streamWriter.Write(message);
                //});
            }
        }


        public static List<string> ReadLogs()
        {
            string mainPath = string.Empty;
            List<string> logs = new List<string>();

#if ANDROID
mainPath = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath;
#elif WINDOWS
mainPath = AppDomain.CurrentDomain.BaseDirectory + @"Log\";
#else
            mainPath = FileSystem.AppDataDirectory;
#endif

            var files = Directory.GetFiles(mainPath, "*.txt");
            if (files != null)
            {
                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        //string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
                        using FileStream InputStream = System.IO.File.OpenRead(file);
                        using StreamReader reader = new StreamReader(InputStream);
                        logs.Add(reader.ReadLine());
                    }
                }
            }
            return logs;
        }
    }
}
