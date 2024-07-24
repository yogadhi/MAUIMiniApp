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

        public static void WriteNew(LogEnum logEnum, string functionName, object input)
        {
            string message = functionName + " - ";
            string fileName = logEnum.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
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

        public static async Task<List<string>> ReadLogs(LogEnum logEnum)
        {
            string _logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "logs");
            List<string> logs = new List<string>();

            var files = Directory.GetFiles(_logFilePath, "*.log");
            if (files != null)
            {
                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        string[] lines = await File.ReadAllLinesAsync(file);
                        if (lines != null && lines.Length > 0)
                        {
                            logs.AddRange(lines);
                        }
                    }
                }
            }
            return logs;
        }

        public static async void Write(LogEnum logEnum, string functionName, object input)
        {
            try
            {
                string message = $"{functionName} - ";
                string fileName = $"{logEnum.ToString()}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string _logFilePath = Path.Combine(filePath, "logs", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                if (input is Exception)
                {
                    var ex = (Exception)input;
                    message += $"{ex.Source} - {ex.StackTrace} - {ex.Message}";
                    SentrySdk.CaptureException(ex);
                }
                else if (input is string)
                {
                    message += (string)input;
                    message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                    SentrySdk.CaptureMessage(message);
                }
                await File.WriteAllTextAsync(_logFilePath, message);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
