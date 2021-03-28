using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AptekaHelper.Logger
{
    public static class Logger
    {
        private static Action<Log> _logAction;
        private static string _path;
        public static void Init(Action<Log> action)
        {
            _logAction = action;
        }

        public static void SetLogPath(string path)
        {
            _path = path;
        }

        public static void Log(string logMessage, Exception e = null)
        {
            var log = new Log()
            {
                Data = logMessage
            };
            if (e != null)
                log.FullData = e.StackTrace;
            using (var writer = File.AppendText(_path))
            {
                writer.WriteLine($"{DateTime.Now} - {log.Data}");
            }

            _logAction?.Invoke(log);
        }

        public static void LogScreenShot(int pos, IWebDriver driver)
        {
        }
    }

    public class Log {
        public string Data { get; set; }
        public string FullData { get; set; }
    }
}
