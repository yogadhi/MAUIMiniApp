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

        public static void Write(LogEnum logEnum, string input)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + @"Log\";
            var fileName = logEnum.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            var filePathName = Path.Combine(filePath, fileName);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            StreamWriter sw = new StreamWriter(filePathName, true);
            sw.WriteLine(input);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}
